using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.ModelEx.Validation
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public abstract class ValidationBaseAttribute : Attribute
    {
        public string Error { get; private set;}

        public ValidationBaseAttribute(string error)
        {
            Error = error;
        }
    }
}
