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
    /// Interaction logic for frmMaterialsList.xaml
    /// </summary>
    public partial class frmMaterialsList 
        : Window
    {
        private MaterialListViewModel _viewModel;
        private frmProcessed _process;

        public frmMaterialsList(MaterialListViewModel viewModel, Window owner)
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
            var customer = new frmMaterial(viewModel as MaterialViewModel, this);
            customer.ShowDialog();
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

        private void GrdMaterials_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        private void HideForm()
        {
            Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() => base.Hide()));
        }
    }
}
