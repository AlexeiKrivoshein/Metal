using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MetalServer.Server
{
    public sealed class ConnectionBroker
    {
        private const int SERVER_START_TIMEOUT = 10_000;
        private ConcurrentBag<IServer> _servers = new ConcurrentBag<IServer>();
        private CancellationTokenSource _tokenSource;

        public ConnectionBroker()
        {
            _tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            Task.Run(() =>
            {
                while (!_tokenSource.Token.IsCancellationRequested)
                {
                    foreach (var server in _servers)
                    {
                        if (!server.IsWork)
                        {
                            server.Start();
                        }
                    }

                    Task.Delay(SERVER_START_TIMEOUT).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            );
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            while (_servers.TryTake(out var server))
            {
                server.Stop();
            }
            _tokenSource.Dispose();
        }

        public void AddServer(IServer server)
        {
            _servers.Add(server);
        }
    }
}
