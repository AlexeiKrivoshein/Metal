using System;
using System.Windows.Input;

namespace MetalServerSetupWPF.ViewModel
{
    public class CommandViewModel
    {
        public string DisplayName { get; set; }

        public ICommand Command { get; set; }
    }
}
