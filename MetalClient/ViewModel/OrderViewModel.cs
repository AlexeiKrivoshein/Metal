using MetalClient.DataManager;
using MetalClient.Exceptions;
using MetalTransport.Datagram;
using MetalTransport.Datagram.Properties;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Win32;
using MetalClient.Helper;
using System.Threading.Tasks;
using MetalClient.View;
using System.ComponentModel;

namespace MetalClient.ViewModel
{
    public partial class OrderViewModel 
        : ElementViewModelBase<OrderDTO>
    {
        protected override DatagramType SetDatagramType => DatagramType.SetOrderElement;

        protected override DatagramType GetDatagramType => DatagramType.GetOrderElement;

        //операции лимитки
        private ObservableCollection<LimitCardOperationDTO> _operations;
        public ObservableCollection<LimitCardOperationDTO> Operations
        {
            get => _operations;
            set
            {
                _operations = value;
                OnPropertyChanged(nameof(Operations));
            }
        }

        private List<Guid> _removedOperations;

        //материалы лимитки
        private ObservableCollection<LimitCardMaterialDTO> _materials;
        public ObservableCollection<LimitCardMaterialDTO> Materials
        {
            get => _materials;
            set
            {
                _materials = value;
                OnPropertyChanged(nameof(Materials));
            }
        }

        private List<Guid> _removedMaterials;

        //операции заказа
        private ObservableCollection<OrderOperationDTO> _orderOperations;
        public ObservableCollection<OrderOperationDTO> OrderOperations
        {
            get => _orderOperations;
            set
            {
                _orderOperations = value;
                OnPropertyChanged(nameof(OrderOperations));
            }
        }

        private List<Guid> _removedOrderOperations;

        //выбранная операция
        private OrderOperationDTO _selectedOrderOperation;
        public OrderOperationDTO SelectedOrderOperation
        {
            get => _selectedOrderOperation;
            set
            {
                _selectedOrderOperation = value;
                OnPropertyChanged(nameof(SelectedOrderOperation));
            }
        }

        //модель лимитки
        public LimitCardViewModel LimitCardVM;
        //модель списка чертежей
        public OrderDrawingListViewModel OrderDrawingListVM;
        private List<Guid> _removedDrawingList;
        private List<Guid> _addingDrawingList;


        //кэши
        public ICollection<VersionListItemDTO> _customerCache;
        public ICollection<VersionListItemDTO> CustomerCache 
        {
            get => _customerCache;
            private set
            {
                _customerCache = value;
                OnPropertyChanged(nameof(CustomerCache));
            }
        }

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

        public ICollection<VersionListItemDTO> _employeeCache;
        public ICollection<VersionListItemDTO> EmployeeCache 
        {
            get => _employeeCache;
            private set
            {
                _employeeCache = value;
                OnPropertyChanged(nameof(EmployeeCache));
            }
        }

        private ICollection<VersionListItemDTO> _orderGroupCache;
        public ICollection<VersionListItemDTO> OrderGroupCache 
        { 
            get => _orderGroupCache;
            private set
            {
                _orderGroupCache = value;
                OnPropertyChanged(nameof(OrderGroupCache));
            }
        }

        public OrderViewModel(Guid id, ClientDataManager dataManager, ElementState state)
            :base(id, dataManager, state)
        {
            _operations = new ObservableCollection<LimitCardOperationDTO>();
            _materials = new ObservableCollection<LimitCardMaterialDTO>();
            _orderOperations = new ObservableCollection<OrderOperationDTO>();

            _removedOrderOperations = new List<Guid>();
            _removedOperations = new List<Guid>();
            _removedMaterials = new List<Guid>();
            _removedDrawingList = new List<Guid>();

            _addingDrawingList = new List<Guid>();

            LockObject.NotSaved = true;
            LockObject.StateLock = true;
        }

        protected override Task<bool> InitViewModel(CancellationToken token)
        {
            CustomerCache = DataManager.CustomerNameCache.Values.OrderBy(x => x.Name).ToList();
            OperationCache = DataManager.OperationNameCache.Values.OrderBy(x => x.Name).ToList();
            EmployeeCache = DataManager.EmployeeNameCache.Values.OrderBy(x => x.Name).ToList();
            OrderGroupCache = DataManager.OrderGroupNameCache.Values.OrderBy(x => x.Name).ToList();

            return TaskHelper.FromResult<bool>(true);
        }

        protected override Task<bool> AfterLoad(CancellationToken token)
        {
            return Task.Factory.StartNew<bool>(() =>
            {
                LockObject.NotSaved = false;
                LockObject.StateLock = (Element.SendDeliveryDate != Constants.EMPTY_DATETIME);
                OnPropertyChanged(nameof(LockObject));

                OrderDrawingListVM = new OrderDrawingListViewModel(DataManager, Element.Id, ElementListSelectType.Show);
                LimitCardVM = new LimitCardViewModel(DataManager, Element.Id, Element.TechCalcMultiplier, false, !UserGroupHelper.IsEditing(LockObject, UserGroupHelper.TECH_1_2), Element.Count);

                var operations = OperationsLoad(token);
                var drawingList = OrderDrawingListVM.Load();
                var limitCard = LimitCardVM.Load();

                Task.WaitAll(limitCard, operations, drawingList);

                if (operations.Result && drawingList.Result)
                {
                    if (Element.Number > 0)
                    {
                        Header = $"Заказ №{Element.Number} от {Element.Date.ToString("dd.MM.yyyy")}";
                    }
                    else
                    {
                        Header = "Новый заказ";
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        private Task<bool> OperationsLoad(CancellationToken token)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetOrderOperationList)
                                 .WithDTOObject(new GetOrderIdFilteredElementsList(_id))
                                 .Build();

            return DataManager.ExcuteRequestAsync<SetListData<OrderOperationDTO>>(request, token)
                .ContinueWith((t) =>
                {
                    if (!TaskHelper.CheckError(t, out var error))
                    {
                        ErrorInvoke(error);
                        return false;
                    }
                    else
                    {
                        /*
                        var data = t.Result;
                        var orderOperations = new ObservableCollection<OrderOperationDTO>();
                        foreach (var item in data.Elements)
                            if (item is OrderOperationDTO operation)
                                orderOperations.Add(operation);

                        OrderOperations = orderOperations;
                        State = ElementState.Loaded;
                        */
                        return true;
                    }
                });
        }

        protected override Task AfterSave(CancellationToken token, HandledDTO parameter)
        {
            return Task.Factory.StartNew((object param) =>
            {
                var handled = param as OrderHandledDTO;

                //необходимые работы
                foreach (var operation in _operations)
                {
                    var factory = new DatagramFactory();
                    var request = factory.WithType(DatagramType.SetLimitOperationElement).WithDTOObject(operation).Build();
                    DataManager.ExcuteRequestAsync<HandledDTO>(request, token).Wait();
                }

                //удаленные работы
                foreach (var id in _removedOperations)
                {
                    var factory = new DatagramFactory();
                    var request = factory.WithType(DatagramType.RemLimitOperationElement).WithDTOObject(new RemElementData(id, true)).Build();
                    DataManager.ExcuteRequestAsync<HandledDTO>(request, token).Wait();
                }

                //необходимые материалы
                foreach (var material in _materials)
                {
                    var factory = new DatagramFactory();
                    var request = factory.WithType(DatagramType.SetLimitMaterialElement)
                                         .WithDTOObject(material)
                                         .Build();
                    DataManager.ExcuteRequestAsync<HandledDTO>(request, token).Wait();
                }

                //удаленные материалы
                foreach (var id in _removedMaterials)
                {
                    var factory = new DatagramFactory();
                    var request = factory.WithType(DatagramType.RemLimitMaterialElement).WithDTOObject(new RemElementData(id, true)).Build();
                    DataManager.ExcuteRequestAsync<HandledDTO>(request, token).Wait();
                }

                //выполнениые работы заказа
                foreach (var operation in _orderOperations)
                {
                    if (operation.OperationId != Guid.Empty)
                    {
                        var factory = new DatagramFactory();
                        var request = factory.WithType(DatagramType.SetOrderOperationElement).WithDTOObject(operation).Build();
                        DataManager.ExcuteRequestAsync<HandledDTO>(request, token).Wait();
                    }
                }

                //удаленные работы заказа
                foreach (var id in _removedOrderOperations)
                {
                    var factory = new DatagramFactory();
                    var request = factory.WithType(DatagramType.RemOrderOperationElement).WithDTOObject(new RemElementData(id, true)).Build();
                    DataManager.ExcuteRequestAsync<HandledDTO>(request, token).Wait();
                }

                //удаленные файлы
                foreach (var id in _removedDrawingList)
                {
                    var factory = new DatagramFactory();
                    var request = factory.WithType(DatagramType.RemFile).WithDTOObject(new RemElementData(id, true)).Build();
                    DataManager.ExcuteRequestAsync<HandledDTO>(request, token).Wait();
                }

                _removedDrawingList.Clear();
                _addingDrawingList.Clear();

                if (OrderDrawingListVM != null)//список чертежей не был создан, т.е. заказ не загружен а создан
                {
                    OrderDrawingListVM.Adding.Clear();
                    OrderDrawingListVM.Removed.Clear();
                }

                Element.Number = handled.OrderNumber;

                LockObject.NotSaved = false;
                LockObject.StateLock = (Element.SendDeliveryDate != Constants.EMPTY_DATETIME);
                OnPropertyChanged(nameof(LockObject));

                InformInvoke($"Заказ № {Element.Number} сохранен.");

            }, parameter);
        }

        private void OpenLimitCard(bool isFact)
        {
            if (LimitCardVM == null)//лимитная карточка не была создана, т.е. заказ не загружен а создан
            {
                LimitCardVM = new LimitCardViewModel(DataManager, Element.Id, Element.TechCalcMultiplier, false, !UserGroupHelper.IsEditing(LockObject, UserGroupHelper.TECH_1_2), Element.Count);
                LimitCardVM.Load();
            }

            LimitCardVM.IsFact = isFact;
            ShowLimitCard(LimitCardVM);

            if (!LimitCardVM.IsModify)
            {
                return;
            }

            RefreshOperationCache();

            _removedOperations.Clear();
            _removedOperations.AddRange(LimitCardVM.RemovedOperations);

            _removedMaterials.Clear();
            _removedMaterials.AddRange(LimitCardVM.RemovedMaterials);

            Operations.Clear();

            foreach (var operation in LimitCardVM.Operations)
            {
                Operations.Add(operation.Clone());

                if (!OrderOperations.Any())
                {
                    var orderOperation = new OrderOperationDTO()
                    {
                        OrderId = Element.Id,
                        OperationId = operation.OperationId
                    };

                    OrderOperations.Add(orderOperation);
                }
            }

            Materials.Clear();
            foreach (var material in LimitCardVM.Materials)
            {
                var added = material.Clone();

                //дополнение фактических материалов
                if (!LimitCardVM.IsFact)
                {
                    if (added.FactMaterials.Count == 1 &&
                        added.FactMaterials[0].MaterialId == Guid.Empty)
                    {
                        added.FactMaterials[0].MaterialId = added.MaterialId;
                    }
                }
                else
                {
                    //удаление фактических материалов, с незаполненным материалом
                    var count = added.FactMaterials.Count;

                    for (var i = 0; i < count; i++)
                    {
                        if (added.FactMaterials[i].MaterialId == Guid.Empty)
                        {
                            added.FactMaterials.Remove(added.FactMaterials[i]);
                        }
                    }

                    
                }

                Materials.Add(added);
            }

            Element.TechCalcPrice = LimitCardVM.SumMaterial + LimitCardVM.SumOperation;
            Element.TechCalcHour = LimitCardVM.SumHour;
            Element.TechCalcMinutes = LimitCardVM.SumMinutes;
            Element.TechCalcPriceDate = DateTimeHelper.DateTimeNow();
            Element.TechCalcMultiplier = LimitCardVM.Multiplier;

            Element.DirectorExpectedPrice = Element.TechCalcPrice;
            Element.SalesPrice = Element.TechCalcPrice;
            Element.SalesPriceDate = Element.TechCalcPriceDate;
        }

        private void SelectCustomer()
        {
            var viewModel = new CustomerListViewModel(Element.Customer.Id, DataManager, ElementListSelectType.Select);
            ShowCustomerList(viewModel);

            if (viewModel.IsModify)
            {
                CustomerCache = DataManager.CustomerNameCache.Values.OrderBy(x => x.Name).ToList();
            }

            if (viewModel.IsSelected)
            {
                Element.CustomerId = viewModel.Selected.Id;
                OnPropertyChanged(nameof(Element.Customer));
            }
        }

        private Task HandleCustomer(Guid customerId)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.GetCustomerElement).WithDTOObject(new GetElementData(customerId)).Build();

            return DataManager.ExcuteRequestAsync<CustomerDTO>(request, CancellationToken.None).
                ContinueWith((t1) =>
                {
                    if (!TaskHelper.CheckError(t1, out var error))
                    {
                        ErrorInvoke(error);
                        return;
                    }

                    var customer = t1.Result;
                    Element.Customer.CopyFrom(customer);
                });
        }

        private void SelectOrderGroup()
        {
            var viewModel = new OrderGroupListViewModel(Element.OrderGroup?.Id ?? Guid.Empty, DataManager, ElementListSelectType.Select);
            ShowOrderGroupList(viewModel);

            if (viewModel.IsModify)
            {
                OrderGroupCache = DataManager.OrderGroupNameCache.Values.OrderBy(x => x.Name).ToList();
            }

            if (viewModel.IsSelected)
            {
                if (Element.OrderGroup == null)
                {
                    Element.OrderGroup = new OrderGroupDTO();
                }

                Element.OrderGroup.Name = viewModel.Selected.Name;
                Element.OrderGroup.Id = viewModel.Selected.Id;
                Element.OrderGroupId = viewModel.Selected.Id;

                OnPropertyChanged(nameof(Element.OrderGroup));
            }
        }

        private void EditDrawingList()
        {
            if (OrderDrawingListVM == null)//список чертежей не был создан, т.е. заказ не загружен а создан
            {
                OrderDrawingListVM = new OrderDrawingListViewModel(DataManager, Element.Id, ElementListSelectType.Show);
            }

            ShowDrawingList(OrderDrawingListVM);

            _removedDrawingList.Clear();
            _removedDrawingList.AddRange(OrderDrawingListVM.Removed);

            _addingDrawingList.Clear();
            _addingDrawingList.AddRange(OrderDrawingListVM.Adding);
        }

        private void AddOperation()
        {
            OrderOperations.Add(new OrderOperationDTO()
            {
                StartDate = DateTimeHelper.DateTimeNow(),
                OrderId = Element.Id
            });
        }


        private void SelectOrderOperationEmployee()
        {
            var viewModel = new EmployeeListViewModel(SelectedOrderOperation.EmployeeId.HasValue ? SelectedOrderOperation.EmployeeId.Value : Guid.Empty, DataManager, ElementListSelectType.Select);
            ShowEmployeeList(viewModel);

            if (viewModel.IsModify)
            {
                EmployeeCache = DataManager.EmployeeNameCache.Values.OrderBy(x => x.Name).ToList();
            }

            if (viewModel.IsSelected)
            {
                SelectedOrderOperation.EmployeeId = viewModel.Selected.Id;
                OnPropertyChanged(nameof(SelectedOrderOperation));
            }
        }

        private void SelectOrderOperation()
        {
            var viewModel = new OperationListViewModel(SelectedOrderOperation.Id, DataManager, ElementListSelectType.Select);
            ShowOperationList(viewModel);

            if (viewModel.IsModify)
            {
                RefreshOperationCache();
            }

            if (viewModel.IsSelected)
            {
                SelectedOrderOperation.OperationId = viewModel.Selected.Id;
                OnPropertyChanged(nameof(SelectedOrderOperation));
            }
        }

        private void RefreshOperationCache()
        {
            OperationCache = DataManager.OperationNameCache.Values.OrderBy(x => x.Name).ToList();
        }

        protected override Task Close()
        {
            if (_addingDrawingList.Any())
            {
                //удаленние добавленных файлов при отмене сохранения
                foreach (var id in _addingDrawingList)
                {
                    var factory = new DatagramFactory();
                    var request = factory.WithType(DatagramType.RemFile).WithDTOObject(new RemElementData(id, true)).Build();
                    DataManager.ExcuteRequestAsync<HandledDTO>(request, CancellationToken.None);
                }
            }

            return TaskHelper.Complite();
        }

        protected override bool SaveValidation()
        {
            return Element != null &&
                   Element.CustomerId != Guid.Empty &&
                   !string.IsNullOrEmpty(Element.Name) &&
                   Element.Count > 0;
        }

        protected override Task<HandledDTO> InnerSave(CancellationToken token, CancellationTokenSource processToken)
        {
            var factory = new DatagramFactory();
            var request = factory.WithType(SetDatagramType).WithDTOObject(Element).Build();

            var task = DataManager.ExcuteRequestAsync<OrderHandledDTO>(request, token)
            .ContinueWith((t) =>
            {
                if (!CheckCancelled(token) ||
                    !CheckSaveProcess(t))
                {
                    processToken.Cancel();
                    return null;
                }
                else
                {
                    return t.Result as HandledDTO;
                }
            });

            return task;
        }
    }
}
