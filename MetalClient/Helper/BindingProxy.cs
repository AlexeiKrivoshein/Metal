﻿using System.Windows;

namespace MetalClient.Helper
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty DataProperty =
           DependencyProperty.Register("Data", typeof(object),
              typeof(BindingProxy));

        public object Data
        {
            get { 
                return GetValue(DataProperty); 
            }
            set { 
                SetValue(DataProperty, value); 
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}
