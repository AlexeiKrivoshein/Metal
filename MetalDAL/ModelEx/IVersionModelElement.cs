using MetalCore.ModelEx;

namespace MetalDAL.ModelEx
{
    public interface IVersionModelElement<T>
        : IModelElement<T>
        where T: class, IModelElement<T>
    {
        long Version { get; set; }

        bool Deleted { get; set; }
    }
}
