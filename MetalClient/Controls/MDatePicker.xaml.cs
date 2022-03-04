using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace MetalClient.Controls
{
    /// <summary>
    /// Interaction logic for MDatePicker.xaml
    /// </summary>
    public partial class MDatePicker : UserControl
    {
        //источник данных
        public static readonly DependencyProperty DateProperty;
        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        //границы
        public static readonly DependencyProperty BorderProperty;
        public Thickness Border
        {
            get { return (Thickness)GetValue(BorderProperty); }
            set { SetValue(BorderProperty, value); }
        }

        //границы
        public static readonly DependencyProperty IsTimePickerVisibilityProperty;
        public bool IsTimePickerVisibility
        {
            get { return (bool)GetValue(IsTimePickerVisibilityProperty); }
            set { SetValue(IsTimePickerVisibilityProperty, value); }
        }

        private RelayCommand showCalendarCommand;
        public RelayCommand ShowCalendarCommand
        {
            get
            {
                return showCalendarCommand ??
                  (showCalendarCommand = new RelayCommand(obj =>
                  {
                      calendar.IsOpen = true;
                  }));
            }
        }

        public string FormatString => IsTimePickerVisibility ? "dd.MM.yyyy HH:mm" : "dd.MM.yyyy";

        static MDatePicker()
        {
            DateProperty = DependencyProperty.Register(
                nameof(Date),
                typeof(DateTime),
                typeof(MDatePicker));

            BorderProperty = DependencyProperty.Register(
                nameof(Border),
                typeof(Thickness),
                typeof(MDatePicker),
                new PropertyMetadata(new Thickness(1)));

            IsTimePickerVisibilityProperty = DependencyProperty.Register(
                nameof(IsTimePickerVisibility),
                typeof(bool),
                typeof(MDatePicker),
                new PropertyMetadata(false));
        }

        public MDatePicker()
        {
            InitializeComponent();
        }

        public void ShowCalendar(object sender, MouseButtonEventArgs e)
        {
            calendar.IsOpen = true;
        }
    }
}
