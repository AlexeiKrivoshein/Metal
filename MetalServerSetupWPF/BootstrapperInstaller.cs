using MetalServerSetupWPF.ViewModel;
using MetalServerSetupWPF.ViewModel.Pages;
using MetalTransport.Datagram;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MetalServerSetupWPF
{
    public class BootstrapperInstaller
        : BootstrapperApplication
    {
        private IVariableSource _variableSource;

        public static Dispatcher BootstrapperDispatcher { get; private set; }
        private static MainViewModel _viewModel;

        private static MainForm _view;

        public static MainForm BoostrapperWindow { get; private set; }

        protected override void Run()
        {
            BootstrapperDispatcher = Dispatcher.CurrentDispatcher;
            _variableSource = new VariablesSource(Engine, BuildVersion());

            try
            {
                var install = Command.Action == LaunchAction.Install;
                var showUi = Command.Display != Display.None && Command.Display != Display.Embedded;

                if (install && showUi)
                {
                    Install();
                }
                else
                {
                    Execute(Command.Action);
                }

                Engine.Quit(0);
            }
            catch (Exception)
            {
                Engine.Quit(1);
                throw;
            }
        }
        
        public static Version BuildVersion()
        {
            return typeof(DatagramBase).Assembly.GetName().Version;
        }
        
        protected virtual void Execute(LaunchAction action)
        {
            _variableSource.Load();
            _variableSource.SetVariable(BootstrapperVariables.Install, action == LaunchAction.Install ? "1" : string.Empty);

            _variableSource.Modify();

            var taskCompletionSource = new TaskCompletionSource<object>();
            Engine.Detect();

            Error += (s, e) =>
            {
                taskCompletionSource.TrySetException(new Exception(e.ErrorMessage + $" PacakgeId: { e.PackageId}, Action: { action}"));
            };
            ApplyComplete += (s, e) => taskCompletionSource.TrySetResult(null);
            PlanComplete += (s, e) => Engine.Apply(IntPtr.Zero);

            Engine.Plan(action);

            taskCompletionSource.Task.Wait();
        }
        
        private void Install()
        {
            _variableSource = new VariablesSource(Engine, BuildVersion());
            _variableSource.Load();
            _variableSource.SetVariable(BootstrapperVariables.Install, "1");

            _viewModel = new MainViewModel(_variableSource, this, BootstrapperDispatcher, BuildInstallPages(_variableSource));
            _view = new MainForm { DataContext = _viewModel };
            _view.Title += " " + BuildVersion();
            BoostrapperWindow = _view;
            _view.Closed += (sender, e) => BootstrapperDispatcher.InvokeShutdown();
            _view.Show();

            Dispatcher.Run();
        }
        
        private IEnumerable<PageViewModel> BuildInstallPages(IVariableSource variableSource)
        {
            yield return new WelcomePageViewModel(variableSource);
            yield return new PublicationPageViewModel(variableSource);
            yield return new DataBasePageViewModel(variableSource);
            yield return new InstallPageViewModel(variableSource, this);
            yield return new ReadyPageViewModel(variableSource);
        }
    }
}
