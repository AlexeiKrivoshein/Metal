using MetalDAL.Mapper;
using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalServer.Handler
{
    [Handler]
    public sealed class SetPostHandler
        : BaseHandler<PostDTO, HandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.SetPostElement };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось сохранить должность";

        protected override HandledDTO InnerHandle(PostDTO data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            return Manager.SetElement(MapperContainer.Instance.Map<Post>(data));
        }
    }
}
