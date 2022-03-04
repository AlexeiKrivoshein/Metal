using MetalDAL.Mapper;
using MetalDAL.Model;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.IO;

namespace MetalServer.Handler
{
    [Handler]
    public class SetFileHandler
        : BaseHandler<MetalFileDTO, HandledDTO>
    {
        private List<DatagramType> _types = new List<DatagramType> { DatagramType.SetFileData };

        protected override List<DatagramType> Types() => _types;

        public override string ExceptionHeader() => "Не удалось сохранить файл";

        protected override HandledDTO InnerHandle(MetalFileDTO data, DatagramType type)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var order = Manager.GetElement<Order>(data.OrderId) as OrderDTO;

            var dir = $@"{Manager.FilePath}\{FileHelper.PrepareFileName(order.Customer.Name)}\{order.Number}_{order.Date:dd.MM.yyyy}\";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var path = $"{dir}{data.Name}";

            //запись в файл
            if (data.Index == 0)
            {
                File.WriteAllBytes(path, data.Data);
            }
            else
            {
                using (FileStream stream = new FileStream(path, FileMode.Append))
                {
                    stream.Write(data.Data, 0, data.Data.Length);
                }
            }

            if (data.Index + 1 == data.ChunkCount) //последний чанк
            {
                data.Path = path;
                Manager.SetElement(MapperContainer.Instance.Map<MetalFile>(data));
            }

            return HandledDTO.Success(data.Id);
        }
    }
}
