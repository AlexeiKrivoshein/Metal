using MetalTransport.Datagram.GetListData;
using MetalTransport.Datagram.Properties;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalTransport.Datagram
{
    /// <summary>
    /// Запрос элементов с пагинацией и сортировкой
    /// </summary>
    [Serializable]
    public class GetPaginationElementsList
        : GetListDataBase
    {
        /// <summary>
        /// Индекс страницы (сортировка по SortType)
        /// </summary>
        public int PageIndex;

        /// <summary>
        /// Размер страницы (сортировка по SortType)
        /// </summary>
        public int PageSize;

        /// <summary>
        /// Список полей сортировки
        /// </summary>
        public List<Sort> Sort;

        protected GetPaginationElementsList() { }

        public GetPaginationElementsList(int pageIndex, int pageSize, bool excludeDelete, List<Sort> sortType)
            : base(excludeDelete)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Sort = sortType;
        }
    }
}
