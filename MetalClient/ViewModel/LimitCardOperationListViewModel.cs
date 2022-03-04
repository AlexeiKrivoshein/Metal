using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class LimitCardOperationListViewModel
        : ComplexElementsListViewModel<LimitCardOperationDTO>
    {
        private Guid _orderId;

        private bool _isFactOperation = false;
        public bool IsFactOperation
        {
            get => _isFactOperation;
            set
            {
                _isFactOperation = value;
                OnPropertyChanged(nameof(IsFactOperation));
            }
        }

        public bool NotIsFactOperation => !_isFactOperation;

        //удаленные элементы
        public List<Guid> RemovedOperations { get; private set; }

        protected override DatagramType RemoveDatagramType => DatagramType.NONE;

        protected override DatagramType SetDatagramType => DatagramType.NONE;

        //кэш
        public ICollection<VersionListItemDTO> _operationCache;
        public ICollection<VersionListItemDTO> OperationCache
        {
            get => _operationCache;
            private set
            {
                _operationCache = value;
                OnPropertyChanged(nameof(OperationCache));
            }
        }

        // Выбор операции
        public Action<OperationListViewModel> ShowOperationList;
        private RelayCommand _selectOperationCommand;
        public RelayCommand SelectOperationCommand
        {
            get
            {
                return _selectOperationCommand ??
                  (_selectOperationCommand = new RelayCommand(obj => SelectOperation(),
                  (obj) =>   ShowOperationList != null && Selected != null));
            }
        }

        public LimitCardOperationListViewModel(Guid orderId, bool isFactOperation, ClientDataManager dataManager, ElementListSelectType selectType)
            : base(Guid.Empty, dataManager, selectType)
        {
            _orderId = orderId;
            IsFactOperation = isFactOperation;
            RemovedOperations = new List<Guid>();
        }

        protected override Task<List<LimitCardOperationDTO>> InnerLoad(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetLimitOperationList)
                                 .WithDTOObject(new GetOrderIdFilteredElementsList(_orderId))
                                 .Build();

            return  DataManager.ExcuteRequestAsync<SetListData<LimitCardOperationDTO>>(request, token).
                ContinueWith((t1) =>
                {
                    if (!TaskHelper.CheckError(t1, out var error))
                    {
                        ErrorInvoke(error);
                        return null;
                    }

                    RefreshOperationCache();

                    var data = t1.Result;
                    return data.Elements;
                }, token);
        }

        protected override LimitCardOperationDTO OpenElement()
        {
            return null;
        }

        protected override LimitCardOperationDTO CreateElement()
        {
            return new LimitCardOperationDTO { OrderId = _orderId };
        }

        protected override Task<bool> Remove()
        {
            RemovedOperations.Add(Selected.Id);
            _dispatcher.Invoke(
                        new Action(() =>
                        {
                            if (!Elements.Any())
                            {
                                return;
                            }

                            int index;
                            for (index = 0; Elements[index].Id != Selected.Id; index++) { }

                            if (Elements[index].Id == Selected.Id)
                            {
                                Elements.RemoveAt(index);
                            }
                        }));

            return TaskHelper.FromResult(true);
        }

        private void SelectOperation()
        {
            var viewModel = new OperationListViewModel(Selected.Id, DataManager, ElementListSelectType.Select);
            ShowOperationList(viewModel);

            if (viewModel.IsModify)
            {
                RefreshOperationCache();
            }

            if (viewModel.IsSelected &&
                  viewModel.Selected.Id != null &&
                  viewModel.Selected.Id != Guid.Empty)
            {
                Selected.OperationId = viewModel.Selected.Id;
            }
        }

        private void RefreshOperationCache()
        {
            OperationCache = DataManager.OperationNameCache.Values.OrderBy(x => x.Name).ToList();
        }

        protected override Task<Guid> AddElement()
        {
            var element = CreateElement();

            if (element == null) return null;

            _id = element.Id;
            IsModify = true;

            //обновление списка элементов
            Elements.Add(element);
            SetElements(Elements.ToList());

            return TaskHelper.FromResult(element.Id);
        }
    }
}
