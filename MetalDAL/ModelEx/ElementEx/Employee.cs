using MetalTransport.ModelEx;
using System;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;
using MetalTransport.Helper;
using MetalDAL.Manager;

namespace MetalDAL.Model
{
    public partial class Employee
        : BaseElement<Employee, EmployeeDTO, VersionListItemDTO>
        , IVersionModelElement<Employee>
    {
        public string ShortName => Funcs.GetShortName(Secondname, Name, Patronymic);
        public string FullName => Funcs.GetFullName(Secondname, Name, Patronymic);

        public override void LoadNested(MetalEDMContainer context, ModelManager manager)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (UserGroupId.HasValue && UserGroupId != Guid.Empty)
            {
                UserGroup = context.UserGroupSet.Find(UserGroupId);
            }
            else
            {
                UserGroup = null;
                UserGroupId = null;
            }

            if (PostId.HasValue && PostId != Guid.Empty)
            {
                Post = context.PostSet.Find(PostId);
            }
            else
            {
                Post = null;
                PostId = null;
            }
        }
    }
}
