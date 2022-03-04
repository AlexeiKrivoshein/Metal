using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetalTransport.Datagram
{
    [Serializable]
    public class SetListData<T> 
        : BaseDTO
        where T: BaseDTO
    {
        public List<T> Elements { get; set; }
        public int TotalCount { get; set; }
        public long CacheVersion { get; set; }

        private SetListData()
        {
        }

        public SetListData(IEnumerable<T> elements, int totalCount)
        {
            Elements = elements.ToList();
            TotalCount = totalCount;
        }

        public SetListData(IEnumerable<T> elements, int totalCount, long actualVersion)
        {
            Elements = elements.ToList();
            TotalCount = totalCount;
            CacheVersion = actualVersion;
        }
    }
}
