using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;

namespace MetalClient.ViewModel
{
    public class OrderGroupViewModel
        : ElementViewModelBase<OrderGroupDTO>
    {
        protected override DatagramType SetDatagramType => DatagramType.SetOrderGroupElement;

        protected override DatagramType GetDatagramType => DatagramType.GetOrderGroupElement;

        public OrderGroupViewModel(Guid id, ClientDataManager dataManager, ElementState state)
            : base(id, dataManager, state) { }
    }
}
