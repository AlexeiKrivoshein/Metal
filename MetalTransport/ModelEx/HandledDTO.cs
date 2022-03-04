using MetalTransport.ModelEx.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public class HandledDTO 
        : BaseDTO
    {
        public HandledType Type { get; set; }

        public Exception Exception { get; set; }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is HandledDTO other))
                return false;

            return Type == other.Type;
        }

        public static implicit operator bool(HandledDTO handled) => handled.Type == HandledType.OK;

        public static HandledDTO Success(Guid id)
        {
            return new HandledDTO()
            {
                Id = id,
                Type = HandledType.OK
            };
        }

        public static new HandledDTO Error(Guid id, Exception exception)
        {
            return new HandledDTO()
            {
                Id = id,
                Type = HandledType.Error,
                Exception = exception
            };
        }
    }
}
