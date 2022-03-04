using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MetalServer.Handler
{
    [Handler]
    public class GetOrderIdFiltredElementsHandler
        : BaseHandler<GetOrderIdFilteredElementsList, SetListData<BaseDTO>>
    {
        private List<DatagramType> _types = new List<DatagramType>
        {
            DatagramType.GetOrderOperationList,
            DatagramType.GetLimitMaterialList,
            DatagramType.GetLimitOperationList,
            DatagramType.GetFileList
        };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось получить список элементов заказа";

        protected override SetListData<BaseDTO> InnerHandle(GetOrderIdFilteredElementsList data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            int count;
            List<BaseDTO> elements;
            switch (type)
            {
                case DatagramType.GetOrderOperationList:
                    elements = Manager.GetModelElements<OrderOperation>(data,
                                                                        q => q.OrderBy(x => x.Index),
                                                                        out count);
                    break;
                case DatagramType.GetLimitMaterialList:
                    elements = Manager.GetModelElements<LimitCardMaterial>(data,
                                                                           q => q.OrderBy(x => x.Index),
                                                                           null,
                                                                           q => q.Include(x => x.FactMaterials),
                                                                           out count);
                    break;
                case DatagramType.GetLimitOperationList:
                    elements = Manager.GetModelElements<LimitCardOperation>(data,
                                                                            q => q.OrderBy(x => x.Index),
                                                                            out count);
                    break;
                case DatagramType.GetFileList:
                    elements = Manager.GetModelElements<MetalFile>(data,
                                                                   q => q.OrderBy(x => x.Name),
                                                                   out count);
                    break;
                default:
                    throw new Exception($"{UNKNOWN_DATAGRAM_TYPE} {type}");
            }

            return new SetListData<BaseDTO>(elements, count);
        }
    }
}
