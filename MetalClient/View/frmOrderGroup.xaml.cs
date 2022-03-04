using MetalClient.Helper;
using MetalClient.ViewModel;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MetalClient.View
{
    /// <summary>
    /// Interaction logic for frmOrderGroup.xaml
    /// </summary>
    public partial class frmOrderGroup : Window
    {
        private ElementViewModelBase<OrderGroupDTO> _viewModel;
        private frmProcessed _process;

        public frmOrderGroup(ElementViewModelBase<OrderGroupDTO> viewModel, Window owner)
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
