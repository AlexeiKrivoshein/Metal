using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;
using MetalTransport.ModelEx;

namespace MetalDAL.Model
{
    public partial class MetalFile
        : PartOfOrderBaseElement<MetalFile, MetalFileDTO, BaseListItemDTO>
        , IPartOfOrderModelElement<MetalFile>
    {
    }
}
