using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MetalClientSetupWPF.Messages;
using MetalClientSetupWPF.ViewModel.Pages;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace MetalClientSetupWPF.ViewModel
{
     public class MainViewModel : ViewModelBase
     {
        public static LaunchAction Action { get; set; } = LaunchAction.Install;

        private readonly BootstrapperApplication _bootstrapper;

        private readonly Dispatcher _dispatcher;

        public ObservableCollection<PageViewModel> Pages { get; set; }

        private readonly IVariableSource _variableSource;

        private PageViewModel _selectedPage;
        public PageViewModel SelectedPage
        {
            get
            {
                return _selectedPage;
            }
            set
            {
                if (Set(nameof(SelectedPage), ref _selectedPage, value))
                {
                    _selectedPage.Activate();
                }
            }
        }

        public MainViewModel()
        {
            InitPages(Enumerable.Empty<PageViewModel>());
            SubscribeMessages();
        }

        public MainViewModel(IVariableSource variableSource, BootstrapperApplication bootstrapper, Dispatcher dispatcher, IEnumerable<PageViewModel> pages)
        {
            _variableSource = variableSource;
            _bootstrapper = bootstrapper;
            _dispatcher = dispatcher;

            if (pages != null)
                InitPages(pages);
            SubscribeMessages();
        }

        private void InitPages(IEnumerable<PageViewModel> pages)
        {
            Pages = new ObservableCollection<PageViewModel>();
            //BindingOperations.EnableCollectionSynchronization(Pages, Pages);

            foreach (var page in pages)
            {
                page.InitCommands();
                Pages.Add(page);
            }

            if (Pages.Any())
            {
                SelectedPage = Pages.First();
                SelectedPage.Current = true;
            }
        }

        private void SubscribeMessages()
        {
            Messenger.Default.Register<NextMessage>(this, HandlerNext);
            Messenger.Default.Register<PrevMessage>(this, HandlerPrev);
            Messenger.Default.Register<UninstallMessage>(this, HandlerUninstall);
            Messenger.Default.Register<ErrorMessage>(this, HandlerError);
            Messenger.Default.Register<CancelMessage>(this, HandlerCancel);
            Messenger.Default.Register<CancelInstallMessage>(this, HandlerCancelInstall);
            Messenger.Default.Register<CancelUninstallMessage>(this, HandlerCancelUninstall);
            Messenger.Default.Register<CloseAppMessage>(this, HandlerCloseApp);
        }

        private void HandlerNext(NextMessage message)
        {
            int indexPage = Pages.IndexOf(message.Page);
            int nextPageIndex = indexPage + 1;

            if (indexPage >= 0)
            {
                var prevPage = Pages[indexPage];
                prevPage.Success = true;
                prevPage.Current = false;
            }

            if (indexPage != -1 && nextPageIndex < Pages.Count)
            {
                SelectedPage = Pages[nextPageIndex];
                SelectedPage.Success = null;
                SelectedPage.Current = true;
            }
        }

        private void HandlerPrev(PrevMessage message)
        {
            int indexPage = Pages.IndexOf(message.Page);
            int prevPageIndex = indexPage - 1;

            if (indexPage >= 0)
            {
                var prevPage = Pages[indexPage];
                prevPage.Success = null;
                prevPage.Current = false;
            }

            if (indexPage != -1 && prevPageIndex < Pages.Count)
            {
                SelectedPage = Pages[prevPageIndex];
                SelectedPage.Success = null;
                SelectedPage.Current = true;
            }
        }

        private void HandlerError(ErrorMessage message)
        {
            SelectedPage.Success = false;
            SelectedPage.Current = false;

            lock (Pages)
            {
                int readyPageIndex = Pages.ToList().FindLastIndex(p => p is ReadyPageViewModel);

                if (readyPageIndex != -1 && readyPageIndex < Pages.Count)
                {
                    Pages[readyPageIndex] = new ErrorPageViewModel { ErrorMessage = message.Error };
                    Pages[readyPageIndex].InitCommands();

                    SelectedPage = Pages[readyPageIndex];
                    SelectedPage.Success = null;
                    SelectedPage.Current = true;
                }
            }
        }

        private void HandlerCancel(CancelMessage message)
        {
            SelectedPage.Success = false;
            SelectedPage.Current = false;

            lock (Pages)
            {
                Pages.ToList().ForEach(p => p.Success = p.Success ?? false);

                int readyPageIndex = Pages.ToList().FindLastIndex(p => p is ReadyPageViewModel);

                if (readyPageIndex != -1 && readyPageIndex < Pages.Count)
                {
                    Pages[readyPageIndex] = new CancelPageViewModel();
                    Pages[readyPageIndex].InitCommands();

                    SelectedPage = Pages[readyPageIndex];
                    SelectedPage.Success = null;
                    SelectedPage.Current = true;
                }
            }
        }

        private void HandlerUninstall(UninstallMessage message)
        {
            SelectedPage.Success = true;
            SelectedPage.Current = false;

            lock (Pages)
            {
                int installPageIndex = Pages.ToList().FindLastIndex(p => p is InstallPageViewModel);

                if (installPageIndex != -1 && installPageIndex < Pages.Count)
                {
                    Pages[installPageIndex] = new UninstallPageViewModel(_variableSource, _bootstrapper);
                    Pages[installPageIndex].InitCommands();

                    SelectedPage = Pages[installPageIndex];
                    SelectedPage.Success = null;
                    SelectedPage.Current = true;
                }
            }
        }

        private void HandlerCancelInstall(CancelInstallMessage message)
        {
            lock (Pages)
            {
                foreach (var installPage in Pages.OfType<InstallPageViewModel>())
                {
                    installPage.IsCancel = true;
                }
            }
        }

        private void HandlerCancelUninstall(CancelUninstallMessage message)
        {
            lock (Pages)
            {
                foreach (var installPage in Pages.OfType<UninstallPageViewModel>())
                {
                    installPage.IsCancel = true;
                }
            }
        }

        private void HandlerCloseApp(CloseAppMessage message)
        {
            _dispatcher?.InvokeShutdown();
            Application.Current?.MainWindow?.Close();
        }
    }
}