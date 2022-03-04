﻿using GalaSoft.MvvmLight.Messaging;
using MetalServerSetupWPF.Messages;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetalServerSetupWPF.ViewModel.Pages
{
    public class InstallPageViewModel
        : PageViewModel
    {
        private readonly BootstrapperApplication _bootstrapper;

        private int _currentProgress;

        public int CurrentProgress
        {
            get => _currentProgress;
            set => Set(nameof(CurrentProgress), ref _currentProgress, value);
        }

        private bool _isCancel;
        private bool _isCancelDone;

        public bool IsCancel
        {
            get => _isCancel;
            set => Set(nameof(IsCancel), ref _isCancel, value);
        }

        public InstallPageViewModel()
            : this(new VariablesSource(null, BootstrapperInstaller.BuildVersion()), null)
        {
        }

        public InstallPageViewModel(IVariableSource variableSource, BootstrapperApplication bootstrapper)
            : base("Установка", "", "Выполняется установка продукта", variableSource)
        {
            _bootstrapper = bootstrapper;
        }

        public override void Activate()
        {
            base.Activate();

            ModifyVariables();
            ClearContext();
            StartInstall();
        }

        protected override IEnumerable<CommandViewModel> GetViewCommands()
        {
            yield return CancelInstall();
        }

        public void StartInstall()
        {
            if (_bootstrapper != null)
            {
                _bootstrapper.Engine.Detect();

                _bootstrapper.Error += _bootstrapper_Error;
                _bootstrapper.Progress += _bootstrapper_Progress;
                _bootstrapper.ApplyComplete += _bootstrapper_ApplyComplete;
                _bootstrapper.PlanComplete += _bootstrapper_PlanComplete;

                MainViewModel.Action = LaunchAction.Install;
                _bootstrapper.Engine.Plan(LaunchAction.Install);
            }
            else
            {
                Thread.Sleep(3000);
                _bootstrapper_ApplyComplete(null, null);
            }
        }

        private void _bootstrapper_PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (e.Status >= 0)
            {
                _bootstrapper.Engine.Apply(IntPtr.Zero);
            }
        }

        private void _bootstrapper_Progress(object sender, ProgressEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;

            if (IsCancel)
            {
                e.Result = Result.Cancel;
                _isCancelDone = true;
            }
        }

        private void _bootstrapper_ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            if (_isCancelDone)
            {
                Success = false;
                Messenger.Default.Send(new CancelMessage());
            }
            else if (e.Status != 0)
            {
                Success = false;
                Messenger.Default.Send(new ErrorMessage { Error = $"Установка завершилась с кодом ошибки 0x{e.Status:X8}" });
            }
            else
            {
                Success = true;
                Messenger.Default.Send(new NextMessage { Page = this });
            }
        }

        private void _bootstrapper_Error(object sender, ErrorEventArgs e)
        {
            Success = false;
            Messenger.Default.Send(new ErrorMessage { Error = e.ErrorMessage });
        }

        public void ClearContext()
        {
            CurrentProgress = 0;
            CancelCommandEnabled = true;
            Success = null;
        }
    }
}
