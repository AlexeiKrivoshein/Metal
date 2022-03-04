using log4net;
using MetalDiagnostic.Logger;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.Queue;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.Client
{
    //Tcp клиент, отправляет датаграммы и принимает ответы,
    //которые отправляет в ClientRequestExecutorHandler
    //содержит словарь подключений через каждое происходит отправка и ожидание данных
    public sealed class TcpMetalClient 
        : IClient
    {
        private ILog _log = LogService.GetLogger(nameof(TcpMetalClient));
        private string _serverName = "127.0.0.1";
        private int _serverPort = 9500;

        //словарь подключений к серверу
        private DatagramStream _connection;
        //токен остановки
        private CancellationTokenSource _cts;
        //очередь отправляемых сообщений
        private DatagramQueue _queue = new DatagramQueue();
        //обработчик полученных данных
        private Func<DatagramBase, bool> _handler;

        private bool _isStarted;

        private AutoResetEvent _resetLock = new AutoResetEvent(true);

        private Guid id = Guid.NewGuid();

        private Task _receive;
        private Task _send;

        private object _lock = new object();

        public TcpMetalClient()
        {
            _serverName = System.Configuration.ConfigurationManager.AppSettings["ServerName"];
            _serverPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Port"]);
        }

        public void Start()
        {
            if (_isStarted)
            {
                _log.Warn("TCP клиент уже запущен" + Environment.NewLine + Environment.StackTrace);
                return;
            }

            _cts = new CancellationTokenSource();

            _connection = Connect();

            //запуск ожидания сообщений
            _receive = new Task(() => DataReceiving());
            
            //запуск отправки сообщений
            _send = new Task(() => DataSend());
            
            _isStarted = true;
            _receive.Start();
            _send.Start();
        }

        /// <summary>
        /// Добавить датаграмму в очередь отправляемых
        /// </summary>
        /// <param name="datagram">Датаграмма</param>
        /// <param name="token">Токен отмены отправки</param>
        public void Enqueue(DatagramBase datagram, CancellationToken token, TaskCompletionSource<byte[]> tcs)
        {
            if (!_isStarted)
                throw new InvalidOperationException("TCP клент не запущен.");

            _log.Debug($"Датаграмма {datagram} добавлена в очередь для отправки");
            _queue.Enqueue(datagram, token, tcs);
        }

        /// <summary>
        /// Поток ожидания данных в подключении
        /// </summary>
        /// <param name="connection">Подключение</param>
        private void DataReceiving()
        {
            while (!_cts.IsCancellationRequested)
            {
                // подключение нарушено, пересоздаем
                lock (_lock)
                {
                    if (_connection.IsBroken)
                    {
                        if (!TryResetConnection())
                        {
                            TaskHelper.Delay(500).Wait();
                            continue;
                        }
                    }
                }

                try
                {
                    //поток чтения датаграм
                    var datagram = _connection.ReadDatagram(_cts.Token);
                    _log.Debug($"Получена датаграмма {datagram}");

                    _handler?.BeginInvoke(datagram, x => {
                        _log.Debug($"Датаграммы {datagram} обработана");
                    }, null);
                }
                catch (Exception ex)
                {
                    if (!(ex.InnerException is TaskCanceledException) &&
                        !(ex.InnerException is OperationCanceledException))
                    {
                        _log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    }
                }
            }
        }

        /// <summary>
        /// Поток отправки данных в сервер
        /// </summary>
        /// <param name="connection">Подключение</param>
        private void DataSend()
        {
            while (!_cts.IsCancellationRequested)
            {
                _queue.WaitDataAsync(_cts.Token);
                if (!_queue.TryDequeue(out var data))
                {
                    continue;
                }

                // подключение нарушено, пересоздаем
                lock (_lock)
                {
                    if (_connection.IsBroken)
                    {
                        if (!TryResetConnection())
                        {
                            TaskHelper.Delay(500).Wait();
                            continue;
                        }
                    }
                }

                _log.Debug($"Отправка датаграммы {data.Datagram}");

                try
                {
                    //запись датаграммы
                    _connection.WriteDatagram(data.Datagram, data.Token, out _, out var size);
                    _log.Debug($"Датаграмма {data.Datagram} отправлена{Environment.NewLine}size:{size}{Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    if (data?.TCS != null)
                        data.TCS.SetException(ex);

                    if (!(ex.InnerException is TaskCanceledException) &&
                        !(ex.InnerException is OperationCanceledException))
                    {
                        _log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    }
                }
            }
        }

        /// <summary>
        /// Создать новое подключени
        /// </summary>
        /// <returns>Подключение DatagramStream</returns>
        private DatagramStream Connect()
        {
            var client = new TcpClient(_serverName, _serverPort);
            var stream = new NetworkStream(client.Client, true);

            _log.Info("Клиент подкючен к серверу.");
            return new DatagramStream(stream, nameof(TcpMetalClient));
        }

        public void Stop()
        {
            if(!_isStarted)
            {
                _log.Warn("TCP клиент уже остановлен" + Environment.NewLine + Environment.StackTrace);
                return;
            }

            _cts.Cancel();

            Task.WaitAll(_receive, _send);
            
            _connection.Close();
        }

        /// <summary>
        /// Применить handler обработки, все принятые данне будут отправляться в него
        /// </summary>
        /// <param name="handler"></param>
        public void SetDatagramHandler(Func<DatagramBase, bool> handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Пересоздание конкретного подключения при его падении
        /// </summary>
        /// <param name="id">Идентификатор подключения</param>
        /// <returns>Новое подключение DatagramStream</returns>
        private bool TryResetConnection()
        {
            _resetLock.WaitOne();

            try
            {
                if (_connection.IsBroken)
                {
                    _connection.Close();
                    _log.Info($"Попытка восстановить подключение к {_serverName}:{_serverPort}");
                    _connection = Connect();
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Warn($"Не удалось восстановить подключение к {_serverName}:{_serverPort} {Environment.NewLine} {ex.Message}" + Environment.NewLine + Environment.StackTrace);
                return false;
            }
            finally
            {
                _resetLock.Set();
            }
        }
    }
}
