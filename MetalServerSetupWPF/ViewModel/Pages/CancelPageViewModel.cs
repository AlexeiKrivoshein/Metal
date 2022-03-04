using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalServerSetupWPF.ViewModel.Pages
{
    public class CancelPageViewModel
        : PageViewModel
    {
        public CancelPageViewModel()
            : this(new VariablesSource(null, BootstrapperInstaller.BuildVersion()))
        {

        }

        public CancelPageViewModel(IVariableSource variableSource)
            : base("Отмена", "", "Мастер установки отменен пользователем", variableSource)
        {

        }

        public static string TextCancelPage
        {
            get
            {
                string textCancelPage = "";
                switch (MainViewModel.Action)
                {
                    case LaunchAction.Install:
                        textCancelPage = $"Мастер установки не смог установить Metal server на компьютере.\nУстановка была прервана пользователем.";
                        break;

                    case LaunchAction.Uninstall:
                        textCancelPage = $"Мастер установки не смог удалить Metal server на компьютере.\nУдаление было прервано пользователем.";
                        break;
                }
                return textCancelPage;
            }
        }

        protected override IEnumerable<CommandViewModel> GetViewCommands()
        {
            yield return Ready();
        }
    }
}
