using MetalTransport.ModelEx;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;

namespace MetalDAL.Model
{
    public partial class OrderOperation
        : PartOfOrderBaseElement<OrderOperation, OrderOperationDTO, OrderOperationDTO>
        , IPartOfOrderModelElement<OrderOperation>
    {
    }
}
