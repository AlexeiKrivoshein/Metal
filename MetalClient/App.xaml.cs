using log4net;
using MetalClient.Client;
using MetalClient.DataManager;
using MetalClient.View;
using MetalClient.ViewModel;
using MetalDiagnostic.Logger;
using System;
using System.IO;
using System.Windows;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
        : Application
    {
        private ClientDataManager _dataManager;
        private bool _OK = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            LogService.InitLogger(LoggerType.Client);

            AppDomain domain = AppDomain.CurrentDomain;
            domain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            var client = new TcpMetalClient();
            client.Start();

            _dataManager = new ClientDataManager(client);

            var userId = Guid.Empty;
            if (File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}\user.def"))
            {
                var content = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\user.def");
                if (!string.IsNullOrWhiteSpace(content))
                {
                    Guid.TryParse(content, out userId);
                }
            }

            var loginVM = new LoginViewModel(userId, _dataManager, ElementListSelectType.Select);
            var login = new frmLogin(loginVM, null);
            login.ShowDialog();

            if (! loginVM.IsSelected)
            {
                Shutdown(0);
                return;
            }
            else
            {
                _dataManager.SecurityContext = loginVM.Context;
                _OK = true;
            }

            var loadVm = new LoadViewModel(_dataManager);
            var load = new frmLoad(loadVm);
            load.ShowDialog();

            if (loadVm.IsSucces && _OK)
            {
                var orderViewModel = new OrderListViewModel(Guid.Empty, _dataManager, ElementListSelectType.Show);
                var planViewModel = new PlanListViewModel(Guid.Empty, _dataManager, ElementListSelectType.Show);
                var viewModel = new MainFormViewModel(_dataManager, orderViewModel, planViewModel);

                var orderList = new frmOrderList(viewModel, null);
                orderList.Show();
            }
            else
            {
                Shutdown(0);
                return;
            }
        }

        private void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var log = LogService.GetLogger(nameof(App));

            Exception ex = (Exception)args.ExceptionObject;
            log.Error($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }
}
