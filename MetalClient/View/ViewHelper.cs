using MetalClient.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MetalClient.View
{
    public static class ViewHelper
    {
        public static void SetGridEditRow(DataGrid grid, Guid id)
        {
            if (id != Guid.Empty)
            {
                var selected = grid.Items.AsList<BaseListItemDTO>().FirstOrDefault(x => x.Id == id);

                if (selected is null) return;

                Application.Current.Dispatcher.Invoke(
                    new Action(() =>
                    {
                        grid.SelectedItem = selected;

                        grid.UpdateLayout();
                        grid.ScrollIntoView(selected);
                        Keyboard.Focus(VisualComponentHelper.GetDataGridCell(grid.SelectedCells[0]));
                        grid.BeginEdit();
                    }));
            }
        }
    }
}
