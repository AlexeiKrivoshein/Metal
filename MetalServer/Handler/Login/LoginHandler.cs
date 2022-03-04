using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.Datagram.Security;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public class LoginHandler
        : BaseHandler<GetLoginElement, SecurityContext>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.UserLogin };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Пользователь не авторизован";

        protected override SecurityContext InnerHandle(GetLoginElement data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var employee = Manager.GetElement<Employee>(data.Id) as EmployeeDTO;

            if (employee is null)
                throw new Exception($"Не найден пользователь c идентификатором [{data.Id}].");

            if (employee.Password != data.Password)
                return new SecurityContext(Guid.Empty, new byte[0]);

            var shortName = Funcs.GetShortName(employee.Secondname, employee.Name, employee.Patronymic);

            if (!employee.UserGroupId.HasValue)
                throw new Exception($"Для пользователя [{shortName}] не назначена роль.");

            var group = Manager.GetElement<UserGroup>(employee.UserGroupId.Value) as UserGroupDTO;

            if (group is null)
                throw new Exception($"Не найдена роль для пользователя [{shortName}].");

            return new SecurityContext(data.Id, group.Rights);
        }
    }
}
