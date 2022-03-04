using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.ModelEx.Validation
{
    public class NumberRangeAttribute : ValidationBaseAttribute
    {
        public float From { get; private set; }

        public float To { get; private set; }

        public NumberRangeAttribute(float from, float to, string error)
            : base(error)
        {
            From = from;
            To = to;
        }
    }
}
