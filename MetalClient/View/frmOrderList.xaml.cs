using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MetalClient.DataManager;
using MetalClient.Helper;
using System.Windows.Media;
using MetalClient.ViewModel;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.ComponentModel;
using System.IO;
using OfficeOpenXml;
using System.Globalization;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class frmOrderList : Window
    {
        //количетсво строк до конца подгруженных данных когда начнется загрузка новой страницы
        private const int PRELOAD_ROW_COUNT = 4;

        private OrderListViewModel _orderViewModel;
        private PlanListViewModel _planViewModel;
        private MainFormViewModel _viewModel;

        private bool _isFilter = false;

        public frmOrderList(MainFormViewModel viewModel, Window owner)
        {
            InitializeComponent();

            _viewModel = viewModel;
            _orderViewModel = _viewModel.OrderListViewModel;
            _planViewModel = _viewModel.PlanListViewModel;

            _orderViewModel.OnInform += Inform;
            _orderViewModel.OnError += Error;
            _orderViewModel.ShowElement = ShowElement;

            _planViewModel.OnInform += Inform;
            _planViewModel.OnError += Error;
            _planViewModel.ShowElement = ShowElement;

            _viewModel.ShowCustomerList = ShowCustomerList;
            _viewModel.ShowEmployeeList = ShowEmployeeList;
            _viewModel.ShowUserGroupList = ShowUserGroupList;
            _viewModel.ShowMaterialList = ShowMaterialList;
            _viewModel.ShowOperationList = ShowOperationList;

            _viewModel.OrderListViewModel.ShowFilterOrderGroupList = ShowFilterOrderGroupList;
            _viewModel.OrderListViewModel.ShowFilterCustomerList = ShowFilterCustomerList;

            Owner = owner;
            DataContext = _viewModel;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            var orderProcess = new frmProcessed(this);

            _orderViewModel.PrgShow = (header) => orderProcess.Start(header);
            _orderViewModel.PrgHide = () => orderProcess.Stop();

            var planProcess = new frmProcessed(this);

            _planViewModel.PrgShow = (header) => planProcess.Start(header);
            _planViewModel.PrgHide = () => planProcess.Stop();

            _viewModel.Load();

            base.OnContentRendered(e);
        }

        private void GridUnloaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void ShowEmployeeList(IViewModel viewModel)
        {
            var frmEmployeeList = new frmEmployeeList(viewModel as EmployeeListViewModel, this);
            frmEmployeeList.ShowDialog();
        }

        private void ShowOperationList(IViewModel viewModel)
        {
            var frmOperationList = new frmOperationList(viewModel as OperationListViewModel, this);
            frmOperationList.ShowDialog();
        }

        private void ShowMaterialList(IViewModel viewModel)
        {
            var frmMaterialsList = new frmMaterialsList(viewModel as MaterialListViewModel, this);
            frmMaterialsList.ShowDialog();
        }

        private void ShowCustomerList(IViewModel viewModel)
        {
            var frmCustomersList = new frmCustomersList(viewModel as CustomerListViewModel, this);
            frmCustomersList.ShowDialog();
        }

        private void ShowUserGroupList(IViewModel viewModel)
        {
            var frnUserGroupList = new frmUserGroupList(viewModel as UserGroupListViewModel, this);
            frnUserGroupList.ShowDialog();
        }

        private void mnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void grdOrders_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_orderViewModel.GetNextOrdersCommand.CanExecute((int)e.VerticalOffset + (int)e.ViewportHeight + PRELOAD_ROW_COUNT)
                && !_orderViewModel.IsFiltered)
            {
                _orderViewModel.GetNextOrdersCommand.Execute(null);
            }
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

        private void ShowElement(IViewModel viewModel)
        {
            var order = new frmOrder(viewModel as OrderViewModel, this);
            order.ShowDialog();
        }

        private void ShowFilterOrderGroupList(OrderGroupListViewModel viewModel)
        {
            var frmOrderGroups = new frmOrderGroupList(viewModel, this);
            frmOrderGroups.ShowDialog();
        }

        private void ShowFilterCustomerList(CustomerListViewModel viewModel)
        {
            var frmCustomer = new frmCustomersList(viewModel, this);
            frmCustomer.ShowDialog();
        }

        private void DoubleValid(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex(@"[0-9\.]").IsMatch(e.Text);
        }

        private void IntValid(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex(@"[0-9]").IsMatch(e.Text);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var dir = $@"{AppDomain.CurrentDomain.BaseDirectory}\Files\";

            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException) { }
                }
            }
        }

        private void PlanPrint_Click(object sender, RoutedEventArgs e)
        {
            var buffer = Properties.Resources.plan;
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = $@"{folder}\Metal\План_{_planViewModel.PlanMonth}_{_planViewModel.PlanYear}";
            var fi = new FileInfo($@"{fileName}.xlsx");

            var number = 0;
            while (File.Exists(fi.FullName))
                fi = new FileInfo($@"{fileName}_{number++}.xlsx");

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["План"];

                    //Строки
                    var rowIndex = 8;

                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_planViewModel.PlanMonth);
                    worksheet.Cells[$"E4"].Value = $"{monthName} {_planViewModel.PlanYear}";

                    foreach (var row in _planViewModel.Elements)
                    {
                        worksheet.InsertRow(rowIndex, 1);
                        worksheet.Cells[$"A{rowIndex - 1}:O{rowIndex - 1}"].Copy(worksheet.Cells[$"A{rowIndex}:O{rowIndex}"]);

                        worksheet.Cells[$"A{rowIndex}"].Value = row.Number.ToString();
                        worksheet.Cells[$"B{rowIndex}"].Value = row.Customer;
                        worksheet.Cells[$"C{rowIndex}"].Value = row.Name;
                        worksheet.Cells[$"D{rowIndex}"].Value = row.Count.ToString();
                        worksheet.Cells[$"E{rowIndex}"].Value = row.Price.ToString();
                        worksheet.Cells[$"F{rowIndex}"].Value = row.Sum.ToString();
                        worksheet.Cells[$"G{rowIndex}"].Value = row.Maked.ToString();
                        worksheet.Cells[$"H{rowIndex}"].Value = row.Week1.ToString();
                        worksheet.Cells[$"I{rowIndex}"].Value = row.Week2.ToString();
                        worksheet.Cells[$"J{rowIndex}"].Value = row.Week3.ToString();
                        worksheet.Cells[$"K{rowIndex}"].Value = row.Week4.ToString();
                        worksheet.Cells[$"L{rowIndex}"].Value = row.MakedSum.ToString();
                        worksheet.Cells[$"M{rowIndex}"].Value = row.OrderInManufactureDate.ToString("dd MMMM yyyy", new CultureInfo("ru-RU"));
                        worksheet.Cells[$"N{rowIndex}"].Value = row.ReadyDate.ToString("dd MMMM yyyy", new CultureInfo("ru-RU"));
                        worksheet.Cells[$"O{rowIndex}"].Value = row.LimitCard;

                        rowIndex++;
                    }

                    //Итоги
                    worksheet.Cells[$"F{rowIndex}"].Value = _planViewModel.Footer.Sum;
                    worksheet.Cells[$"F{rowIndex}"].Style.Font.Bold = true;

                    worksheet.Cells[$"H{rowIndex}"].Value = _planViewModel.Footer.Week1Precent;
                    worksheet.Cells[$"H{rowIndex}"].Style.Font.Bold = true;

                    worksheet.Cells[$"I{rowIndex}"].Value = _planViewModel.Footer.Week2Precent;
                    worksheet.Cells[$"I{rowIndex}"].Style.Font.Bold = true;

                    worksheet.Cells[$"J{rowIndex}"].Value = _planViewModel.Footer.Week3Precent;
                    worksheet.Cells[$"J{rowIndex}"].Style.Font.Bold = true;

                    worksheet.Cells[$"K{rowIndex}"].Value = _planViewModel.Footer.Week4Precent;
                    worksheet.Cells[$"K{rowIndex}"].Style.Font.Bold = true;

                    worksheet.Cells[$"L{rowIndex}"].Value = _planViewModel.Footer.MakedSum;
                    worksheet.Cells[$"L{rowIndex}"].Style.Font.Bold = true;

                    if (!Directory.Exists(fi.Directory.FullName))
                        Directory.CreateDirectory(fi.Directory.FullName);

                    excelPackage.SaveAs(fi);
                }
            }

            Type officeType = Type.GetTypeFromProgID("Excel.Application");
            if (officeType == null)
                MessageBoxHelper.InformationMesage("Печать", $"Не установленн Excel. Файл сохранен в каталог {fi.FullName}", this);
            else
                System.Diagnostics.Process.Start(fi.FullName);
        }
    }
}