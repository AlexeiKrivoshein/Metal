using MetalTransport.ModelEx;
using System;

namespace MetalTransport.Datagram
{
    [Serializable]
    public sealed class RemElementData
        :BaseDTO
    {
        public bool Permanent { get; set; } = false;

        public RemElementData() { }

        public RemElementData(Guid id, bool permanent = false)
        {
            Id = id;
            Permanent = permanent;
        }
    }
}
