using log4net;
using MetalDiagnostic.Logger;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.Client
{
    /// <summary>
    /// Исполняет запросы через клиент IClient, содержит словарь ожиданий ответов
    /// </summary>
    public sealed class ClientRequestExecutorHandler
    {
        private ConcurrentDictionary<Guid, TaskCompletionSource<byte[]>> _contexts = new ConcurrentDictionary<Guid, TaskCompletionSource<byte[]>>();
        private IClient _client;
        private ILog _log = LogService.GetLogger(nameof(ClientRequestExecutorHandler));

        public ClientRequestExecutorHandler(IClient client)
        {
            _client = client;
            _client?.SetDatagramHandler(HandleMessage);
        }

        public Task<T> ExcuteRequestAsync<T>(DatagramBase request, CancellationToken token) 
            where T: BaseDTO
        {
            if (request.CorelationId == Guid.Empty) request.CorelationId = Guid.NewGuid();

            var tcs = new TaskCompletionSource<byte[]>();
            _contexts.TryAdd(request.CorelationId, tcs);//проверить отмену отправки, возможно зависание отмененных отправко в списке запросов

            _client.Enqueue(request, token, tcs);

            return tcs.Task.ContinueWith(t => {
                if (! TaskHelper.CheckError(t, out var eror))
                {
                    throw new InvalidOperationException(eror);
                }

                var datagram = SerializationHelper.Deserialize<T>(t.Result, out _);
                _log.Debug($"Получена датаграмма {datagram}{Environment.NewLine}");

                return datagram;
            }, token);
        }

        private bool HandleMessage(DatagramBase datagram)
        {
            if (_contexts.TryGetValue(datagram.CorelationId, out var context))
            {
                if (datagram == null)
                {
                    _log.Error($"Получена пустая датаграмма{Environment.NewLine}{Environment.StackTrace}");
                    context.SetException(new Exception($"Получена пустая датаграмма"));

                    return false;
                }
                else if (datagram.DataType == DatagramType.Error)
                {
                    var error = SerializationHelper.Deserialize<HandledDTO>(datagram.Data, out var content);
                    _log.Error($"{error.Exception}{Environment.NewLine}{error.Exception?.StackTrace ?? ""}");
                    context.SetException(error.Exception);

                    return false;
                }
                else
                {
                    context.SetResult(datagram.Data);

                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            _client.SetDatagramHandler(null);
        }
    }
}
