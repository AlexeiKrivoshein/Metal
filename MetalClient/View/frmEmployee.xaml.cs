using MetalClient.Helper;
using MetalClient.ViewModel;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Threading;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for frmEmployee.xaml
    /// </summary>
    public partial class frmEmployee : Window
    {
        private EmployeeViewModel _viewModel;
        private frmProcessed _process;

        public frmEmployee(EmployeeViewModel viewModel, Window owner)
        {
            InitializeComponent();

            _viewModel = viewModel;
            Owner = owner;
            DataContext = viewModel;

            _viewModel.OnClose += HideForm;
            _viewModel.OnSave += HideForm;
            _viewModel.OnInform += Inform;
            _viewModel.OnError += Error;
            _viewModel.ShowUserGroupList += ShowUserGroupList;

            _viewModel.TakePassword = () => 
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    return SecurityHelper.GetMd5Hash(md5Hash, pbxPassword.Password);
                }
            };
        }

        private void ShowUserGroupList(IViewModel viewModel)
        {
            var frnUserGroupList = new frmUserGroupList(viewModel as UserGroupListViewModel, this);
            frnUserGroupList.ShowDialog();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            _process = new frmProcessed(this);

            _viewModel.PrgShow = (header) => _process.Start(header).Token;
            _viewModel.PrgHide = () => _process.Stop();

            _viewModel.Load();

            base.OnContentRendered(e);
        }

        private void HideForm()
        {
            Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() => Hide()));
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
