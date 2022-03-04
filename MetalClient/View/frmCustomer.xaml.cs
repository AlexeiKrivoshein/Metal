using MetalClient.Helper;
using MetalClient.ViewModel;
using MetalTransport.ModelEx;
using System;
using System.Windows;
using System.Windows.Threading;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for frmCustomer.xaml
    /// </summary>
    public partial class frmCustomer 
        : Window
    {
        private ElementViewModelBase<CustomerDTO> _viewModel;
        private frmProcessed _process;

        public frmCustomer(ElementViewModelBase<CustomerDTO> viewModel, Window owner)
        {
            InitializeComponent();

            _viewModel = viewModel;
            Owner = owner;
            DataContext = _viewModel;

            _viewModel.OnClose += HideForm;
            _viewModel.OnSave += HideForm;
            _viewModel.OnInform += Inform;
            _viewModel.OnError += Error;
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
