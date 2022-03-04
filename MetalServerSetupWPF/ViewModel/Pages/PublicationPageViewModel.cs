using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace MetalServerSetupWPF.ViewModel.Pages
{
    public class PublicationPageViewModel
        : PageViewModel
    {
        public const string UserNameLocalSystem = "Локальная система";
        public const string UserNameLocalService = "Локальная служба";
        public const string UserNameNetworkService = "Сетевая служба";

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

        public string UserName
        {
            get => GetVariable(BootstrapperVariables.UserName);
            set => SetVariable(BootstrapperVariables.UserName, value);
        }

        public string UserPassword
        {
            get => GetVariable(BootstrapperVariables.UserPassword);
            set => SetVariable(BootstrapperVariables.UserPassword, value);
        }

        public string DrawingPath
        {
            get => GetVariable(BootstrapperVariables.DrawingPath);
            set => SetVariable(() => DrawingPath, BootstrapperVariables.DrawingPath, value);
        }

        public PublicationPageViewModel()
            : this(new VariablesSource(null, BootstrapperInstaller.BuildVersion()))
        {
        }

        public PublicationPageViewModel(IVariableSource variableSource)
            : base("Настройки", "", "Укажите настройки публикации", variableSource)
        {
            ServerName = GetVariable(BootstrapperVariables.ServerName);
            ServerPort = GetVariable(BootstrapperVariables.ServerPort);
            DrawingPath = GetVariable(BootstrapperVariables.DrawingPath);

            UserName = GetVariable(BootstrapperVariables.UserName);
            if (UserName == "LocalSystem")
                UserName = UserNameLocalSystem;
            else if (UserName == @"NT AUTHORITY\LOCAL SERVICE")
                UserName = UserNameLocalService;
            else if (UserName == @"NT AUTHORITY\NETWORK SERVICE")
                UserName = UserNameNetworkService;

            UserPassword = GetVariable(BootstrapperVariables.UserPassword);
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
            SetVariable(BootstrapperVariables.DrawingPath, DrawingPath);

            if (UserName == UserNameLocalSystem)
            {
                SetVariable(BootstrapperVariables.UserName, "LocalSystem");
                SetVariable(BootstrapperVariables.UserPassword, "");
            }
            else if (UserName == UserNameLocalService)
            {
                SetVariable(BootstrapperVariables.UserName, @"NT AUTHORITY\LOCAL SERVICE");
                SetVariable(BootstrapperVariables.UserPassword, "");
            }
            else if (UserName == UserNameNetworkService)
            {
                SetVariable(BootstrapperVariables.UserName, @"NT AUTHORITY\NETWORK SERVICE");
                SetVariable(BootstrapperVariables.UserPassword, "");
            }
            else if (!ValidateUser())
            {
                return false;
            }
            else
            {
                SetVariable(BootstrapperVariables.UserName, UserName);
                SetVariable(BootstrapperVariables.UserPassword, UserPassword);
            }


            if (!Directory.Exists(DrawingPath))
                Directory.CreateDirectory(DrawingPath);

            SetFullControlPermissions(DrawingPath);

            return true;
        }

        private bool ValidateUser()
        {
            try
            {
                if (UserName == "LocalSystem" && string.IsNullOrEmpty(UserPassword))
                    return true;

                if (UserName.Split('\\').Length != 2)
                    return false;

                using (var context = new PrincipalContext(ContextType.Machine))
                    return context.ValidateCredentials(UserName, UserPassword);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void SetFullControlPermissions(string path)
        {
            const FileSystemRights rights = FileSystemRights.FullControl;

            var allUsers = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);

            // Add Access Rule to the actual directory itself
            var accessRule = new FileSystemAccessRule(
                allUsers,
                rights,
                InheritanceFlags.None,
                PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow);

            var info = new DirectoryInfo(path);
            var security = info.GetAccessControl(AccessControlSections.Access);

            bool result;
            security.ModifyAccessRule(AccessControlModification.Set, accessRule, out result);

            if (!result)
            {
                throw new InvalidOperationException("Failed to give full-control permission to all users for path " + path);
            }

            // add inheritance
            var inheritedAccessRule = new FileSystemAccessRule(
                allUsers,
                rights,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.InheritOnly,
                AccessControlType.Allow);

            bool inheritedResult;
            security.ModifyAccessRule(AccessControlModification.Add, inheritedAccessRule, out inheritedResult);

            if (!inheritedResult)
            {
                throw new InvalidOperationException("Failed to give full-control permission inheritance to all users for " + path);
            }

            info.SetAccessControl(security);
        }
    }
}
