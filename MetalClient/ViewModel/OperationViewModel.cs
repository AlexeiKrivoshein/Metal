using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;

namespace MetalClient.ViewModel
{
    public class OperationViewModel
        : ElementViewModelBase<OperationDTO>
    {
        protected override DatagramType SetDatagramType => DatagramType.SetOperationElement;

        protected override DatagramType GetDatagramType => DatagramType.GetOperationElement;

        public OperationViewModel(Guid id, ClientDataManager dataManager, ElementState state)
            : base(id, dataManager, state) { }
    }
}
