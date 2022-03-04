using MetalDAL.Manager;
using MetalDAL.Model;
using MetalTransport.ModelEx;
using System;

namespace MetalDAL.ModelEx.ElementEx
{
    public abstract class PartOfOrderBaseElement<T, TDTO, TListDTO>
        : BaseElement<T, TDTO, TListDTO>
        where T : class, IPartOfOrderModelElement<T>, new()
        where TDTO : BaseDTO
        where TListDTO : BaseDTO
    {
        public override void LoadNested(MetalEDMContainer context, ModelManager manager)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (_typed.OrderId == Guid.Empty)
                throw new Exception("Для элемента-части заказа не определен идентификатор заказа");

            var order = context.OrderSet.Find(_typed.OrderId);
            _typed.Order = order;
        }
    }
}
