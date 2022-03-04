using System;
using System.Threading;
using System.IO;
using MetalTransport.Helper;
using System.Runtime.Serialization;
using log4net;
using MetalDiagnostic.Logger;
using System.Net.Sockets;

namespace MetalTransport.Datagram
{
    /// <summary>
    /// Фасад потока для отправки датаграм
    /// </summary>
    public class DatagramStream 
        : IDatagramStream
    {
        /// <summary>
        /// Использовать гарантированную доставку, на каждое сообщение требуется ответ (TODO удалить)
        /// </summary>
        private const bool USE_GARANTY_DELIVERY = false;

        /// <summary>
        /// Размер заголовка
        /// </summary>
        private const int FRAME_HEADER_SIZE = 5;

        private NetworkStream _stream;
        private CancellationTokenSource _innerTokenSource;
        private CancellationToken _innerToken;

        // поток нарушен в ходе работы (необходимо пересоздать)
        public bool IsBroken { get; private set; }

        private string _source;

        private ILog _log = LogService.GetLogger(nameof(DatagramStream));
        private Guid id = Guid.NewGuid();

        public DatagramStream(NetworkStream stream, string source)
        {
            _source = source;
            _stream = stream;

            _innerTokenSource = new CancellationTokenSource();
            _innerToken = _innerTokenSource.Token;

            IsBroken = false;
        }


        public DatagramBase ReadDatagram(CancellationToken token)
        {
            byte[] buffer;
            int size = 0;
            try
            {
                buffer = InnerReadDatagram(token, out size);
            }
            catch (IOException)
            {
                IsBroken = true;
                throw;
            }

            return HandleByteArray(buffer, size);
        }

        private DatagramBase HandleByteArray(byte[] buffer, int size)
        {
            DatagramBase datagram;
            try
            {
                if (buffer.Length > 0)
                    datagram = SerializationHelper.Deserialize<DatagramBase>(buffer, out _);
                else
                    throw new SerializationException($"Не удалось десериализовать датаграмму, получен пустой буфер.");
            }
            catch (Exception ex)
            {
                throw new SerializationException($"Не удалось десериализовать датаграмму. {Environment.NewLine}size:{size}{Environment.NewLine}", ex);
            }

            return datagram;
        }

        private byte[] InnerReadDatagram(CancellationToken token, out int size)
        {
            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(_innerToken, token))
            {
                if (IsBroken)
                    throw new IOException("Поток нарушен.");

                var header = new byte[FRAME_HEADER_SIZE];
                var read = _stream.ReadAsync(header, 0, FRAME_HEADER_SIZE);
                read.Wait(linked.Token);

                var len = (header[3] << 24) | (header[2] << 16) | (header[1] << 8) | header[0];

                if (len == 0)
                {
                    IsBroken = true;
                    throw new IOException("Поток нарушен.");
                }

                using (MemoryStream memory = new MemoryStream())
                {
                    size = 0;
                    while (size < len)
                    {
                        var buffer = new byte[len - size];
                        read = _stream.ReadAsync(buffer, 0, len - size);
                        read.Wait(linked.Token);
                        size += read.Result;

                        memory.Write(buffer, 0, read.Result);
                    }

                    return memory.GetBuffer();
                }
            }
        }


        public void WriteDatagram(DatagramBase datagram, CancellationToken token, out string content, out int size)
        {
            byte[] buffer = SerializationHelper.Serialize(datagram, out content);
            size = buffer.Length;

            try
            {
                InnerWriteDatagram(buffer, datagram, token);

            }
            catch (IOException)
            {
                IsBroken = true;
                throw;
            }
        }

        private void InnerWriteDatagram(byte[] buffer, DatagramBase datagram, CancellationToken token)
        {
            var len = buffer.Length;
            var frame = new byte[len + FRAME_HEADER_SIZE];

            frame[0] = (byte)(len >> 0);
            frame[1] = (byte)(len >> 8);
            frame[2] = (byte)(len >> 16);
            frame[3] = (byte)(len >> 24);
            frame[4] = (byte)(datagram.DataType != DatagramType.Response && USE_GARANTY_DELIVERY  ? 1 : 0);

            Array.Copy(buffer, 0, frame, FRAME_HEADER_SIZE, len);

            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(_innerToken, token))
            {
                var write = _stream.WriteAsync(frame, 0, frame.Length);
                write.Wait(linked.Token);
            };
        }


        public void Close()
        {
            _innerTokenSource.Cancel();
            _stream.Close();
            _log.Debug($"Stream {id} остановлен");
        }
    }
}
