using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalTransport.Handler
{
    public enum HadlerResult
    {
        Succes,
        Break,
        Timeout,
        Error,
        NoHandler
    }
}
