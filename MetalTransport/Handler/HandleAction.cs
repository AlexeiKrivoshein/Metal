using MetalTransport.Datagram;
using System.Threading;

namespace MetalTransport.Handler
{
    public delegate HadlerResult HandleAction(DatagramBase datagram, CancellationToken token);
}
