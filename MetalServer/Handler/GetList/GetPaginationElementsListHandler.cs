using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler.GetList
{
    [Handler]
    public sealed class GetPaginationElementsListHandler
        : BaseHandler<GetPaginationElementsList, SetListData<BaseDTO>>
    {
        private List<DatagramType> _types = new List<DatagramType>
        {
            DatagramType.GetOrdersNext
        };

        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось получить список заказов";

        protected override SetListData<BaseDTO> InnerHandle(GetPaginationElementsList data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var elements = Manager.GetModelElements<Order>(data, out var count);

            return new SetListData<BaseDTO>(elements, count, 0);
        }
    }
}
