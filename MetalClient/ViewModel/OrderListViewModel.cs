using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MetalClient.DataManager;
using MetalTransport.Datagram;
using MetalTransport.Datagram.Properties;
using MetalTransport.ModelEx;
using System.Collections.Generic;
using MetalTransport.Helper;
using System.ComponentModel;

namespace MetalClient.ViewModel
{
    public class OrderListViewModel
        : ComplexElementsListViewModel<OrderListItemDTO>
    {
        private const int SMALL_FILTER_SIZE = 12;
        private const int BIG_FILTER_SIZE = 300;

        public bool IsFiltered { get; set; } = false;

        private FilterDTO _filterDTO = new FilterDTO();
        public FilterDTO Filter
        {
            get => _filterDTO;
            set
            {
                _filterDTO = value;
                OnPropertyChanged(nameof(Filter));
            }
        }

        private int _filterPanelSize = SMALL_FILTER_SIZE;
        public int FilterPanelSize
        {
            get => _filterPanelSize;
            set
            {
                _filterPanelSize = value;
                OnPropertyChanged(nameof(FilterPanelSize));
            }
        }

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

        //показать/скрыть панель фильтра
        private RelayCommand _toggleFilterCommand;
        public RelayCommand ToggleFilterCommand
        {
            get
            {
                return _toggleFilterCommand ??
                  (_toggleFilterCommand = new RelayCommand(obj =>
                  {
                      ToggleFilter();
                  }));
            }
        }

        //Применение фильтра
        private RelayCommand _setFilterCommand;
        public RelayCommand SetFilterCommand
        {
            get
            {
                return _setFilterCommand ??
                  (_setFilterCommand = new RelayCommand(obj =>
                  {
                      SetFilter();
                  }));
            }
        }

        //Сброс фильтра
        private RelayCommand _dropFilterCommand;
        public RelayCommand DropFilterCommand
        {
            get
            {
                return _dropFilterCommand ??
                  (_dropFilterCommand = new RelayCommand(obj =>
                  {
                      DropFilter();
                  }));
            }
        }

        //Комманда выбор группы заказов
        public Action<OrderGroupListViewModel> ShowFilterOrderGroupList;
        private RelayCommand _selectFilterOrderGroupCommand;
        public RelayCommand SelectFilterOrderGroupCommand
        {
            get
            {
                return _selectFilterOrderGroupCommand ??
                  (_selectFilterOrderGroupCommand = new RelayCommand(obj =>
                  {
                      SelectFilterOrderGroup();
                  }, (obj) => ShowFilterOrderGroupList != null));
            }
        }

        //Комманда выбор группы заказов
        private RelayCommand _clearFilterOrderGroupCommand;
        public RelayCommand ClearFilterOrderGroupCommand
        {
            get
            {
                return _clearFilterOrderGroupCommand ??
                  (_clearFilterOrderGroupCommand = new RelayCommand(obj =>
                  {
                      Filter.OrderGroupId = Guid.Empty;
                  }));
            }
        }

        //Комманда выбор заказчика
        public Action<CustomerListViewModel> ShowFilterCustomerList;
        private RelayCommand _selectFilterCustomerCommand;
        public RelayCommand SelectFilterCustomerCommand
        {
            get
            {
                return _selectFilterCustomerCommand ??
                  (_selectFilterCustomerCommand = new RelayCommand(obj =>
                  {
                      SelectFilterCustomer();
                  }, (obj) => ShowFilterCustomerList != null));
            }
        }

        //Комманда выбор группы заказов
        private RelayCommand _clearFilterCustomerCommand;
        public RelayCommand ClearFilterCustomerCommand
        {
            get
            {
                return _clearFilterCustomerCommand ??
                  (_clearFilterCustomerCommand = new RelayCommand(obj =>
                  {
                      Filter.CustomerId = Guid.Empty;
                  }));
            }
        }

        private Timer _orderRefreshTimer;
        //номер страницы
        private int _pageIndex = 0;
        //размер страницы
        private int _pageSize = Constants.DEFAULT_PAGE_ELEMENT_COUNT;

        // получить новую порцию заказов
        private RelayCommand _getNextOrdersCommand;
        public RelayCommand GetNextOrdersCommand
        {
            get
            {
                return _getNextOrdersCommand ??
                  (_getNextOrdersCommand = new RelayCommand(obj =>
                  {
                      _pageIndex++;
                      Load();
                  }, (obj) => (obj is int index && index >= ((_pageIndex + 1) * _pageSize))));
            }
        }

        protected override DatagramType RemoveDatagramType => DatagramType.RemOrderElement;

        protected override DatagramType SetDatagramType => DatagramType.SetOrderElement;

        private AutoResetEvent _ordersRefreshLock = new AutoResetEvent(true);

        public OrderListViewModel(Guid id, ClientDataManager dataManager, ElementListSelectType selectType)
            :base(id, dataManager, selectType)
        {
            _orderRefreshTimer = new Timer((x) => GetActualOrders(), true, Timeout.Infinite, Timeout.Infinite);
            OrderGroupCache = DataManager.OrderGroupNameCache.Values.OrderBy(x => x.Name).ToList();
            CustomerCache = DataManager.CustomerNameCache.Values.OrderBy(x => x.Name).ToList();

            _filterDTO.Sort = OrderDTO.ListSort;

            OnAdd += (Guid _) => GetActualOrders();           
            OnRemove += () => GetActualOrders();
        }

        protected override Task<List<OrderListItemDTO>> InnerLoad(CancellationToken token)
        {
            _orderRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _ordersRefreshLock.WaitOne();

            return DataManager.GetOrders(_pageIndex, _pageSize, token).ContinueWith((t) =>
            {
                if (!TaskHelper.CheckError(t, out var error))
                {
                    ErrorInvoke(error);
                    return null;
                }

                _ordersRefreshLock.Set();
                _orderRefreshTimer.Change(Constants.ORDERS_REFRESH_TIMEOUT_MS, Constants.ORDERS_REFRESH_TIMEOUT_MS);

                return DataManager.OrderCache.Values.Cast<OrderListItemDTO>().ToList();
            });
        }

        public void GetActualOrders()
        {
            _orderRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _ordersRefreshLock.WaitOne();

            DataManager.RefreshOrders(_pageIndex, _pageSize, CancellationToken.None).ContinueWith((t) =>
            {
                _ordersRefreshLock.Set();
                _orderRefreshTimer.Change(Constants.ORDERS_REFRESH_TIMEOUT_MS, Constants.ORDERS_REFRESH_TIMEOUT_MS);

                if (!TaskHelper.CheckError(t, out var error))
                {
                    ErrorInvoke(error);
                    return;
                }

                var list = DataManager.OrderCache.Values.Cast<OrderListItemDTO>().ToList();

                SetElements(list);
            });
        }

        protected override OrderListItemDTO CreateElement()
        {
            if (ShowElement == null)
                return null;

            var viewModel = new OrderViewModel(Guid.NewGuid(), DataManager, ElementState.Created);
            ShowElement(viewModel);

            if (viewModel.State == ElementState.Saved)
            {
                _id = viewModel.Element.Id;
                return viewModel.Element.ToListItem();
            }
            else
            {
                return null;
            }
        }

        protected override OrderListItemDTO OpenElement()
        {
            if (ShowElement == null)
                return null;

            var viewModel = new OrderViewModel(Selected.Id, DataManager, ElementState.Loading);
            ShowElement(viewModel);

            if (viewModel.State == ElementState.Saved)
            {
                _id = viewModel.Element.Id;
                return viewModel.Element.ToListItem();
            }
            else
            {
                return null;
            }
        }

        protected override SortDescription? GetSortDescription()
        {
            return new SortDescription("Number", ListSortDirection.Descending);
        }

        private void ToggleFilter()
        {
            FilterPanelSize = FilterPanelSize == SMALL_FILTER_SIZE ? BIG_FILTER_SIZE : SMALL_FILTER_SIZE;
        }

        private void SetFilter()
        {
            _orderRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _ordersRefreshLock.WaitOne();

            IsFiltered = true;

            var token = PrgShow?.Invoke("Фильтрация").Token ?? CancellationToken.None;

            var factory = new DatagramFactory();
            var request = factory.WithType(DatagramType.Filter)
                                 .WithDTOObject(Filter)
                                 .Build();

            DataManager.ExcuteRequestAsync<SetListData<OrderListItemDTO>>(request, token)
                .ContinueWith((t1) =>
                {
                    _ordersRefreshLock.Set();
                    PrgHide?.Invoke();

                    if (!TaskHelper.CheckError(t1, out var error))
                    {
                        ErrorInvoke(error);
                        DropFilter();
                        return;
                    }

                    var data = t1.Result;
                    var list = data.Elements;

                    SetElements(list);
                }, token);
        }

        private void DropFilter()
        {
            IsFiltered = false;
            Load();
        }

        private void SelectFilterOrderGroup()
        {
            var viewModel = new OrderGroupListViewModel(Filter.OrderGroupId, DataManager, ElementListSelectType.Select);
            ShowFilterOrderGroupList(viewModel);

            if (viewModel.IsModify)
            {
                OrderGroupCache = DataManager.OrderGroupNameCache.Values.OrderBy(x => x.Name).ToList();
            }

            if (viewModel.IsSelected)
            {
                Filter.OrderGroupId = viewModel.Selected.Id;
            }
        }

        private void SelectFilterCustomer()
        {
            var viewModel = new CustomerListViewModel(Filter.CustomerId, DataManager, ElementListSelectType.Select);
            ShowFilterCustomerList(viewModel);

            if (viewModel.IsModify)
            {
                CustomerCache = DataManager.CustomerNameCache.Values.OrderBy(x => x.Name).ToList();
            }

            if (viewModel.IsSelected)
            {
                Filter.CustomerId = viewModel.Selected.Id;
            }
        }
    }
}
