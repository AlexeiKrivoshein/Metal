using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace MetalServerSetupWPF
{
    public class EnumConverter
        : MarkupExtension
        , IValueConverter
    {

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            foreach (var one in Enum.GetValues(parameter as Type ?? throw new InvalidOperationException()))
            {
                if (value.ToString().ToUpper() == EnumHelper.GetDisplayName(one).Replace(" ", "").ToUpper())
                    return EnumHelper.GetDisplayName(one);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            foreach (var one in Enum.GetValues(parameter as Type ?? throw new InvalidOperationException()))
            {
                if (value.ToString() == EnumHelper.GetDisplayName(one))
                {
                    return one;
                }
            }
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
