using MetalDAL.Mapper;
using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class SetOrderHandler
        : BaseHandler<OrderDTO, OrderHandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.SetOrderElement };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось сохранить заказ";

        protected override OrderHandledDTO InnerHandle(OrderDTO data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            return Manager.SetElement(MapperContainer.Instance.Map<Order>(data));
        }
    }
}
