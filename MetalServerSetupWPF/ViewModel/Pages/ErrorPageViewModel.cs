using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalServerSetupWPF.ViewModel.Pages
{
    public class ErrorPageViewModel
        : PageViewModel
    {
        public ErrorPageViewModel()
            : this(new VariablesSource(null, BootstrapperInstaller.BuildVersion()))
        {

        }

        public ErrorPageViewModel(IVariableSource variableSource)
            : base("Ошибка", "", "Мастер установки завершился с ошибкой", variableSource)
        {

        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => Set(nameof(ErrorMessage), ref _errorMessage, value);
        }

        protected override IEnumerable<CommandViewModel> GetViewCommands()
        {
            yield return Ready();
        }
    }
}
