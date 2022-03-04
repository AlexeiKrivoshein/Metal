using System.Threading;

namespace MetalDAL.Manager
{
    internal class ModelElementVersion
    {
        private long _version = 0L;

        public ModelElementVersion(long version)
        {
            _version = version;
        }

        public long GetActual()
        {
            return _version;
        }

        public long GetNext()
        {
            return Interlocked.Increment(ref _version);
        }
    }
}
