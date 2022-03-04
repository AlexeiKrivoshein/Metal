using MetalTransport.Datagram.GetListData;
using MetalTransport.ModelEx;
using System;

namespace MetalTransport.Datagram
{
    /// <summary>
    /// Запрос элементов отфильтрованных по идентификатору заказа (напр. получение данных лимитной карты)
    /// </summary>
    [Serializable]
    public sealed class GetOrderIdFilteredElementsList
        : GetListDataBase
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public Guid OrderId { get; set; }

        private GetOrderIdFilteredElementsList() { }

        public GetOrderIdFilteredElementsList(Guid orderId)
            : base(true)
        {
            OrderId = orderId;
        }
    }
}
