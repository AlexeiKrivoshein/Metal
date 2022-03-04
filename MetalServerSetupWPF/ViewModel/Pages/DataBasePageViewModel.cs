using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace MetalServerSetupWPF.ViewModel.Pages
{
    public class DataBasePageViewModel
        : PageViewModel
    {
        public string SQLServerName
        {
            get => GetVariable(BootstrapperVariables.SQLServerName);
            set => SetVariable(BootstrapperVariables.SQLServerName, value);
        }

        public string SQLUserName
        {
            get => GetVariable(BootstrapperVariables.SQLUserName);
            set => SetVariable(BootstrapperVariables.SQLUserName, value);
        }

        public string SQLUserPassword
        {
            get => GetVariable(BootstrapperVariables.SQLUserPassword);
            set => SetVariable(BootstrapperVariables.SQLUserPassword, value);
        }

        public string SQLIntegrated
        {
            get => GetVariable(BootstrapperVariables.SQLIntegrated);
            set => SetVariable(BootstrapperVariables.SQLIntegrated, value);
        }

        public string SQLConnectionString
        {
            get => GetVariable(BootstrapperVariables.SQLConnectionString);
            set => SetVariable(BootstrapperVariables.SQLConnectionString, value);
        }

        public DataBasePageViewModel()
            : this(new VariablesSource(null, BootstrapperInstaller.BuildVersion()))
        {
        }

        public DataBasePageViewModel(IVariableSource variableSource)
            : base("Настройки", "", "Укажите настройки БД", variableSource)
        {
            SQLIntegrated = GetVariable(BootstrapperVariables.SQLIntegrated);
            SQLServerName = GetVariable(BootstrapperVariables.SQLServerName);
            SQLUserName = GetVariable(BootstrapperVariables.SQLUserName);
            SQLUserPassword = GetVariable(BootstrapperVariables.SQLUserPassword);
            SQLConnectionString = GetVariable(BootstrapperVariables.SQLConnectionString);
        }

        protected override IEnumerable<CommandViewModel> GetViewCommands()
        {
            yield return Next();
            yield return Cancel();
        }

        protected override bool OnNext()
        {
            SetVariable(BootstrapperVariables.SQLIntegrated, SQLIntegrated);
            SetVariable(BootstrapperVariables.SQLServerName, SQLServerName);
            SetVariable(BootstrapperVariables.SQLUserName, SQLUserName);
            SetVariable(BootstrapperVariables.SQLUserPassword, SQLUserPassword);

            string connectionString;
            if (SQLIntegrated == nameof(SQLIntegratedMode.SQLServer))
                connectionString = $@"{SQLServerName};initial catalog=Metal;persist security info=True;user id={SQLUserName};password={SQLUserPassword}";
            else
                connectionString = $@"{SQLServerName};initial catalog=Metal;integrated security=True";


            var connection = "metadata=res://*/Model.MetalEDM.csdl|res://*/Model.MetalEDM.ssdl|res://*/Model.MetalEDM.msl;provider=System.Data.SqlClient;provider connection string=\";data source=" + connectionString + ";MultipleActiveResultSets=True;App=EntityFramework\";";

            SetVariable(BootstrapperVariables.SQLConnectionString, connection);

            return true;
        }
  
    }

    public enum SQLIntegratedMode
    {
        [DisplayName("Windows")]
        Windows,

        [DisplayName("SQL server")]
        SQLServer,
    }
}
