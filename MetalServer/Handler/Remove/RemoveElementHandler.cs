using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class RemoveElementHandler
        : BaseHandler<RemElementData, HandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType>
        {
            DatagramType.RemOrderElement,
            DatagramType.RemCustomerElement,
            DatagramType.RemEmployeeElement,
            DatagramType.RemPostElement,
            DatagramType.RemOperationElement,
            DatagramType.RemMaterialElement,
            DatagramType.RemOrderGroupElement,
            DatagramType.RemOrderOperationElement,
            DatagramType.RemLimitMaterialElement,
            DatagramType.RemLimitOperationElement,
            DatagramType.RemUserGroupElement
        };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось удалить элемент";

        protected override HandledDTO InnerHandle(RemElementData data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var id = data.Id;
            var permanent = data.Permanent;

            switch (type)
            {
                case DatagramType.RemOrderElement:
                    return Manager.RemoveVersioningElement<Order>(data.Id, permanent);
                case DatagramType.RemCustomerElement:
                    return Manager.RemoveVersioningElement<Customer>(data.Id, permanent);
                case DatagramType.RemEmployeeElement:
                    return Manager.RemoveVersioningElement<Employee>(data.Id, permanent);
                case DatagramType.RemPostElement:
                    return Manager.RemoveVersioningElement<Post>(data.Id, permanent);
                case DatagramType.RemOperationElement:
                    return Manager.RemoveVersioningElement<Operation>(data.Id, permanent);
                case DatagramType.RemMaterialElement:
                    return Manager.RemoveVersioningElement<Material>(data.Id, permanent);
                case DatagramType.RemOrderGroupElement:
                    return Manager.RemoveVersioningElement<OrderGroup>(data.Id, permanent);
                case DatagramType.RemOrderOperationElement:
                    return Manager.RemovePartOfOrderElement<OrderOperation>(data.Id);
                case DatagramType.RemLimitMaterialElement:
                    return Manager.RemovePartOfOrderElement<LimitCardMaterial>(data.Id);
                case DatagramType.RemLimitOperationElement:
                    return Manager.RemovePartOfOrderElement<LimitCardOperation>(data.Id);
                case DatagramType.RemUserGroupElement:
                    return Manager.RemoveVersioningElement<UserGroup>(data.Id, permanent);
                default:
                    throw new Exception($"{UNKNOWN_DATAGRAM_TYPE} {type}");
            }
        }
    }
}
