using MetalDAL.Manager;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
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

        public override void LoadNested(MetalEDMContainer context, ModelManager manager)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            Customer = context.CustomerSet.Find(CustomerId);
            CustomerId = Customer.Id;

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

        public override void RemoveNested(MetalEDMContainer context, ModelManager manager, bool permanent)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (manager is null)
                throw new ArgumentNullException(nameof(manager));

            //чертежи
            OrderDrawings.ToList().ForEach(drawing =>
            {
                manager.RemovePartOfOrderElement<MetalFile>(drawing.Id, context);

                if (File.Exists(drawing.Path))
                    File.Delete(drawing.Path);
            });

            //работы
            OrderWork.ToList().ForEach(work =>
            {
                manager.RemovePartOfOrderElement<OrderOperation>(work.Id, context);
            });

            //материалы лимитки
            LimitCard.ToList().ForEach(material =>
            {
                manager.RemovePartOfOrderElement<LimitCardMaterial>(material.Id, context);
            });

            //работы лимитки
            LimitCardWork.ToList().ForEach(work =>
            {
                manager.RemovePartOfOrderElement<LimitCardOperation>(work.Id, context);
            });
        }
    }
}
