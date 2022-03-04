using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalClientSetupWPF.ViewModel.Pages
{
    public class ReadyPageViewModel
        : PageViewModel
    {
        public ReadyPageViewModel()
            : this(null)
        {
        }

        public ReadyPageViewModel(IVariableSource variableSource)
            : base("Завершение", "", $"Установка Metal client завершена успешно", variableSource)
        {
        }

        public override void Activate()
        {
            SaveVariables();

            switch (MainViewModel.Action)
            {
                case LaunchAction.Install:
                    Description = $"Установка Metal client завершена успешно";
                    break;

                case LaunchAction.Uninstall:
                    Description = $"Удаление Metal client выполнено успешно";
                    break;
            }
        }

        protected override IEnumerable<CommandViewModel> GetViewCommands()
        {
            yield return Ready();
        }
    }
}
