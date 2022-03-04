using log4net;
using MetalDiagnostic.Logger;
using MetalServer.Handler;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.Queue;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MetalServer.Server
{
    public sealed class TcpMetalServer
        : IServer
    {
        private ILog _log = LogService.GetLogger(nameof(TcpMetalServer));

        public string ServerName { get; private set; } = "127.0.0.1";
        public string ServerPort { get; private set; } = "9500";

        private CancellationTokenSource _tcs;
        public bool IsWork { get; private set; } = false;

        private ConcurrentBag<ClientWorker> _workers = new ConcurrentBag<ClientWorker>();

        //карта обработчиков датаграм
        private ConcurrentDictionary<DatagramType, IDatagramHandler> _datagramHandlerMap = new ConcurrentDictionary<DatagramType, IDatagramHandler>();

        private TcpListener _server;
        private Task _wait;

        public TcpMetalServer()
        {
            ServerName = System.Configuration.ConfigurationManager.AppSettings["ServerName"];
            ServerPort = System.Configuration.ConfigurationManager.AppSettings["Port"];
        }

        public void Start()
        {
            if (IsWork)
            {
                _log.Warn("Сервер уже запущен." + Environment.NewLine + Environment.StackTrace);
                return;
            }

            _tcs = new CancellationTokenSource();
            IsWork = true;

            IPAddress localAddr = IPAddress.Parse(ServerName);

            _server = new TcpListener(localAddr, int.Parse(ServerPort));
            _server.Start();

            _wait = Task.Run(() => WaitClient());
        }

        public void Stop()
        {
            if (!IsWork)
            {
                _log.Warn("Сервер уже остановлен." + Environment.NewLine + Environment.StackTrace);
                return;
            }

            IsWork = false;
            _tcs?.Cancel();

            foreach (var worker in _workers)
            {
                worker.Stop();
            }

            if (!_wait.Wait(10_000))
                _log.Warn($"Не удалось остановить поток ожидания клиентов." + Environment.NewLine + Environment.StackTrace);

            _tcs.Dispose();
            _server.Stop();
        }

        public void AddDatagramHandler(IDatagramHandler handler)
        {
            if (handler is null) throw new ArgumentNullException(nameof(handler));

            foreach (var type in handler.DatagramTypes)
            {
                _datagramHandlerMap.AddOrUpdate(type, handler, (k, v) => handler);
            }
        }

        private async void WaitClient()
        {
            while (!_tcs.IsCancellationRequested)
            {
                try
                {
                    var wait = _server.AcceptTcpClientAsync();
                    wait.Wait(_tcs.Token);

                    if (_tcs.IsCancellationRequested)
                        break;

                    var client = wait.Result;

                    if (client != null)
                    {
                        var clientId = Guid.NewGuid();

                        _log.Info($"Подключен клиент {clientId}.");

                        //очередь на отправку датаграм данному клиенту
                        var sendQueue = new DatagramQueue();
                        //очередь на боработку датаграм полученных от данного клиента
                        var receiveQueue = new DatagramQueue();

                        var worker = new ClientWorker(clientId, client, sendQueue, receiveQueue, _datagramHandlerMap);
                        worker.Start();
                        _workers.Add(worker);
                    }
                    else
                    {
                        await Task.Delay(100).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Во время ожидания клиента прозошло исключений {Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }
    }
}
