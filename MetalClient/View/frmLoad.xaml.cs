using MetalClient.ViewModel;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MetalClient.View
{
    /// <summary>
    /// Interaction logic for frmLoad.xaml
    /// </summary>
    public partial class frmLoad 
        : Window
    {
        private CancellationTokenSource _cts;
        private LoadViewModel _viewModel;
        
        public frmLoad(LoadViewModel viewModel)
        {
            InitializeComponent();

            _cts = new CancellationTokenSource();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            _viewModel.Load(_cts.Token).ContinueWith(t => 
            {
                HideForm();
            });
        }

        private void HideForm()
        {
            Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() => base.Hide()));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }
    }
}
