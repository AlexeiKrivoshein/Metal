using MetalCore.ModelEx;
using MetalDAL.Manager;
using MetalDAL.ModelEx;
using MetalDAL.ModelEx.ElementEx;
using MetalTransport.ModelEx;
using System;
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

        public override void RemoveNested(MetalEDMContainer context, ModelManager manager, bool permanent)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (manager is null)
                throw new ArgumentNullException(nameof(manager));

            context.LimitCardFactMaterialSet.RemoveRange(FactMaterials);
        }
    }
}
