using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.Datagram.GetListData
{
    /// <summary>
    /// Сортировка поля
    /// </summary>
    [Serializable]
    public class Sort
    {
        /// <summary>
        /// Поле по которому сортируется
        /// </summary>
        public SortField Field { get; set; }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        public bool IsDesc { get; set; }


        private Sort() { }


        public Sort(SortField field, bool isDesc)
        {
            Field = field;
            IsDesc = isDesc;
        }
    }
}
