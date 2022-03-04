using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.ModelEx
{
    [Serializable]
    public class OrderHandledDTO
        : HandledDTO
    {
        public int OrderNumber { get; set; }

        protected override bool InnerEquals(BaseDTO obj)
        {
            if (!(obj is OrderHandledDTO other))
                return false;

            return  Type == other.Type &&
                    OrderNumber == other.OrderNumber;
        }
    }
}
