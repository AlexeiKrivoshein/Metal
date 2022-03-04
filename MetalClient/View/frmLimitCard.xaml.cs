using MetalClient.Helper;
using MetalClient.ValidateRule;
using MetalClient.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for LimitCard.xaml
    /// </summary>
    public partial class frmLimitCard
        : Window
    {
        private LimitCardViewModel _viewModel;
        private frmProcessed _process;

        /// <summary>
        /// Форма-лимитка
        /// </summary>
        /// <param name="dataManager">Менеджер данных</param>
        /// <param name="orderId">Идентификатор заказа</param>
        public frmLimitCard(LimitCardViewModel viewModel, Window owner)
        {
            InitializeComponent();

            _viewModel = viewModel;

            _viewModel.MaterialVM.OnInform += Inform;
            _viewModel.OperationVM.OnInform += Inform;

            _viewModel.MaterialVM.OnError += Error;
            _viewModel.OperationVM.OnError += Error;

            _viewModel.ShowMaterialList = ShowMaterialList;
            _viewModel.ShowOperationList = ShowOperationList;
            _viewModel.Hide = Hide;

            Owner = owner;
            DataContext = _viewModel;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            _process = new frmProcessed(this);

            _viewModel.PrgShow += (header) => _process.Start(header).Token;
            _viewModel.PrgHide += () => _process.Stop();

            base.OnContentRendered(e);
        }

        #region МАТЕРИАЛЫ
        private void ShowMaterialList(MaterialListViewModel viewModel)
        {
            var material = new frmMaterialsList(viewModel, this);
            material.ShowDialog();
        }
        #endregion

        #region ОПЕРАЦИИ
        private void ShowOperationList(OperationListViewModel viewModel)
        {
            var operation = new frmOperationList(viewModel, this);
            operation.ShowDialog();
        }
        #endregion

        #region ОБЩИЕ МЕТОДЫ ФОРМЫ
        private void GrdMaterials_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        private void GrdOperations_Unloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
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
        #endregion

        private void CalcMaterial(object sender, TextChangedEventArgs e)
        {
            _viewModel.CalcMaterial();
        }

        private void CalcOperation(object sender, TextChangedEventArgs e)
        {
            _viewModel.CalcOperation();
        }

        public void IntValid(object sender, TextCompositionEventArgs e)
        {
            Validator.IntValid(sender, e);
        }

        public void DoubleValid(object sender, TextCompositionEventArgs e)
        {
            Validator.DoubleValid(sender, e);
        }
    }
}
