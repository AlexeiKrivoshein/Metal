using System;
using System.Threading.Tasks;
using log = MetalDiagnostic.Logger.Logger;
using MetalTransport.Handler;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using MetalTransport.Helper;
using TaskHelper = MetalTransport.Helper.TaskHelper;


namespace MetalTransport.Datagram
{
    public class DatagramStreamAsync : IDisposable, ISendResultHandler, IDatagramStream
    {
        private const int WAIT_STREAM_TIMEOUT_MS = 200;
        private const int WAIT_RESPONSE_TIMEOUT_MS = 500;

        /// <summary>
        /// Размер заголовка
        /// </summary>
        private const int FRAME_HEADER_SIZE = 3;

        private Stream _stream;
        private CancellationTokenSource _innerTokenSource;
        private CancellationToken _innerToken;

        private AutoResetEvent _streamLock = null;

        public bool IsActive { get; private set; }
        private bool _brokenConnection;

        private Guid _id;

        public DatagramStreamAsync(Stream stream)
        {
            _stream = stream;
            IsActive = true;

            _innerTokenSource = new CancellationTokenSource();
            _innerToken = _innerTokenSource.Token;
            _streamLock = new AutoResetEvent(true);
            _brokenConnection = false;

            _id = Guid.NewGuid();
        }

        public async Task<DatagramBase> ReadDatagramAsync(CancellationToken token, int timeout = Timeout.Infinite)
        {
            if (_innerToken.IsCancellationRequested)
                return null;

            byte[] buffer = null;
            bool isLocked = false;
            try
            {
                if (!_streamLock.WaitOne(WAIT_STREAM_TIMEOUT_MS))
                {
                    log.Log.Warn($"Не удалось заблокировать поток для получения датаграммы. ({_id})");
                    return null;
                }

                isLocked = true;
                buffer = await InnerReadDatagramAsync(token, timeout);
            }
            finally
            {
                if (isLocked)
                    _streamLock.Set();
            }

            DatagramBase datagram = null;
            try
            {
                if (buffer.Length > 0)
                {
                    datagram = SerializationHelper.Deserialize<DatagramBase>(buffer);

                    log.Log.Debug($"Датаграмма получена [{datagram.DataType} - {datagram.Id}]");
                }
                else
                {
                    log.Log.Error($"Не удалось десериализовать датаграмму, получен пустой поток.");
                }
            }
            catch (Exception ex)
            {
                log.Log.Error($"Не удалось десериализовать датаграмму. {ex.Message}.");
            }

            return datagram;
        }

        private async Task<byte[]> InnerReadDatagramAsync(CancellationToken token, int timeout)
        {
            CancellationToken[] tokens;
            if (timeout > Timeout.Infinite)
                tokens = new CancellationToken[] { _innerToken, token, CancellationTokenHelper.WithTimeout(timeout).Token };
            else
                tokens = new CancellationToken[] { _innerToken, token };

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(tokens);
            byte[] buffer = null;
            byte resp = 0;

            try
            {
                var header = new byte[FRAME_HEADER_SIZE];

                log.Log.Debug("Начало чтения данных из канала");

                await _stream.ReadAsync(header, 0, FRAME_HEADER_SIZE, linkedCts.Token);
                var len = header[0] * 256;
                len += header[1];
                resp = header[2];

                buffer = new byte[len];

                await _stream.ReadAsync(buffer, 0, buffer.Length, linkedCts.Token);
                log.Log.Debug($"Получены данные [{buffer.Length} байт]");
            }
            catch (Exception ex)
            {
                log.Log.Error($"Ошибка чтения датаграммы {ex.Message} ({_id})");
                _brokenConnection = true;
                return null;
            }

            if (buffer == null ||
                buffer.Length == 0)
            {
                log.Log.Error($"Получен пустой поток датаграм ({_id})");
                _brokenConnection = true;
                return null;
            }

            if (resp > 0)
            {
                //отправляем подтверждение получения датаграммы
                log.Log.Debug("Отправка подтверждения");
                await InnerWriteDatagramAsync(DatagramBase.ResponseByte, Guid.Empty, DatagramType.Response, linkedCts.Token, WAIT_RESPONSE_TIMEOUT_MS);
                log.Log.Debug("Подтверждение отправлено");
            }

            return buffer;
        }

