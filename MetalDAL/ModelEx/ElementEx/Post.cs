using MetalTransport.ModelEx;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;

namespace MetalDAL.Model
{
    public partial class Post
        : BaseElement<Post, PostDTO, VersionListItemDTO>
        , IVersionModelElement<Post>
    {
    }
}
