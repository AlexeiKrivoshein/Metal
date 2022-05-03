using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetalClient.Controls
{
    /// <summary>
    /// Interaction logic for MTextBox.xaml
    /// </summary>
    public partial class MTextBox : UserControl
    {
        //тип данных для поля ввода
        public static readonly DependencyProperty FormatProperty;
        public TextBoxFormats Format
        {
            get { return (TextBoxFormats)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        //максимальная длинна строки
        public static readonly DependencyProperty MaxLengthProperty;
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        //команда потери фокуса(обработка изменнных данных)
        public static readonly DependencyProperty LostFocusCommandProperty;

        public RelayCommand LostFocusCommand
        {
            get { return (RelayCommand)GetValue(LostFocusCommandProperty); }
            set { SetValue(LostFocusCommandProperty, value); }
        }

        //строка контрола
        public static readonly DependencyProperty TextProperty;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public HorizontalAlignment TextAlignment 
        {
            get => (Format == TextBoxFormats.Currency || Format == TextBoxFormats.Number) ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        //валидаторы в зависимости от формата данных
        private static TextCompositionEventHandler _doubleValid = new TextCompositionEventHandler((object sender, TextCompositionEventArgs e) =>
        {
            e.Handled = !new Regex(@"[0-9\.]").IsMatch(e.Text);
        });

        private static TextCompositionEventHandler _intValid = new TextCompositionEventHandler((object sender, TextCompositionEventArgs e) =>
        {
            e.Handled = !new Regex(@"[0-9]").IsMatch(e.Text);
        });

        private static TextCompositionEventHandler _noneValid = new TextCompositionEventHandler((object sender, TextCompositionEventArgs e) => {});

        public void PreviewInput(object sender, TextCompositionEventArgs e)
        {
            var validate = Format == TextBoxFormats.Currency ? _doubleValid : 
                           Format == TextBoxFormats.Number ? _intValid : 
                           _noneValid;

            validate(sender, e);
        }

        static MTextBox()
        {
            // Регистрация свойств зависимости
            FormatProperty = DependencyProperty.Register(
                nameof(Format),
                typeof(TextBoxFormats),
                typeof(MTextBox));

            LostFocusCommandProperty = DependencyProperty.Register(
                nameof(LostFocusCommand),
                typeof(RelayCommand),
                typeof(MTextBox));

            MaxLengthProperty = DependencyProperty.Register(
                nameof(MaxLength),
                typeof(int),
                typeof(MTextBox));

            TextProperty = DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(MTextBox));
        }

        public MTextBox()
        {
            InitializeComponent();
        }

        public void OnLostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (LostFocusCommand != null)
            {
                LostFocusCommand.Execute(null);
            }
        }
    }
}
