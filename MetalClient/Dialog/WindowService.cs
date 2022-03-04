using MetalClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MetalClient.Dialog
{
    internal class WindowService
        : IWindowService
    {
        private IViewModel _viewModel;
        private Window _owner;

        public WindowService(Window owner)
        {
            _owner = owner;
        }
        

        public void ShowWindow<T>(IViewModel viewModel) where T : Window, new()
        {
            _viewModel = viewModel;

            var window = new T();
            window.DataContext = _viewModel;
            if (_owner != null)
                window.Owner = _owner;
            window.ShowDialog();
        }
    }
}
