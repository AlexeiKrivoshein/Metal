using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.ModelEx.Validation
{
    public class RequiredAttribute : ValidationBaseAttribute
    {
        public RequiredAttribute(string error)
            : base(error)
        {
        }
    }
}
