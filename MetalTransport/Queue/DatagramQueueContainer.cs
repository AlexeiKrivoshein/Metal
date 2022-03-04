using MetalTransport.Datagram;
using System.Threading;
using System.Threading.Tasks;

namespace MetalTransport.Queue
{
    public class DatagramQueueContainer
    {
        public DatagramBase Datagram { get; }

        public CancellationToken Token { get; }

        public TaskCompletionSource<byte[]> TCS { get; }

        public DatagramQueueContainer(DatagramBase datagram, CancellationToken token, TaskCompletionSource<byte[]> tcs)
        {
            Datagram = datagram;
            Token = token;
            TCS = tcs;
        }
    }
}
