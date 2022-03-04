using MetalDAL.Mapper;
using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class SetLimitMaterialHandler
        : BaseHandler<LimitCardMaterialDTO, HandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.SetLimitMaterialElement };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось сохранить материал лимитной карточки";

        protected override HandledDTO InnerHandle(LimitCardMaterialDTO data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            return Manager.SetElement(MapperContainer.Instance.Map<LimitCardMaterial>(data));
        }
    }
}
