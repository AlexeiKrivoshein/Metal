using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MetalClient.Dialog
{
    internal interface IMessageBoxService
    {
        MessageBoxResult Show(string caption, string message, MessageBoxImage image, MessageBoxButton button);
    }
}
