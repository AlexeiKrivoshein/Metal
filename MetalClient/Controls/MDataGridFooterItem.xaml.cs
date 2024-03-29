﻿using System;
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

namespace MetalClient.Controls
{
    /// <summary>
    /// Interaction logic for MDataGridFooterItem.xaml
    /// </summary>
    public partial class MDataGridFooterItem : UserControl
    {
        public int Position { get; set; }

        //Текст
        public static readonly DependencyProperty TextProperty;
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        static MDataGridFooterItem()
        {
            TextProperty = DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(MDataGridFooterItem));
        }

        public MDataGridFooterItem()
        {
            InitializeComponent();
        }
    }
}
