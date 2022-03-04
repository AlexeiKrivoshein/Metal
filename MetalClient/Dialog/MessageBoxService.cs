using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MetalClient.Dialog
{
    internal class MessageBoxService : IMessageBoxService
    {
        private Window _owner;

        public MessageBoxService(Window owner)
        {
            _owner = owner;
        }

        public MessageBoxResult Show(string caption, string message, MessageBoxImage image, MessageBoxButton button)
        {
            return MetalMessageBox.Show(caption, message, image, button, _owner);
        }
    }
}
