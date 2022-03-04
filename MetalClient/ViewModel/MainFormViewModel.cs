using MetalClient.DataManager;
using MetalTransport.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalClient.ViewModel
{
    public class MainFormViewModel
    {
        protected ClientDataManager _dataManager;

        public OrderListViewModel OrderListViewModel { get; }

        public PlanListViewModel PlanListViewModel { get; }

        public Action<IViewModel> ShowCustomerList;
        // список заказчиков
        private RelayCommand _showCustomerListCommand;
        public RelayCommand ShowCustomerListCommand
        {
            get
            {
                return _showCustomerListCommand ??
                  (_showCustomerListCommand = new RelayCommand(obj =>
                  {
                      var viewModel = new CustomerListViewModel(Guid.Empty, _dataManager, ElementListSelectType.Show);
                      ShowCustomerList(viewModel);

                  }, (o) => ShowCustomerList != null));
            }
        }

        public Action<IViewModel> ShowMaterialList;
        // список материалов
        private RelayCommand _showMaterialListCommand;
        public RelayCommand ShowMaterialListCommand
        {
            get
            {
                return _showMaterialListCommand ??
                  (_showMaterialListCommand = new RelayCommand(obj =>
                  {
                      var viewModel = new MaterialListViewModel(Guid.Empty, _dataManager, ElementListSelectType.Show);
                      ShowMaterialList(viewModel);

                  }, (o) => ShowMaterialList != null));
            }
        }

        public Action<IViewModel> ShowEmployeeList;
        // список заказчиков
        private RelayCommand _showEmployeeListCommand;
        public RelayCommand ShowEmployeeListCommand
        {
            get
            {
                return _showEmployeeListCommand ??
                  (_showEmployeeListCommand = new RelayCommand(obj =>
                  {
                      var viewModel = new EmployeeListViewModel(Guid.Empty, _dataManager, ElementListSelectType.Show);
                      ShowEmployeeList(viewModel);

                  }, (o) => ShowEmployeeList != null));
            }
        }

        public Action<IViewModel> ShowUserGroupList;
        // список заказчиков
        private RelayCommand _showUserGroupListCommand;
        public RelayCommand ShowUserGroupListCommand
        {
            get
            {
                return _showUserGroupListCommand ??
                  (_showUserGroupListCommand = new RelayCommand(obj =>
                  {
                      var viewModel = new UserGroupListViewModel(Guid.Empty, _dataManager, ElementListSelectType.Show);
                      ShowUserGroupList(viewModel);

                  }, (o) => ShowUserGroupList != null));
            }
        }

        public Action<IViewModel> ShowOperationList;
        // список операций
        private RelayCommand _showOperationListCommand;
        public RelayCommand ShowOperationListCommand
        {
            get
            {
                return _showOperationListCommand ??
                  (_showOperationListCommand = new RelayCommand(obj =>
                  {
                      var viewModel = new OperationListViewModel(Guid.Empty, _dataManager, ElementListSelectType.Show);
                      ShowOperationList(viewModel);

                  }, (o) => ShowOperationList != null));
            }
        }


        public MainFormViewModel(ClientDataManager dataManager, OrderListViewModel orderListViewModel, PlanListViewModel planListViewModel)
        {
            _dataManager = dataManager;
            OrderListViewModel = orderListViewModel;
            PlanListViewModel = planListViewModel;

        }

        public Task Load()
        {
            var task = new Task(() =>
            {
                var t1 = OrderListViewModel.Load();
                var t2 = PlanListViewModel.Load(true);

                Task.WaitAll(new Task[] { t1, t2 });
            });

            task.Start();

            return task;
        }
    }
}