        public async Task<bool> WriteDatagramAsync(DatagramBase datagram, CancellationToken token, int timeout = Timeout.Infinite)
        {
            if (_innerToken.IsCancellationRequested)
                return false;

            bool isLocked = false;
            try
            {
                byte[] buffer = SerializationHelper.Serialize(datagram);
                if (!_streamLock.WaitOne(WAIT_STREAM_TIMEOUT_MS))
                {
                    log.Log.Warn($"Не удалось заблокировать поток для отправки датаграммы. ({_id})");
                    return false;
                }
                isLocked = true;

                log.Log.Debug("================== Начало отправки датаграммы ==================");
                var result = true;
                try
                {
                    result = await InnerWriteDatagramAsync(buffer, datagram.Id, datagram.DataType, token, timeout).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    log.Log.Error(ex.Message);
                }
                log.Log.Debug("================== Датаграмма отправлена ==================");

                return result;
            }
            finally
            {
                if (isLocked)
                {
                    _streamLock.Set();
                }
            }
        }

        private async Task<bool> InnerWriteDatagramAsync(byte[] buffer, Guid id, DatagramType type, CancellationToken token, int timeout = Timeout.Infinite)
        {
            CancellationToken[] tokens;
            if (timeout > Timeout.Infinite)
                tokens = new CancellationToken[] { _innerToken, token, CancellationTokenHelper.WithTimeout(timeout).Token };
            else
                tokens = new CancellationToken[] { _innerToken, token };

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(tokens);

            var len = (int)buffer.Length;

            try
            {
                log.Log.Debug("Начало записи данных в канал");
                var frame = new byte[len + FRAME_HEADER_SIZE];

                frame[0] = (byte)(len / 256);
                frame[1] = (byte)(len & 255);
                frame[2] = (byte)(type != DatagramType.Response ? 1 : 0);
                Array.Copy(buffer, 0, frame, FRAME_HEADER_SIZE, len);

                await _stream.WriteAsync(frame, 0, frame.Length, linkedCts.Token);
                await _stream.FlushAsync().ConfigureAwait(false);

                log.Log.Debug($"Датаграмма отправлена [{type} - {id}]");
            }
            catch (Exception ex)
            {
                log.Log.Error($"Ошибка отправки датаграммы [{type} - {id}] {ex.Message}. ({_id})");
                _brokenConnection = true;
                return false;
            }

            if (_stream is NamedPipeServerStream pipeServerStream)
            {
                pipeServerStream.WaitForPipeDrain();
            }

            if (_stream is NamedPipeClientStream pipeClientStream)
            {
                pipeClientStream.WaitForPipeDrain();
            }

            if (type == DatagramType.Response)
                return true;

            //ожидание подтверждения
            log.Log.Debug("Ожидание подтверждения");
            buffer = await InnerReadDatagramAsync(token, WAIT_RESPONSE_TIMEOUT_MS);
            log.Log.Debug("Подтверждение получено");

            if (buffer != null && buffer.Length > 0)
            {
                try
                {
                    var response = SerializationHelper.Deserialize<DatagramBase>(buffer);
                    log.Log.Debug("Подтверждение разобрано");
                    var result = ((response?.DataType ?? DatagramType.NONE) == DatagramType.Response);
                    log.Log.Debug($"Ответ: {result}");
                    return result;
                }
                catch (Exception ex)
                {
                    log.Log.Error(ex.Message);
                }
            }
            return false;
        }

        public void Close()
        {
            IsActive = false;

            _innerTokenSource.Cancel();
            TaskHelper.Delay(100).Wait();

            _stream.Close();
            _stream.Dispose();
        }

        public void Dispose()
        {
            Close();
        }

        public async Task<bool> SendResultAsync(DatagramBase datagram, CancellationToken token)
        {
            return await WriteDatagramAsync(datagram, token);
        }

        public bool getBroken()
        {
            return _brokenConnection;
        }
    }
}
