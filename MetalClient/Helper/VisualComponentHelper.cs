using System.Windows.Controls;

namespace MetalClient.Helper
{
    public static class VisualComponentHelper
    {
        public static DataGridCell GetDataGridCell(System.Windows.Controls.DataGridCellInfo cellInfo)
        {
            var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);

            if (cellContent != null)
                return ((DataGridCell)cellContent.Parent);

            return (null);
        }
    }
}
