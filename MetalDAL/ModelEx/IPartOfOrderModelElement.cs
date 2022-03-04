using MetalCore.ModelEx;
using MetalDAL.Model;
using System;

namespace MetalDAL.ModelEx
{
    public interface IPartOfOrderModelElement<T>
        : IModelElement<T>
        where T: class
    {
        Guid OrderId { get; set; }

        Order Order { get; set; }
    }
}
