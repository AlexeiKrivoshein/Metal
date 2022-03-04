using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MetalServerSetupWPF.Converters
{
    public class IntToBoolenConverter 
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (! (parameter is string parameterString))
                return DependencyProperty.UnsetValue;

            if (! (value is string valueString))
                return DependencyProperty.UnsetValue;

            return parameterString == valueString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is string parameterString))
                return DependencyProperty.UnsetValue;

            return parameterString;
        }
    }
}
