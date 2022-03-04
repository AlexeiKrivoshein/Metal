using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Datagram.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;
using MetalClient.Exceptions;
using MetalTransport.Helper;
using MetalTransport.ModelEx;

namespace MetalClient.Helper
{
    public static class FileHelper
    {
        public static Guid SaveFile(string fileName, Guid orderId, ClientDataManager dataManager, CancellationToken token)
        {
            var path = fileName;
            var fileId = Guid.NewGuid();
            var content = File.ReadAllBytes(path);
            var buffer = new byte[Constants.CHUNK_SIZE];
            var size = content.Length;
            var chunkCount = (size / Constants.CHUNK_SIZE) + ((size % Constants.CHUNK_SIZE) > 0 ? 1 : 0);
            var offset = 0;
            var factory = new DatagramFactory();

            //файл 1 или менее чанков
            if (size <= Constants.CHUNK_SIZE)
            {
                buffer = new byte[size];
                Array.Copy(content, buffer, size);

                var filedata = new MetalFileDTO
                {
                    Id = fileId,
                    OrderId = orderId,
                    Name = Path.GetFileName(path),
                    ChunkCount = 1,
                    Index = 0,
                    Data = buffer
                };

                var request = factory.WithType(DatagramType.SetFileData).WithDTOObject(filedata).Build();
                dataManager.ExcuteRequestAsync<HandledDTO>(request, token).Wait();
            }
            else //2 и более чанков
            {
                var index = 0;
                //отправка чанков, пока не закончится контент
                while (offset < size)
                {
                    // последний чанк меньше CHUNK_SIZE
                    if (size - offset < Constants.CHUNK_SIZE)
                    {
                        buffer = new byte[size - offset];
                        Array.Copy(content, offset, buffer, 0, size - offset);
                    }
                    else
                        Array.Copy(content, offset, buffer, 0, Constants.CHUNK_SIZE);

                    var filedata = new MetalFileDTO
                    {
                        Id = fileId,
                        OrderId = orderId,
                        Name = Path.GetFileName(path),
                        ChunkCount = chunkCount,
                        Index = index,
                        Data = buffer
                    };

                    var request = factory.WithType(DatagramType.SetFileData).WithDTOObject(filedata).Build();
                    dataManager.ExcuteRequestAsync<HandledDTO>(request, token).Wait();

                    offset = offset + Constants.CHUNK_SIZE;
                    index++;
                }
            }

            return fileId;
        }

        public static Task<string> LoadFile(Guid fileId, ClientDataManager dataManager, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<string>();

            var factory = new DatagramFactory();

            var request = factory.WithType(DatagramType.GetFileData)
                                 .WithDTOObject(new GetFileElementData(fileId, 0))
                                 .Build();

            dataManager.ExcuteRequestAsync<MetalFileDTO>(request, token).
                ContinueWith((t1) =>
                {
                    if (!TaskHelper.CheckError(t1, out var error))
                    {
                        tcs.SetException(new Exception(error));
                        return;
                    }

                    var file = t1.Result;

                    var dir = $@"{AppDomain.CurrentDomain.BaseDirectory}\Files\";
                    var path = dir + file.Name;

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (File.Exists(path))
                        File.Delete(path);

                    using (FileStream fs = new FileStream(path, FileMode.CreateNew))
                    {
                        fs.Write(file.Data, 0, file.Data.Length);
                    }

                    //файл больше одно чанка, получаем остальные
                    if (file.ChunkCount > 1)
                    {
                        for (int index = 1; index < file.ChunkCount; index++)
                        {
                            //запрос следующего чанка
                            request = factory.WithType(DatagramType.GetFileData)
                                             .WithDTOObject(new GetFileElementData(fileId, index))
                                             .Build();

                            //TODO асинхронное получение чанков файла
                            var task = dataManager.ExcuteRequestAsync<MetalFileDTO>(request, token);
                            task.Wait();
                                
                            if (task.IsFaulted)
                            {
                                throw task.Exception;
                            }

                            if (task.IsCanceled)
                            {
                                return;
                            }

                            var content = task.Result;
                            using (FileStream fs = new FileStream(path, FileMode.Append))
                            {
                                fs.Write(content.Data, 0, content.Data.Length);
                            }
                        }

                    }

                    tcs.SetResult(path);
                }, token);

            return tcs.Task;
        }
    }
}
