using MetalDAL.Manager;
using MetalDAL.Model;
using MetalTransport.ModelEx;
using System;

namespace MetalDAL.ModelEx.ElementEx
{
    /// <summary>
    /// Класс элемента вложенного в заказ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDTO"></typeparam>
    /// <typeparam name="TListDTO"></typeparam>
    public abstract class PartOfOrderBaseElement<T, TDTO, TListDTO>
        : BaseElement<T, TDTO, TListDTO>
        where T : class, IPartOfOrderModelElement<T>, new()
        where TDTO : BaseDTO
        where TListDTO : BaseDTO
    {
        public override void LoadOuther(ModelManager manager)
        {
            using (var context = new MetalEDMContainer())
            {
                if (_typed.OrderId == Guid.Empty)
                    throw new Exception("Для элемента-части заказа не определен идентификатор заказа");

                ///Заказ
                var order = context.OrderSet.Find(_typed.OrderId);
                _typed.Order = order;
            }
        }
    }
}
