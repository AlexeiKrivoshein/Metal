using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using System;

namespace MetalClient.ViewModel
{
    public class MaterialViewModel
        : ElementViewModelBase<MaterialDTO>
    {
        protected override DatagramType SetDatagramType => DatagramType.SetMaterialElement;

        protected override DatagramType GetDatagramType => DatagramType.GetMaterialElement;

        public MaterialViewModel(Guid id, ClientDataManager dataManager, ElementState state)
            : base(id, dataManager, state) { }
    }
}
