using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.IO;

namespace MetalServer.Handler
{
    [Handler]
    public class RemoveFileHandler
        : BaseHandler<RemElementData, HandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.RemFile };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось удалить файл";

        protected override HandledDTO InnerHandle(RemElementData data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var element = Manager.GetElement<MetalFile>(data.Id) as MetalFileDTO;
                
            if (File.Exists(element.Path))
                File.Delete(element.Path);

            Manager.RemovePartOfOrderElement<MetalFile>(data.Id);

            return HandledDTO.Success(data.Id);
        }
    }
}
