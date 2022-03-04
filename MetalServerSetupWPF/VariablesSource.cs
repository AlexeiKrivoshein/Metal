﻿using MetalServerSetupWPF.ViewModel.Pages;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MetalServerSetupWPF
{
    public class VariablesSource
        : IVariableSource
    {
        private const string RegistryKeyMachine = @"HKEY_LOCAL_MACHINE\SOFTWARE\Metal\Server";
        private const string RegistryKeyUser = @"HKEY_CURRENT_USER\SOFTWARE\Metal\Server";

        private readonly Engine _engine;

        private readonly Version _coreVersion;
        private Version _installedVersion;

        private readonly Dictionary<string, string> _modifyDictionary = new Dictionary<string, string>();

        public VariablesSource(Engine engine, Version coreVersion)
        {
            _engine = engine;
            _coreVersion = coreVersion;
        }

        public Version InstalledVersion => _installedVersion ?? new Version();

        public bool IsInstalled => _installedVersion == _coreVersion;
        
        public string GetVariable(string name) => _engine.StringVariables[name];

        public void SetVariable(string name, string value) => _engine.StringVariables[name] = value;

        public void Load()
        {
            _ = Version.TryParse(ReadString(BootstrapperVariables.Version), out _installedVersion);
            SetVariable(BootstrapperVariables.ServerName, ReadString(BootstrapperVariables.ServerName, "127.0.0.1"));
            SetVariable(BootstrapperVariables.ServerPort, ReadString(BootstrapperVariables.ServerPort, "9500"));
            SetVariable(BootstrapperVariables.UserName, ReadString(BootstrapperVariables.UserName, PublicationPageViewModel.UserNameLocalSystem));
            SetVariable(BootstrapperVariables.UserPassword, ReadString(BootstrapperVariables.UserPassword, ""));
            SetVariable(BootstrapperVariables.DrawingPath, ReadString(BootstrapperVariables.DrawingPath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\MetalServer\File"));

            SetVariable(BootstrapperVariables.SQLIntegrated, ReadString(BootstrapperVariables.SQLIntegrated, "1"));
            SetVariable(BootstrapperVariables.SQLServerName, ReadString(BootstrapperVariables.SQLServerName, "(local)\\SQLEXPRESS"));
            SetVariable(BootstrapperVariables.SQLUserName, ReadString(BootstrapperVariables.SQLUserName, ""));
            SetVariable(BootstrapperVariables.SQLUserPassword, ReadString(BootstrapperVariables.SQLUserPassword, ""));
        }

        public void Save()
        {
            SaveString(BootstrapperVariables.ServerName, GetVariable(BootstrapperVariables.ServerName));
            SaveString(BootstrapperVariables.ServerPort, GetVariable(BootstrapperVariables.ServerPort));
            SaveString(BootstrapperVariables.UserName, GetVariable(BootstrapperVariables.UserName));
            SaveString(BootstrapperVariables.UserPassword, GetVariable(BootstrapperVariables.UserPassword));
            SaveString(BootstrapperVariables.DrawingPath, GetVariable(BootstrapperVariables.DrawingPath));

            SaveString(BootstrapperVariables.SQLIntegrated, GetVariable(BootstrapperVariables.SQLIntegrated));
            SaveString(BootstrapperVariables.SQLServerName, GetVariable(BootstrapperVariables.SQLServerName));
            SaveString(BootstrapperVariables.SQLUserName, GetVariable(BootstrapperVariables.SQLUserName));
            SaveString(BootstrapperVariables.SQLUserPassword, GetVariable(BootstrapperVariables.SQLUserPassword));

            //SaveString(BootstrapperVariables.Version, _coreVersion.ToString());
        }

        public void Modify()
        {
            SetVariable(BootstrapperVariables.MetalServerModify, !IsInstalled ? "1" : string.Empty);

            SaveModify(BootstrapperVariables.ServerName, GetVariable(BootstrapperVariables.ServerName), BootstrapperVariables.MetalServerModify);
            SaveModify(BootstrapperVariables.ServerPort, GetVariable(BootstrapperVariables.ServerPort), BootstrapperVariables.MetalServerModify);
            SaveModify(BootstrapperVariables.UserName, GetVariable(BootstrapperVariables.UserName), BootstrapperVariables.MetalServerModify);
            SaveModify(BootstrapperVariables.UserPassword, GetVariable(BootstrapperVariables.UserPassword), BootstrapperVariables.MetalServerModify);
            SaveModify(BootstrapperVariables.DrawingPath, GetVariable(BootstrapperVariables.DrawingPath), BootstrapperVariables.MetalServerModify);

            SaveModify(BootstrapperVariables.SQLIntegrated, GetVariable(BootstrapperVariables.SQLIntegrated), BootstrapperVariables.MetalServerModify);
            SaveModify(BootstrapperVariables.SQLServerName, GetVariable(BootstrapperVariables.SQLServerName), BootstrapperVariables.MetalServerModify);
            SaveModify(BootstrapperVariables.SQLUserName, GetVariable(BootstrapperVariables.SQLUserName), BootstrapperVariables.MetalServerModify);
            SaveModify(BootstrapperVariables.SQLUserPassword, GetVariable(BootstrapperVariables.SQLUserPassword), BootstrapperVariables.MetalServerModify);
        }

        private string ReadString(string name, string defaultValue = null)
        {
            try
            {
                return (_modifyDictionary[name] =
                    (string)Registry.GetValue(RegistryKeyMachine, name, null) ??
                    (string)Registry.GetValue(RegistryKeyUser, name, null)) ?? defaultValue;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());

                return defaultValue;
            }
        }

        private void SaveString(string name, string value)
        {
            try
            {
                Registry.SetValue(RegistryKeyMachine, name, value);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }

            try
            {
                Registry.SetValue(RegistryKeyUser, name, value);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }
        }

        private void SaveModify(string name, string value, string modifyName)
        {
            if (_modifyDictionary.TryGetValue(name, out var modifyValue) && !Equals(value, modifyValue))
            {
                SetVariable(modifyName, "1");
            }
        }
    }
}
