using MetalCore.ModelEx;
using MetalDAL.Helpers;
using MetalDAL.Manager;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetalDAL.Model
{
    public partial class LimitCardMaterial
        : PartOfOrderBaseElement<LimitCardMaterial, LimitCardMaterialDTO, LimitCardMaterialDTO>
        , IPartOfOrderModelElement<LimitCardMaterial>
    {
        public override void CopyFrom(IModelElement<LimitCardMaterial> other)
        {
            if (!(other is LimitCardMaterial typed))
                throw new ArgumentNullException(nameof(other));

            Id = typed.Id;
            OrderId = typed.OrderId;
            MaterialId = typed.MaterialId;
            Units = typed.Units;
            UsagePerUnits = typed.UsagePerUnits;
            UsagePerOrder = typed.UsagePerOrder;
            Multiplicity = typed.Multiplicity;
            Price = typed.Price;
            Index = typed.Index;

            //удаление фактических материалов
            var removedIds = FactMaterials.Select(fm => fm.Id).Except(typed.FactMaterials.Select(fm => fm.Id)).ToList();
            foreach (var materialId in removedIds)
            {
                FactMaterials.Remove(FactMaterials.First(fm => fm.Id.Equals(materialId)));
            }

            //добавление/изменение фактическоих материалов
            foreach (var factMaterial in typed.FactMaterials)
            {
                var material = FactMaterials.FirstOrDefault(m => m.Id == factMaterial.Id);

                if (material != null)
                {
                    material.MaterialId = factMaterial.MaterialId;
                    material.Material = factMaterial.Material;

                    material.Count = factMaterial.Count;
                    material.Price = factMaterial.Price;
                }
                else
                {
                    factMaterial.LimitCardMaterial = this;
                    factMaterial.LimitCardMaterialId = Id;

                    FactMaterials.Add(factMaterial);
                }
            }
        }

        public override void RemoveInner(ModelManager manager, bool permanent)
        {
            if (manager is null)
                throw new ArgumentNullException(nameof(manager));

            using (var context = new MetalEDMContainer())
            {
                context.LimitCardFactMaterialSet.RemoveRange(FactMaterials);
            }
        }

        public override void SaveInner(ModelManager manager)
        {
            if (manager is null)
                throw new ArgumentNullException(nameof(manager));

            //получение идентификаторов удаленных фактических материалов
            List<Guid> removedIds = null;
            using (var context = new MetalEDMContainer())
            {
                var stored = context.LimitCardMaterialSet.Find(Id);
                removedIds = stored.FactMaterials.Select(material => material.Id)
                            .Except(FactMaterials.Select(material => material.Id)).ToList();
            }

            //удаление фактических материалов
            if (removedIds != null)
            {
                foreach (var id in removedIds)
                {
                    manager.RemoveElement<LimitCardFactMaterial>(id);
                }
            }

            //добавление/обновление фактических материалов
            if (FactMaterials.Any())
            {
                foreach (var material in FactMaterials)
                {
                    manager.InnerSetElement(material);
                }
            }
        }

        public override void LoadOuther(ModelManager manager)
        {
            base.LoadOuther(manager);

            using (var context = new MetalEDMContainer())
            {
                ///материал из справочника
                if (MaterialId != Guid.Empty)
                {
                    Material = context.MaterialSet.Find(MaterialId);
                }
            }
        }
    }
}
