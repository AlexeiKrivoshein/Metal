using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;
using MetalTransport.ModelEx;

namespace MetalDAL.Model
{
    public partial class UserGroup
        : BaseElement<UserGroup, UserGroupDTO, VersionListItemDTO>
        , IVersionModelElement<UserGroup>
    {
    }
}
