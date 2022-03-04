using MetalTransport.ModelEx;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;

namespace MetalDAL.Model
{
    public partial class Material
        : BaseElement<Material, MaterialDTO, VersionListItemDTO>
        , IVersionModelElement<Material>
    {
    }
}
