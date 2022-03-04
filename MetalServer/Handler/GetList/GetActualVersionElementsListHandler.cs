using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.Datagram.GetListData;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalServer.Handler.GetList
{
    [Handler]
    public sealed class GetActualVersionElementsListHandler
        : BaseHandler<GetActualVersionElementsList, SetListData<BaseDTO>>
    {
        private List<DatagramType> _types = new List<DatagramType>
        {
            DatagramType.GetOrdersActual
        };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось обновить список элементов";

        protected override SetListData<BaseDTO> InnerHandle(GetActualVersionElementsList data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            int count;
            List<BaseDTO> elements;

            switch (type)
            {
                case DatagramType.GetOrdersActual:
                    elements = Manager.GetModelElements<Order>(data, out count);
                    break;
                default:
                    throw new Exception($"{UNKNOWN_DATAGRAM_TYPE} {type}");
            }

            return new SetListData<BaseDTO>(elements, count, -1);
        }
    }
}
