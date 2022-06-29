using MetalDAL.Manager;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;
using MetalTransport.ModelEx;
using System;
using System.IO;
using System.Linq;

namespace MetalDAL.Model
{
    public partial class Order
        : BaseElement<Order, OrderDTO, OrderListItemDTO>
        , IVersionModelElement<Order>
    {
        public PlanItemDTO ToPlanDTO()
        {
            return OrderDTO.ToPlanDTO(ToDTO());
        }

        public override void LoadOuther(ModelManager manager)
        {
            using (var context = new MetalEDMContainer())
            {
                //контрагент
                Customer = context.CustomerSet.Find(CustomerId);
                CustomerId = Customer.Id;

                //группа заказа
                if (OrderGroupId.HasValue && OrderGroupId != Guid.Empty)
                {
                    OrderGroup = context.OrderGroupSet.Find(OrderGroupId);
                    OrderGroupId = OrderGroup.Id;
                }
                else
                {
                    OrderGroup = null;
                    OrderGroupId = null;
                }
            }
        }

        public override void RemoveInner(ModelManager manager, bool permanent)
        {
            if (manager is null)
                throw new ArgumentNullException(nameof(manager));

            //чертежи
            OrderDrawings.ToList().ForEach(drawing =>
            {
                manager.RemovePartOfOrderElement<MetalFile>(drawing.Id);

                if (File.Exists(drawing.Path))
                    File.Delete(drawing.Path);
            });

            //работы
            OrderWork.ToList().ForEach(work =>
            {
                manager.RemovePartOfOrderElement<OrderOperation>(work.Id);
            });

            //материалы лимитки
            LimitCard.ToList().ForEach(material =>
            {
                manager.RemovePartOfOrderElement<LimitCardMaterial>(material.Id);
            });

            //работы лимитки
            LimitCardWork.ToList().ForEach(work =>
            {
                manager.RemovePartOfOrderElement<LimitCardOperation>(work.Id);
            });
        }
    }
}
