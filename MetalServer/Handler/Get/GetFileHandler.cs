using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.Datagram.Properties;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.IO;

namespace MetalServer.Handler
{
    [Handler]
    public class GetFileHandler
        : BaseHandler<GetFileElementData, MetalFileDTO>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.GetFileData };
        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось получить файл";

        protected override MetalFileDTO InnerHandle(GetFileElementData data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var dto =  Manager.GetElement<MetalFile>(data.Id) as MetalFileDTO;
                
            //загрузка содержимого с диска
            var index = data.FileIndex;
            using (FileStream stream = new FileStream(dto.Path, FileMode.Open))
            {
                var offset = Constants.CHUNK_SIZE * index;
                var fileLength = (int)(new FileInfo(dto.Path)).Length;
                var buffer = new byte[Math.Min(Constants.CHUNK_SIZE, fileLength - offset)];

                stream.Position = offset;
                stream.Read(buffer, 0, buffer.Length);

                dto.Data = buffer;

                return dto;
            }
        }
    }
}
