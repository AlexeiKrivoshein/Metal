using System.Threading;

namespace MetalTransport.Datagram
{
    public interface IDatagramStream
    {
        bool IsBroken { get; }

        DatagramBase ReadDatagram(CancellationToken token);

        void WriteDatagram(DatagramBase datagram, CancellationToken token, out string content, out int size);

        void Close();
    }
}