using MetalCore.ModelEx;
using MetalDAL.Manager;
using MetalDAL.Mapper;
using MetalDAL.Model;
using MetalTransport.ModelEx;

namespace MetalDAL.ModelEx.ElementEx
{
    public abstract class BaseElement<T, TDTO, TListDTO>
        where T : class, new()
        where TDTO : BaseDTO
        where TListDTO : BaseDTO
    {
        protected T _typed;

        public BaseElement()
        {
            _typed = this as T;
        }

        public virtual T Clone()
        {
            var clone = MapperContainer.Instance.Map<T>(_typed);
            return clone;
        }

        public virtual void CopyFrom(IModelElement<T> value)
        {
            MapperContainer.Instance.Map(value as T, _typed);
        }

        public virtual BaseDTO ToDTO()
        {
            var dto = MapperContainer.Instance.Map<TDTO>(_typed);
            return dto;
        }

        public virtual BaseDTO ToListItemDTO()
        {
            var dto = MapperContainer.Instance.Map<TListDTO>(_typed);
            return dto;
        }
        
        public virtual void LoadNested(MetalEDMContainer context, ModelManager manager) { }

        public virtual void RemoveNested(MetalEDMContainer context, ModelManager manager, bool permanent) { }
    }
}
