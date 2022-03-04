using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MetalTransport.ModelEx.Validation
{
    public class ValidationMapTuple
    {
        public PropertyInfo Property { get; set; }

        public List<ValidationBaseAttribute> ValidationAttributes { get; set; }
    }
}
