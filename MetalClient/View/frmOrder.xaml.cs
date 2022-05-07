using MetalClient.Helper;
using MetalClient.View;
using MetalClient.ViewModel;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class frmOrder 
        : Window
    {
        private OrderViewModel _viewModel;
        private frmProcessed _process;

        private const int MAX_PAGE_INDEX = 3;

        private const int FIRST_PAGE = 0;
        private const int SECOND_PAGE = 1;
        private const int THIRD_PAGE = 2;
        private const int FOURTH_PAGE = 3;

        private bool[] _pageVisibleMap = new bool[MAX_PAGE_INDEX + 1];
        private int _maxVisiblePageIndex = MAX_PAGE_INDEX;
        private int _minVisiblePageIndex = FIRST_PAGE;

        private int _currentTab = 0;

        public Locker LockObject => _viewModel.LockObject;

        public frmOrder(OrderViewModel viewModel, Window owner)
        {
            InitializeComponent();
            
            _viewModel = viewModel;
            _viewModel.OnInform += Inform;
            _viewModel.OnError += Error;
            _viewModel.OnClose += Hide;
            _viewModel.ShowPrint = ShowPrint;
            _viewModel.ShowCustomerList = ShowCustomerList;
            _viewModel.ShowLimitCard = ShowLimitCard;
            _viewModel.ShowEmployeeList = ShowEmployeeList;
            _viewModel.ShowOperationList = ShowOperationList;
            _viewModel.ShowOrderGroupList = ShowOrderGroupList;
            _viewModel.ShowDrawingList = ShowDrawingList;

            for (int index = 0; index <= MAX_PAGE_INDEX; index++)
            {
                _pageVisibleMap[index] = TabIsVisible(index);
                if (_pageVisibleMap[index])
                {
                    if (_currentTab < 0)
                    {
                        _minVisiblePageIndex = index;
                        _currentTab = index;
                        tabs.TabIndex = _currentTab;
                    }

                    _maxVisiblePageIndex = index;
                }
            }

            if (_pageVisibleMap.Count(p => p) > 1)
            {
                Next.IsEnabled = true;
            }
            else
            {
                Prev.IsEnabled = false;
                Next.IsEnabled = false;
            }

            Owner = owner;
            DataContext = _viewModel;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            _process = new frmProcessed(this);

            _viewModel.PrgShow = (header) => _process.Start(header).Token;
            _viewModel.PrgHide = () => _process.Stop();
            
            _viewModel.Load();

            base.OnContentRendered(e);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Prev.IsEnabled = true;

            if (_currentTab < _maxVisiblePageIndex)
            {
                for (var index = (_currentTab + 1); index <= _maxVisiblePageIndex; index++)
                {
                    if (_pageVisibleMap[index])
                    {
                        _currentTab = index;
                        tabs.SelectedItem = tabs.Items[_currentTab];
                        break;
                    }
                }
            }

            if (_currentTab == _maxVisiblePageIndex)
                Next.IsEnabled = false;
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            Next.IsEnabled = true;

            if (_currentTab > _minVisiblePageIndex)
            {
                for (var index = (_currentTab - 1); index >= _minVisiblePageIndex; index--)
                {
                    if (_pageVisibleMap[index])
                    {
                        _currentTab = index;
                        tabs.SelectedItem = tabs.Items[_currentTab];
                        break;
                    }
                }
            }

            if (_currentTab == _minVisiblePageIndex)
                Prev.IsEnabled = false;
        }

        private void ShowPrint(OrderDTO order, List<OrderOperationDTO> orderOperations)
        {
            var frmprint = new frmOrderPrint(_viewModel.DataManager, order, orderOperations, this);
            frmprint.ShowDialog();
        }

        private void ShowCustomerList(CustomerListViewModel viewModel)
        {
            var frmCustomer = new frmCustomersList(viewModel, this);
            frmCustomer.ShowDialog();
        }

        private void ShowOrderGroupList(OrderGroupListViewModel viewModel)
        {
            var frmOrderGroups = new frmOrderGroupList(viewModel, this);
            frmOrderGroups.ShowDialog();
        }

        private void ShowLimitCard(LimitCardViewModel viewModel)
        {
            var limitCard = new frmLimitCard(viewModel, this);
            limitCard.ShowDialog();
        }

        private void ShowEmployeeList(EmployeeListViewModel viewModel)
        {
            var frmEmployee = new frmEmployeeList(viewModel, this);
            frmEmployee.ShowDialog();
        }

        private void ShowOperationList(OperationListViewModel viewModel)
        {
            var frmOperation = new frmOperationList(viewModel, this);
            frmOperation.ShowDialog();
        }

        private void ShowDrawingList(OrderDrawingListViewModel viewModel)
        {
            var order = new frmOrderDrawingList(viewModel , this);
            order.ShowDialog();
        }

        private void DoubleValid(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(new Regex(@"[0-9\.]").IsMatch(e.Text));
        }

        private void IntValid(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(new Regex(@"[0-9]").IsMatch(e.Text));
        }

        private void SelectAllOnKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.SelectAll();
        }

        private void SelectAllOnMouseCapture(object sender, MouseEventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.SelectAll();
        }

        private bool TabIsVisible(int tabIndex)
        {
            if (tabIndex == 0 && UserGroupHelper.IsVisible(LockObject, UserGroupHelper.MAIN_1, UserGroupHelper.TECH_1_2))
                return true;
            else if (tabIndex == 1 && UserGroupHelper.IsVisible(LockObject, UserGroupHelper.DIRECTOR_3, UserGroupHelper.SALES_13))
                return true;
            else if (tabIndex == 2 && UserGroupHelper.IsVisible(LockObject, UserGroupHelper.DIRECTOR_14, UserGroupHelper.MAN_15_18))
                return true;
            else if (tabIndex == 3 && UserGroupHelper.IsVisible(LockObject, UserGroupHelper.OTK_19_22, UserGroupHelper.SEND_28_29))
                return true;

            return false;
        }

        private void Error(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => MessageBoxHelper.ErrorMesage(Title, message, this)));
        }

        private void Inform(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => MessageBoxHelper.InformationMesage(Title, message, this)));
        }
    }
}
