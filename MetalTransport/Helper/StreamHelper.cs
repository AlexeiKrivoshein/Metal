using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace MetalTransport.Helper
{
    public static class StreamHelper
    {
        public static Task<int> ReadAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            var task = 
                Task<int>.Factory.FromAsync(
                    stream.BeginRead,
                    stream.EndRead,
                    buffer,
                    offset,
                    count,
                    null
                    );

            return task;
        }

        public static Task WriteAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            var task = Task.Factory.FromAsync(
                stream.BeginWrite,
                stream.EndWrite,
                buffer,
                offset,
                count,
                null
                );

            return task;
        }
    }
}
