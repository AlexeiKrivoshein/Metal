using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport
{
    public interface IStartable
    {
        void Start();

        void Stop();
    }
}
