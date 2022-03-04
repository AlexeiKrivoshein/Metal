using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class GetAllVersionElementsListHandler
        : BaseHandler<GetAllVersionElementsList, SetListData<BaseDTO>>
    {
        private List<DatagramType> _types = new List<DatagramType>
        {
            DatagramType.GetPostList,
            DatagramType.GetUsersList,
            DatagramType.GetOperationNameList,
            DatagramType.GetMaterialNameList,
            DatagramType.GetCustomerNameList,
            DatagramType.GetEmployeeNameList,
            DatagramType.GetOrderGroupNameList,
            DatagramType.GetUserGroupNameList           
        };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось получить список элементов";

        protected override SetListData<BaseDTO> InnerHandle(GetAllVersionElementsList data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            int count;
            long version;
            List<BaseDTO> elements;

            switch (type)
            {
                case DatagramType.GetPostList:
                    elements = Manager.GetModelElements<Post>(data,
                                                              q => q.OrderBy(x => x.Name),
                                                              out count,
                                                              out version);
                    break;
                case DatagramType.GetUsersList:
                    elements = Manager.GetModelElements<Employee>(data,
                                                                  q => q.OrderBy(x => x.Secondname).ThenBy(x => x.Name).ThenBy(x => x.Patronymic),
                                                                  q => q.Where(x => x.UseForLogin),
                                                                  out count,
                                                                  out version);
                    break;
                case DatagramType.GetOperationNameList:
                    elements = Manager.GetModelElements<Operation>(data,
                                                                   q => q.OrderBy(x => x.Name),
                                                                   out count,
                                                                   out version);
                    break;
                case DatagramType.GetMaterialNameList:
                    elements = Manager.GetModelElements<Material>(data,
                                                                  q => q.OrderBy(x => x.Name),
                                                                  out count,
                                                                  out version);
                    break;
                case DatagramType.GetCustomerNameList:
                    elements = Manager.GetModelElements<Customer>(data,
                                                                  q => q.OrderBy(x => x.Name),
                                                                  out count,
                                                                  out version);
                    break;
                case DatagramType.GetEmployeeNameList:
                    elements = Manager.GetModelElements<Employee>(data,
                                                                  q => q.OrderBy(x => x.Secondname),
                                                                  out count,
                                                                  out version);
                    break;
                case DatagramType.GetOrderGroupNameList:
                    elements = Manager.GetModelElements<OrderGroup>(data,
                                                                    q => q.OrderBy(x => x.Name),
                                                                    out count,
                                                                    out version);
                    break;
                case DatagramType.GetUserGroupNameList:
                    elements = Manager.GetModelElements<UserGroup>(data,
                                                                   q => q.OrderBy(x => x.Name),
                                                                   out count,
                                                                   out version);
                    break;
                default:
                    throw new Exception($"{UNKNOWN_DATAGRAM_TYPE} {type}");
            }

            return new SetListData<BaseDTO>(elements, count, version);
        }
    }
}
