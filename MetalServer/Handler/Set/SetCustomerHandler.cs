using MetalDAL.Mapper;
using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class SetCustomerHandler
        : BaseHandler<CustomerDTO, HandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.SetCustomerElement };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось сохранить заказчика";

        protected override HandledDTO InnerHandle(CustomerDTO element, DatagramType type)
        {
            return Manager.SetElement(MapperContainer.Instance.Map<Customer>(element));
        }
    }
}
