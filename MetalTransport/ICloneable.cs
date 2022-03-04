using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport
{
    public interface ICloneable<T>
    {
        T Clone();
    }
}
