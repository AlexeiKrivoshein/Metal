using MetalDAL.Mapper;
using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class SetEmployeeHandler
        : BaseHandler<EmployeeDTO, HandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType>
        {
            DatagramType.SetEmployeeElement 
        };

        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось сохранить сотрудника";

        protected override HandledDTO InnerHandle(EmployeeDTO data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            return Manager.SetElement(MapperContainer.Instance.Map<Employee>(data));
        }
    }
}
