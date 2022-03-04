using System;

namespace MetalTransport.ModelEx
{
    /// <summary>
    /// Элемент списка плана
    /// </summary>
    [Serializable]
    public sealed class PlanItemDTO
        : BaseDTO
    {
        /// <summary>
        /// Номер заказа
        /// </summary>
        public int Number { get; set; }
       
        /// <summary>
        /// Заказчик
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// Наименование заказа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Количество ед. изготовления
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public double Sum { get; set; }

        /// <summary>
        /// Количество ед. изготовлено
        /// </summary>
        public int Maked { get; set; }

        /// <summary>
        /// Изготовлено в 1ый квартал
        /// </summary>
        public int Week1 { get; set; }

        /// <summary>
        /// Изготовлено в 2ой квартал
        /// </summary>
        public int Week2 { get; set; }

        /// <summary>
        /// Изготовлено в 3ий квартал
        /// </summary>
        public int Week3 { get; set; }

        /// <summary>
        /// Изготовлено в 4ый квартал
        /// </summary>
        public int Week4 { get; set; }

        /// <summary>
        /// Выполнение сумма
        /// </summary>
        public double MakedSum { get; set; }

        /// <summary>
        /// Дата получения заказа в производство
        /// </summary>
        public DateTime OrderInManufactureDate { get; set; }

        /// <summary>
        /// Срок изготовления до
        /// </summary>
        public DateTime ReadyDate { get; set; }

        /// <summary>
        /// Лимитная карточка
        /// </summary>
        public string LimitCard { get; set; }
    }
}
