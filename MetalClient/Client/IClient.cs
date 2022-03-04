using MetalClient.DataManager;
using MetalTransport.Datagram;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.Client
{
    public interface IClient
    {
        void Start();

        void Stop();

        void Enqueue(DatagramBase datagram, CancellationToken token, TaskCompletionSource<byte[]> tcs);

        void SetDatagramHandler(Func<DatagramBase, bool> handler);
    }
}
