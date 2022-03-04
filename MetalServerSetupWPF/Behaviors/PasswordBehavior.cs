using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MetalServerSetupWPF.Behaviors
{
    public class PasswordBehavior : Behavior<PasswordBox>
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("UserPassword", typeof(string), typeof(PasswordBehavior), new PropertyMetadata(default(string)));

        private bool _skipUpdate;

        public string UserPassword
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.PasswordChanged += PasswordBox_PasswordChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= PasswordBox_PasswordChanged;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == PasswordProperty)
            {
                if (!_skipUpdate)
                {
                    if (e.NewValue != null)
                    {
                        _skipUpdate = true;
                        AssociatedObject.Password = e.NewValue as string ?? throw new InvalidOperationException();
                        _skipUpdate = false;
                    }
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _skipUpdate = true;
            UserPassword = AssociatedObject.Password;
            _skipUpdate = false;
        }
    }
}
