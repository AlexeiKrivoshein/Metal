using log4net;
using MetalDAL.Manager;
using MetalDiagnostic.Logger;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MetalServer.Handler
{
    public abstract class BaseHandler<TSource, TResult>
        : IDatagramHandler 
        where TSource : BaseDTO
        where TResult : BaseDTO
    {
        public ModelManager Manager { get; set; }

        public IEnumerable<DatagramType> DatagramTypes => Types();

        protected const string UNKNOWN_DATAGRAM_TYPE = "Не найден обработчик для датаграммы";

        protected abstract List<DatagramType> Types();

        public DatagramBase HandleAction(DatagramBase datagram, CancellationToken token)
        {
            if (datagram == null)
                throw new ArgumentNullException(nameof(datagram));

            var factory = new DatagramFactory();
            factory.WithCorelationId(datagram.CorelationId);

            var id = Guid.Empty;

            try
            {
                var data = SerializationHelper.Deserialize<TSource>(datagram.Data, out var content);

                if (data == null) 
                    throw new ArgumentException($"Не удаётся разобрать сообщение {datagram}{Environment.NewLine}{content}");

                id = data.Id;

                var result = InnerHandle(data, datagram.DataType);

                factory.WithType(datagram.DataType)
                       .WithDTOObject(result);
            }
            catch (Exception ex)
            {
                var exception = new Exception($"{ExceptionHeader()}{Environment.NewLine}{ex.Message}", ex);
                
                factory.WithType(DatagramType.Error)
                       .WithDTOObject(HandledDTO.Error(id, exception));
            }

            var response = factory.Build();

            return response;
        }

        public abstract string ExceptionHeader();

        protected abstract TResult InnerHandle(TSource data, DatagramType type);
    }
}
