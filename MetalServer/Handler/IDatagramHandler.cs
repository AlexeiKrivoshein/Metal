using MetalDAL.Manager;
using MetalTransport.Datagram;
using System.Collections.Generic;
using System.Threading;

namespace MetalServer.Handler
{
    public interface IDatagramHandler
    {
        ModelManager Manager { get; set; }

        IEnumerable<DatagramType> DatagramTypes { get; }

        DatagramBase HandleAction(DatagramBase datagram, CancellationToken token);
    }
}
