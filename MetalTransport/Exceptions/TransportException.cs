using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.Exceptions
{
    public class TransportException : Exception
    {
        public TransportException(string message):
            base(message)
        {
        }

        public TransportException(string message, Exception exception) :
            base(message, exception)
        {
        }
    }
}
