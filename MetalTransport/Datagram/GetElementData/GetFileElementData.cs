using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.Datagram
{
    /// <summary>
    /// Получить чанк файла
    /// </summary>
    [Serializable]
    public sealed class GetFileElementData
        : GetElementData
    {
        public int FileIndex { get; set; }

        public GetFileElementData() { }

        public GetFileElementData(Guid id, int fileIndex)
            : base(id)
        {
            FileIndex = fileIndex;
        }
    }
}
