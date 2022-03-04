using MetalTransport.ModelEx;
using System;

namespace MetalTransport.Datagram
{
    /// <summary>
    /// Получить конкретный элемент по идентификатору
    /// </summary>
    [Serializable]
    public class GetElementData
        : BaseDTO
    {
        public GetElementData() { }

        public GetElementData(Guid id)
        {
            Id = id;
        }
    }
}
