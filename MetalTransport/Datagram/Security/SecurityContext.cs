using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.Datagram.Security
{
    [Serializable]
    public sealed class SecurityContext
        :BaseDTO
    {
        public Guid UserId { get; set; }

        public byte[] Rights { get; set; }

        public SecurityContext()
        {
            UserId = Guid.Empty;
            Rights = new byte[0];
        }

        public SecurityContext(Guid userId, byte[] rights)
        {
            UserId = userId;
            Rights = rights;
        }
    }
}
