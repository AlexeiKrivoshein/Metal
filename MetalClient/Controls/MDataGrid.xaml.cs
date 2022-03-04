using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    /// Interaction logic for MDataGrid.xaml
    /// </summary>
    public partial class MDataGrid: UserControl
    {
        //количетсво строк до конца подгруженных данных когда начнется загрузка новой страницы
        private const int PRELOAD_ROW_COUNT = 4;

        //источник данных
        public static readonly DependencyProperty ItemsSourceProperty;
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); } 
        }

        //выбранная строка. связано с моделью
        public static readonly DependencyProperty SelectedItemProperty;
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        //команда выбора строки (двойной клик или enter)
        public static readonly DependencyProperty SelectCommandProperty;

        public RelayCommand SelectCommand
        {
            get { return (RelayCommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        // событие прокрутки таблицы
        public event ScrollChangedEventHandler ScrollChanged;
        
        //структура колонок
        public List<DataGridColumn> Columns { get; } = new List<DataGridColumn>();

        //структура футера
        public List<MDataGridFooterItem> FooterItems { get; } = new List<MDataGridFooterItem>();

        static MDataGrid()
        {
            // Регистрация свойств зависимости
            ItemsSourceProperty = DependencyProperty.Register(
                nameof(ItemsSource), 
                typeof(IEnumerable), 
                typeof(MDataGrid));

            SelectedItemProperty = DependencyProperty.Register(
                nameof(SelectedItem), 
                typeof(object), 
                typeof(MDataGrid));

            SelectCommandProperty = DependencyProperty.Register(
                nameof(SelectCommand), 
                typeof(RelayCommand), 
                typeof(MDataGrid));
        }

        public MDataGrid()
        {
            InitializeComponent();
        }

        public override void EndInit()
        {
            base.EndInit();

            if (grid.Columns.Any()) grid.Columns.Clear();

            if (footer.ColumnDefinitions.Any()) footer.ColumnDefinitions.Clear();

            var index = 0;
            foreach (var column in Columns)
            {
                var name = $"collumn_{index++}";
                grid.Columns.Add(column);
                RegisterName(name, column);

                var widthBinding = new Binding()
                {
                    ElementName = name,
                    Path = new PropertyPath(nameof(DataGridColumn.ActualWidth), null)
                };

                var colDefinition = new ColumnDefinition();
                colDefinition.SetBinding(ColumnDefinition.WidthProperty, widthBinding);

                footer.ColumnDefinitions.Add(colDefinition);
            }

            footer.Children.Clear();

            if (!FooterItems.Any())
            {
                footer.Visibility = Visibility.Collapsed;
            }
            else
            {
                footer.Visibility = Visibility.Visible;

                foreach (var footerItem in FooterItems)
                {
                    footerItem.HorizontalAlignment = HorizontalAlignment.Right;

                    Grid.SetColumn(footerItem, footerItem.Position);
                    Grid.SetRow(footerItem, 0);

                    footer.Children.Add(footerItem);
                }
            }
        }

        private void GridUnloaded(object sender, RoutedEventArgs e)
        {
            grid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        private void DoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (SelectCommand != null && SelectCommand.CanExecute(null))
            {
                SelectCommand.Execute(SelectedItem);
            }
        }

        private void PreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SelectCommand != null && SelectCommand.CanExecute(null))
            {
                SelectCommand.Execute(null);
                e.Handled = true;
            }
        }

        private void SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (grid.SelectedItem != null)
            {
                grid.ScrollIntoView(grid.SelectedItem);
            }
        }

        public void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ScrollChanged != null)
            {
                ScrollChanged.BeginInvoke(sender, e, null, null);
            }
        }
    }
}
