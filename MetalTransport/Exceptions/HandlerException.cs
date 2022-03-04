using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.Exceptions
{
    public class HandlerException
        : Exception
    {
        public HandlerException(string message)
            : base(message)
        {
        }

        public HandlerException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
