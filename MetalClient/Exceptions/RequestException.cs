using System;


namespace MetalClient.Exceptions
{
    internal class RequestException 
        : Exception
    {
        public RequestException(string message)
            : base(message)
        {
        }
    }
}
