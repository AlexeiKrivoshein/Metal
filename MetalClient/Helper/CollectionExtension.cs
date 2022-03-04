using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalClient.Helper
{
    /// <summary>
    /// Расширения для <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// Возвращение пустого <see cref="IEnumerable{T}"/>, если текущий равен null. Используется в foreach.
        /// </summary>
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Есть ли в коллекции данные.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Есть ли в коллекции данные.
        /// </summary>
        public static bool IsListEmpty(this IList list)
        {
            return list == null
                   || list.Count == 0;
        }

        /// <summary>
        /// Получение первого элемента коллекции.
        /// </summary>
        public static object FirstItem(this IList list)
        {
            if (list.IsListEmpty())
                throw new InvalidOperationException("Коллекция не содержит элементов.");
            return list[0];
        }

        /// <summary>
        /// Получение последнего элемента коллекции.
        /// </summary>
        public static object LastItem(this IList list)
        {
            if (list.IsListEmpty())
                throw new InvalidOperationException("Коллекция не содержит элементов.");
            return list[list.Count - 1];
        }

        /// <summary>
        /// Получение generic варианта листа.
        /// </summary>
        public static List<object> AsList(this IList list)
        {
            if (list.IsListEmpty())
                return new List<object>();

            return (from object obj in list select obj).ToList();
        }

        /// <summary>
        /// Получение типизированного <see cref="IList{T}"/>.
        /// </summary>
        public static List<T> AsList<T>(this IList list)
        {
            if (list.IsListEmpty())
                return new List<T>();

            return (from object obj in list select (T)obj).ToList();
        }

        /// <summary>
        /// Добавление диапазона значений
        /// </summary>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list is List<T> trueList)
            {
                trueList.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// Удаление диапазона значений
        /// </summary>
        public static void RemoveRange<T>(this IList<T> list, int index, int count)
        {
            if (list is List<T> trueList)
            {
                trueList.RemoveRange(index, count);
            }
            else
            {
                while (count > 0)
                {
                    list.RemoveAt(index);
                    count--;
                }
            }
        }

        /// <summary>
        /// Слияние двух коллекций
        /// </summary>
        public static void Merge<T>(this ICollection<T> collection, IEnumerable<T> other)
        {
            if (collection is List<T> trueList)
            {
                trueList.AddRange(other);
            }
            else
            {
                foreach (var item in other)
                {
                    collection.Add(item);
                }
            }
        }
    }
}
