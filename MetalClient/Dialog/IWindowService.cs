using MetalClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MetalClient.Dialog
{
    internal interface IWindowService
    {
        void ShowWindow<T>(IViewModel viewModel) where T : Window, new();
    }
}
