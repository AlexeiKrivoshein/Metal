using MetalCore.ModelEx;
using MetalDAL.Manager;
using MetalDAL.ModelEx.ElementEx;
using MetalTransport.ModelEx;
using System;

namespace MetalDAL.Model
{
    public partial class LimitCardFactMaterial
        : BaseElement<LimitCardFactMaterial, LimitCardFactMaterialDTO, LimitCardFactMaterialDTO>
        , IModelElement<LimitCardFactMaterial>
    {
        public override void LoadOuther(ModelManager manager)
        {
            using (var context = new MetalEDMContainer())
            {
                ///материал лимитной карты
                if (LimitCardMaterialId != Guid.Empty)
                {
                    LimitCardMaterial = context.LimitCardMaterialSet.Find(LimitCardMaterialId);
                }

                ///материал из справочника
                if (MaterialId != Guid.Empty)
                {
                    Material = context.MaterialSet.Find(MaterialId);
                }
            }
        }
    }
}
