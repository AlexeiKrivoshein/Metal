using MetalDAL.Manager;
using MetalDAL.Model;
using MetalTransport.ModelEx;
using System;

namespace MetalCore.ModelEx
{
    public interface IModelElement<T>
    {
        Guid Id { get; set; }

        BaseDTO ToDTO();

        BaseDTO ToListItemDTO();

        void CopyFrom(IModelElement<T> value);

        void LoadNested(MetalEDMContainer context, ModelManager manager);

        void RemoveNested(MetalEDMContainer context, ModelManager manager, bool permanent);
    }
}
