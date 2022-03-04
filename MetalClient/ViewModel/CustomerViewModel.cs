using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;

namespace MetalClient.ViewModel
{
    public class CustomerViewModel 
        : ElementViewModelBase<CustomerDTO>
    {
        protected override DatagramType SetDatagramType => DatagramType.SetCustomerElement;

        protected override DatagramType GetDatagramType => DatagramType.GetCustomerElement;

        public CustomerViewModel(Guid id, ClientDataManager dataManager, ElementState state)
            : base(id, dataManager, state) { }
    }
}
