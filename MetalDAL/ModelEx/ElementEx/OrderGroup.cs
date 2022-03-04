using MetalTransport.ModelEx;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;

namespace MetalDAL.Model
{
    public partial class OrderGroup
        : BaseElement<OrderGroup, OrderGroupDTO, VersionListItemDTO>
        , IVersionModelElement<OrderGroup>
    {
    }
}
