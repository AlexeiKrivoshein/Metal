using System;

namespace MetalServer.Handler
{
    [AttributeUsage(AttributeTargets.Class,
                   AllowMultiple = false,
                   Inherited = true)]
    public class HandlerAttribute: Attribute
    { }
}
