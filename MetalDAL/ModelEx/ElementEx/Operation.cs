using MetalTransport.ModelEx;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;

namespace MetalDAL.Model
{
    public partial class Operation
        : BaseElement<Operation, OperationDTO, VersionListItemDTO>
        , IVersionModelElement<Operation>
    {
    }
}
