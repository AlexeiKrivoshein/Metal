using MetalDAL.Mapper;
using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class SetOrderGroupHandler
        : BaseHandler<OrderGroupDTO, HandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.SetOrderGroupElement };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось сохранить группу заказов";

        protected override HandledDTO InnerHandle(OrderGroupDTO data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            return Manager.SetElement(MapperContainer.Instance.Map<OrderGroup>(data));
        }
    }
}
