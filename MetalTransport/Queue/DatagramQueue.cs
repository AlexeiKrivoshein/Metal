using MetalTransport.Datagram;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MetalTransport.Queue
{
    public class DatagramQueue
    {
        private readonly ManualResetEvent _notEmpty = new ManualResetEvent(false);
        private ConcurrentQueue<DatagramQueueContainer> _queue = new ConcurrentQueue<DatagramQueueContainer>();

        public void Enqueue(DatagramBase data, CancellationToken token, TaskCompletionSource<byte[]> tcs)
        {
            _queue.Enqueue(new DatagramQueueContainer(data, token, tcs));
            _notEmpty.Set();
        }

        public void Enqueue(DatagramBase data)
        {
            _queue.Enqueue(new DatagramQueueContainer(data, CancellationToken.None, null));
            _notEmpty.Set();
        }

        public bool TryDequeue(out DatagramQueueContainer container)
        {
            if (!_queue.TryDequeue(out container))
            {
                if (_queue.Count == 0)
                    _notEmpty.Reset();

                return false;
            }
            else
            {
                return true;
            }
        }

        public bool WaitDataAsync(CancellationToken token)
        {
            return WaitHandle.WaitAny(new WaitHandle[] { _notEmpty, token.WaitHandle }) == 0;
        }
    }
}
