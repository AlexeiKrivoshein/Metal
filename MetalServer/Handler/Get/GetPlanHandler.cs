using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class GetPlanHandler
        : BaseHandler<GetPlanElementsList, SetListData<PlanItemDTO>>
    {
        private List<DatagramType> _types = new List<DatagramType> 
        { 
            DatagramType.GetPlanElement 
        };

        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось получить план";

        protected override SetListData<PlanItemDTO> InnerHandle(GetPlanElementsList data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var elements = Manager.GetPlanElements(data, out var count);

            return new SetListData<PlanItemDTO>(elements, count);
        }
    }
}
