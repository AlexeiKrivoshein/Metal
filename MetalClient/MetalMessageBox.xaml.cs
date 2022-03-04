using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
//using System.Drawing;

namespace MetalClient
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MetalMessageBox : Window, INotifyPropertyChanged
    {
        private string _message;
        public string Message
        {
            get => _message;
            private set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        public string _caption;
        public string Caption
        {
            get => _caption;
            private set
            {
                _caption = value;
                OnPropertyChanged("Caption");
            }
        }

        private MessageBoxResult _result;
        public MessageBoxResult Result
        {
            get => _result;
            private set
            {
                _result = value;
                OnPropertyChanged("Result");
            }
        }

        private MessageBoxButton _button;

        private MetalMessageBox(string caption, string message, MessageBoxImage image, MessageBoxButton button, Window owner = null)
        {
            Owner = owner?? Application.Current.MainWindow;
            Message = message;
            Caption = caption;
            _button = button;

            InitializeComponent();

            //картинка
            Bitmap bit = null;
            switch (image)
            {
                case MessageBoxImage.None:
                    break;
                case MessageBoxImage.Error:
                    bit = Properties.Resources.error;
                    break;
                case MessageBoxImage.Question:
                    bit = Properties.Resources.info;
                    break;
                case MessageBoxImage.Warning:
                    bit = Properties.Resources.warning;
                    break;
                case MessageBoxImage.Information:
                    bit = Properties.Resources.info;
                    break;
                default:
                    break;
            }

            if (bit != null)
            {
                using (var memory = new MemoryStream())
                {
                    bit.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    var Icon = new BitmapImage();
                    Icon.BeginInit();
                    Icon.StreamSource = memory;
                    Icon.CacheOption = BitmapCacheOption.OnLoad;
                    Icon.EndInit();
                    Icon.Freeze();

                    Pic.Source = Icon;
                }

                Pic.Visibility = Visibility.Visible;
            }
            else
            {
                Pic.Visibility = Visibility.Collapsed;
            }

            btnOk.Visibility = Visibility.Collapsed;
            btnYes.Visibility = Visibility.Collapsed;
            btnNo.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Collapsed;

            //кнопки
            switch (_button)
            {
                case MessageBoxButton.OK:
                    btnOk.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    {
                        btnOk.Visibility = Visibility.Visible;
                        btnCancel.Visibility = Visibility.Visible;
                    }
                    break;
                case MessageBoxButton.YesNoCancel:
                    {
                        btnYes.Visibility = Visibility.Visible;
                        btnNo.Visibility = Visibility.Visible;
                        btnCancel.Visibility = Visibility.Visible;
                    }
                    break;
                case MessageBoxButton.YesNo:
                    {
                        btnYes.Visibility = Visibility.Visible;
                        btnNo.Visibility = Visibility.Visible;
                    }
                    break;
                default:
                    throw new ArgumentException($"Не указан тип кнопок {nameof(MessageBoxButton)}");
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Hide();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Hide();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Hide();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Hide();
        }

        public static MessageBoxResult Show(string caption, string message, MessageBoxImage image, MessageBoxButton button, Window owner = null)
        {
            var result = MessageBoxResult.None;

            Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                        {
                            var box = new MetalMessageBox(caption, message, image, button, owner);
                            box.ShowDialog();

                            result = box.Result;
                        })).Wait();

            return result;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_button == MessageBoxButton.OK)
                OK_Click(null, null);
            else if (_button == MessageBoxButton.OKCancel || _button == MessageBoxButton.YesNoCancel)
                Cancel_Click(null, null);
            else if (_button == MessageBoxButton.YesNo)
                No_Click(null, null);
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
