using System;
using System.Windows;
using System.Windows.Threading;
using MetalClient.Helper;
using MetalClient.ViewModel;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmLogin
        : Window
    {
        private LoginViewModel _viewModel;
        private frmProcessed _process;

        public frmLogin(LoginViewModel viewModel, Window owner)
        {
            InitializeComponent();

            _viewModel = viewModel;
            _viewModel.OnInform += Inform;
            _viewModel.OnError += Error;
            _viewModel.OnSelected += Hide;

            Owner = owner;
            DataContext = _viewModel;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            _process = new frmProcessed(this);
            _viewModel.PrgShow += (header) => _process.Start(header);
            _viewModel.PrgHide += () => _process.Stop();

            _viewModel.Load();

            base.OnContentRendered(e);

            txtPassword.Focus();
        }

        private void Inform(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => MessageBoxHelper.InformationMesage(Title, message, this)));
        }

        private void Error(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() => MessageBoxHelper.ErrorMesage(Title, message, this)));
        }
    }
}
