using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class GetElementHandler
        : BaseHandler<GetElementData, BaseDTO>
    {
        private List<DatagramType> _types = new List<DatagramType>
        {
            DatagramType.GetOrderElement,
            DatagramType.GetCustomerElement,
            DatagramType.GetEmployeeElement,
            DatagramType.GetPostElement,
            DatagramType.GetOperationElement,
            DatagramType.GetMaterialElement,
            DatagramType.GetOrderGroupElement,
            DatagramType.GetOrderOperationElement,
            DatagramType.GetUserGroupElement
        };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось получить элемент";

        protected override BaseDTO InnerHandle(GetElementData data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            switch (type)
            {
                case DatagramType.GetOrderElement:
                    return Manager.GetElement<Order>(data.Id);
                case DatagramType.GetCustomerElement:
                    return Manager.GetElement<Customer>(data.Id);
                case DatagramType.GetEmployeeElement:
                    return Manager.GetElement<Employee>(data.Id);
                case DatagramType.GetPostElement:
                    return Manager.GetElement<Post>(data.Id);
                case DatagramType.GetOperationElement:
                    return Manager.GetElement<Operation>(data.Id);
                case DatagramType.GetMaterialElement:
                    return Manager.GetElement<Material>(data.Id);
                case DatagramType.GetOrderGroupElement:
                    return Manager.GetElement<OrderGroup>(data.Id);
                case DatagramType.GetOrderOperationElement:
                    return Manager.GetElement<OrderOperation>(data.Id);
                case DatagramType.GetUserGroupElement:
                    return Manager.GetElement<UserGroup>(data.Id);
                default:
                    throw new Exception($"{UNKNOWN_DATAGRAM_TYPE} {type}");
            }
        }
    }
}
