using MetalTransport.ModelEx;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;

namespace MetalDAL.Model
{
    public partial class Customer
        : BaseElement<Customer, CustomerDTO, VersionListItemDTO>
        , IVersionModelElement<Customer>
    {
    }
}
