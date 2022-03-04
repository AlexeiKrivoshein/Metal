using MetalServerSetupWPF.ViewModel.Pages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetalServerSetupWPF.View.Page
{
    /// <summary>
    /// Логика взаимодействия для PublicationPage.xaml
    /// </summary>
    public partial class PublicationPage : System.Windows.Controls.UserControl
    {
        public PublicationPage()
        {
            InitializeComponent();
        }

        private void Drawing_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (PublicationPageViewModel)DataContext;

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = viewModel.DrawingPath;
                dialog.Description = "Выберите папку для хранения файлов";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    viewModel.DrawingPath = dialog.SelectedPath;
                }
            }
        }
    }
}
