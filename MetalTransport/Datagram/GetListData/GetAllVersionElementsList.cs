using MetalTransport.Datagram.GetListData;
using MetalTransport.ModelEx;
using System;

namespace MetalTransport.Datagram
{
    /// <summary>
    /// Запрос элементов ведущих историю,
    /// используется для справочников,
    /// возвращает наименования и ключ сразу всех элементов из базы с версией выше CacheVersion.
    /// </summary>
    [Serializable]
    public sealed class GetAllVersionElementsList
        : GetListDataBase
    {
        /// <summary>
        /// Версия
        /// </summary>
        public long CacheVersion { get; set; }

        private GetAllVersionElementsList() { }

        public GetAllVersionElementsList(bool excludeDelete, long cacheVersion)
            : base(excludeDelete) 
        {
            CacheVersion = cacheVersion;
        }
    }
}
