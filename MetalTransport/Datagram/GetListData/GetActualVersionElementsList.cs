using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalTransport.Datagram.GetListData
{
    /// <summary>
    /// Запрос элементов ведущих историю,
    /// используется для обновления списка конкретных элементов (напр. актуализация списка заказов),
    /// возвращает только те элементы ключи которых содержатся в словаре а версия выше версии в словаре.
    /// </summary>
    [Serializable]
    public class GetActualVersionElementsList
        : GetPaginationElementsList
    {
        /// <summary>
        /// Словарь ключ элемента и версия
        /// </summary>
        public IDictionary<Guid, long> ObjectsVersions = new Dictionary<Guid, long>();

        private GetActualVersionElementsList() { }

        public GetActualVersionElementsList(int pageIndex, int pageSize, bool excludeDelete, List<Sort> sortType, IDictionary<Guid, long> objectsVersions)
            : base(pageIndex, pageSize, excludeDelete, sortType)
        {
            ObjectsVersions = objectsVersions;
        }
    }
}
