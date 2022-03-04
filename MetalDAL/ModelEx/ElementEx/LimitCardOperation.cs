using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;
using MetalTransport.ModelEx;

namespace MetalDAL.Model
{
    public partial class LimitCardOperation
        : PartOfOrderBaseElement<LimitCardOperation, LimitCardOperationDTO, LimitCardOperationDTO>
        , IPartOfOrderModelElement<LimitCardOperation>
    {
    }
}
