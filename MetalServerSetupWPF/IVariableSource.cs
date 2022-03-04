using System;

namespace MetalServerSetupWPF
{
    public interface IVariableSource
    {
        Version InstalledVersion { get; }
        bool IsInstalled { get; }

        string GetVariable(string name);

        void SetVariable(string name, string value);

        void Load();

        void Modify();

        void Save();
    }
}
