using MetalDAL.Mapper;
using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class SetOrderOperationHandler
        : BaseHandler<OrderOperationDTO, HandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.SetOrderOperationElement };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось сохранить операцию заказа";

        protected override HandledDTO InnerHandle(OrderOperationDTO data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            return Manager.SetElement(MapperContainer.Instance.Map<OrderOperation>(data));
        }
    }
}
