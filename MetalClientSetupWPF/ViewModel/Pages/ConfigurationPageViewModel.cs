using System;
using System.Collections.Generic;

namespace MetalClientSetupWPF.ViewModel.Pages
{
    public class ConfigurationPageViewModel
        : PageViewModel
    {
        public string ServerName
        {
            get => GetVariable(BootstrapperVariables.ServerName);
            set => SetVariable(BootstrapperVariables.ServerName, value);
        }

        public string ServerPort
        {
            get => GetVariable(BootstrapperVariables.ServerPort);
            set => SetVariable(BootstrapperVariables.ServerPort, value);
        }

        public ConfigurationPageViewModel()
            : this(new VariablesSource(null, null))
        {
        }

        public ConfigurationPageViewModel(IVariableSource variableSource)
            : base("Настройки", "", "Укажите настройки", variableSource)
        {
            ServerName = GetVariable(BootstrapperVariables.ServerName);
            ServerPort = GetVariable(BootstrapperVariables.ServerPort);
        }

        protected override IEnumerable<CommandViewModel> GetViewCommands()
        {
            yield return Next();
            yield return Cancel();
        }

        protected override bool OnNext()
        {
            SetVariable(BootstrapperVariables.ServerName, ServerName);
            SetVariable(BootstrapperVariables.ServerPort, ServerPort);

            return true;
        }
    }
}
