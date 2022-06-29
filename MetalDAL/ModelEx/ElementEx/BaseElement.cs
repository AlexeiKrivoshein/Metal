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
        
        //загрузка вложенных объектов/коллекций
        public virtual void LoadOuther(ModelManager manager) { }

        //удаление не справчных вложенных объектов/коллекций(справчное значение может использоваться в другой сущности)
        public virtual void RemoveInner(ModelManager manager, bool permanent) { }

        //сохранение вложенных объектов/коллекций
        public virtual void SaveInner(ModelManager manager) { }
    }
}
