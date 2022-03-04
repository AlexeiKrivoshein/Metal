using MetalTransport.Datagram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalClientSetupWPF.ViewModel.Pages
{
    public class WelcomePageViewModel
        : PageViewModel
    {
        private Version _coreVersion;

        public WelcomePageViewModel(IVariableSource variableSource)
            : this(variableSource, typeof(DatagramBase).Assembly.GetName().Version)
        {
        }

        public WelcomePageViewModel(IVariableSource variableSource, Version coreVersion)
            : base("Приветствие", "", "Вас приветствует мастер установки", variableSource)
        {
            _coreVersion = coreVersion;
        }

        protected override IEnumerable<CommandViewModel> GetViewCommands()
        {
            if (IsInstalled)
            {
                yield return Uninstall();
            }
            else
            {
                yield return NextVersion(_coreVersion);
            }

            yield return Cancel();
        }
    }
}
