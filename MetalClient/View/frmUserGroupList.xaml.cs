using MetalClient.Helper;
using MetalClient.View;
using MetalClient.ViewModel;
using MetalTransport.ModelEx;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for frmUserGroupList.xaml
    /// </summary>
    public partial class frmUserGroupList : Window
    {
        private UserGroupListViewModel _viewModel;
        private frmProcessed _process;

        public frmUserGroupList(UserGroupListViewModel viewModel, Window owner)
        {
            InitializeComponent();
            
            _viewModel = viewModel;
            _viewModel.OnInform += Inform;
            _viewModel.OnError += Error;
            _viewModel.ShowElement = ShowElement;
            _viewModel.OnSelected += HideForm;

            Owner = owner;
            DataContext = _viewModel;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            _process = new frmProcessed(this);

            _viewModel.PrgShow = (header) => _process.Start(header);
            _viewModel.PrgHide = () => _process.Stop();
            
            _viewModel.Load();

            base.OnContentRendered(e);  
        }

        private void ShowElement(IViewModel viewModel)
        {
            var userGroup = new frmUserGroup(viewModel as UserGroupViewModel, Guid.Empty, this);
            userGroup.ShowDialog();
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
        
        private void HideForm()
        {
            Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() => base.Hide()));
        }
    }
}
