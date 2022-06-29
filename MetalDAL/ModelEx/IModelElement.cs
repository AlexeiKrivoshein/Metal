using MetalDAL.Manager;
using MetalDAL.Model;
using MetalTransport.ModelEx;
using System;

namespace MetalCore.ModelEx
{
    public interface IModelElement<T>
    {
        Guid Id { get; set; }

        BaseDTO ToDTO();

        BaseDTO ToListItemDTO();

        void CopyFrom(IModelElement<T> value);

        /// <summary>
        /// Инициализация связанных внешних элементов
        /// </summary>
        /// <param name="manager"></param>
        void LoadOuther(ModelManager manager);

        /// <summary>
        /// Сохранение вложенных объектов
        /// </summary>
        /// <param name="manager"></param>
        void SaveInner(ModelManager manager);

        /// <summary>
        /// Удаление всех вложенных связанных объектов
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="permanent"></param>
        void RemoveInner(ModelManager manager, bool permanent);
    }
}
