using log4net;
using MetalDiagnostic.Logger;
using MetalServer.Handler;
using MetalTransport.Datagram;
using MetalTransport.Queue;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MetalServer.Server
{
    public class ClientWorker
    {
        private ILog _log = LogService.GetLogger(nameof(ClientWorker));

        public bool IsWork { get; private set; } = false;

        private Guid _clientId;
        private TcpClient _client;

        private Task _receive;
        private Task _send;
        private Task _worker;

        //очередь сообщений на отправку
        private DatagramQueue _sendQueue;
        //очередь полученных сообщений
        private DatagramQueue _receiveQueue;
        //поток датаграмм
        private DatagramStream _stream;
        //словарь обработчиков
        private ConcurrentDictionary<DatagramType, IDatagramHandler> _datagramHandlerMap;

        private CancellationTokenSource _tcs;

        public ClientWorker(Guid clientId, 
                            TcpClient client, 
                            DatagramQueue sendQueue, 
                            DatagramQueue receiveQueue,
                            ConcurrentDictionary<DatagramType, IDatagramHandler> datagramHandlerMap)
        {
            _clientId = clientId;
            _client = client;
            _sendQueue = sendQueue;
            _receiveQueue = receiveQueue;
            _datagramHandlerMap = datagramHandlerMap;

            //поток датаграм для данного клиента
            _stream = new DatagramStream(_client.GetStream(), nameof(TcpMetalServer));
        }

        public void Start()
        {
            if (IsWork)
            {
                _log.Warn($"Обработчик клиента {_clientId} уже запущен." + Environment.NewLine + Environment.StackTrace);
                return;
            }

            _tcs = new CancellationTokenSource();
            IsWork = true;

            //запуск потока приема датаграм для данного клиента
            _receive = Task.Run(() => Receive(_stream));

            //запуск потока отправки датаграм для данного клиента
            _send = Task.Run(() => Send(_stream));

            //запуск потока обработки сообщений полученных от данного клиента
            _worker = Task.Run(() => Worker());
        }

        private void Receive(IDatagramStream datagramStream)
        {
            while (!_tcs.IsCancellationRequested && !datagramStream.IsBroken)
            {
                try
                {
                    var datagram = datagramStream.ReadDatagram(_tcs.Token);
                    if (datagram != null)
                    {
                        _receiveQueue.Enqueue(datagram);
                        _log.Debug($"От клиента [{_clientId}] получена датаграмма {datagram}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        _log.Info($"Остановлен прием датаграм от клиента [{_clientId}]");
                    }
                    else
                    {
                        _log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    }
                }
            }
        }

        private void Send(IDatagramStream datagramStream)
        {
            Stopwatch sw = new Stopwatch();
            while (!_tcs.IsCancellationRequested && !datagramStream.IsBroken)
            {
                _sendQueue.WaitDataAsync(_tcs.Token);
                if (!_sendQueue.TryDequeue(out var data))
                {
                    continue;
                }

                //запись датаграммы
                try
                {
                    sw.Start();
                    datagramStream.WriteDatagram(data.Datagram, _tcs.Token, out _, out var size);
                    _log.Debug($"Датаграмма {data.Datagram} отправлена.{Environment.NewLine}размер:{size}, время:{sw.ElapsedMilliseconds}.{Environment.NewLine}");
                    sw.Reset();
                }
                catch (Exception ex)
                {
                    sw.Reset();

                    if (!(ex is OperationCanceledException))
                        _log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        private void Worker()
        {
            Stopwatch sw = new Stopwatch();
            while (!_tcs.IsCancellationRequested)
            {
                _receiveQueue.WaitDataAsync(_tcs.Token);
                if (!_receiveQueue.TryDequeue(out var container))
                {
                    continue;
                }

                var token = _tcs.Token;
                //обработка датаграммы
                try
                {
                    //по списку обработчиков находим нужный
                    if (_datagramHandlerMap.TryGetValue(container.Datagram.DataType, out var handler))
                    {
                        sw.Start();
                        //запуск обработки и отправки ответа
                        Task.Run(() => {
                            var response = handler.HandleAction(container.Datagram, token);
                            _log.Debug($"Датаграмма {container.Datagram} обработана.{Environment.NewLine}время:{sw.ElapsedMilliseconds}.{Environment.NewLine}");
                            sw.Reset();
                            _sendQueue.Enqueue(response);
                        }, token)
                        .ConfigureAwait(false);
                    }
                    else
                    {
                        _log.Error($"Не найден обработчик для сообщения {container.Datagram}");
                    }
                }
                catch (Exception ex)
                {
                    if (!(ex is OperationCanceledException))
                        _log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        public void Stop()
        {
            if (!IsWork)
            {
                _log.Warn($"Обработчик клиента {_clientId} уже остановлен." + Environment.NewLine + Environment.StackTrace);
                return;
            }

            IsWork = false;
            _tcs?.Cancel();

            if (!Task.WaitAll(new Task[] { _receive, _send, _worker }, 10_000))
                _log.Warn($"Не удалось остановить все потоки клиента {_clientId}." + Environment.NewLine + Environment.StackTrace);

            _stream.Close();

            _client.Close();
        }
    }
}
