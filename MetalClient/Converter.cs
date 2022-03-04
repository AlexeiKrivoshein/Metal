using MetalClient.Helper;
using MetalTransport.Datagram.Properties;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MetalClient
{
    public class DateToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;

            if (date == null ||
                date == Constants.EMPTY_DATETIME)
                return "--.--.----";

            return date.ToString("dd.MM.yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyCheckInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = (int)value;

            if (number == 0)
                return "--";

            return number;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FilterValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value is DateTime date && date == Constants.EMPTY_DATETIME)
            {
                return "";
            }

            if (value is int num && num == default)
            {
                return "";
            }

            if (value is double dbl && dbl == default)
            {
                return "";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                (value is string str && String.IsNullOrEmpty(str)))
            {
                if (targetType == typeof(DateTime))
                {
                    return Constants.EMPTY_DATETIME;
                }
                
                if (targetType == typeof(string))
                {
                    return string.Empty;
                }

                if (targetType == typeof(int))
                {
                    return default(int);
                }

                if (targetType == typeof(double))
                {
                    return default(double);
                }
            }

            return value;
        }
    }

    public class HourMinuteToString : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return "";

            var hour = values[0] as int?;
            var minute = values[1] as int?;

            if (hour == null || minute == null)
                return "--:--";

            return $"{hour:D3}:{minute:D2}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }
        #endregion
    }

    public class IsNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
                return date != null && date > Constants.EMPTY_DATETIME;

            if (value is string str)
                return !string.IsNullOrEmpty(str);

            if (value is bool logic)
                return logic;

            if (value is int number)
                return number > 0;

            throw new ArgumentException($"Неизвестный тип {value.GetType()}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EmptyToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible;
            if (value is Guid id)
            {
                visible = id == Guid.Empty;
            }
            else if (value is DateTime date)
            {
                visible = date == Constants.EMPTY_DATETIME;
            }
            else if (value is string str)
            {
                visible = string.IsNullOrEmpty(str);
            }
            else if (value is int int_num)
            {
                visible = int_num == default;
            }
            else if (value is double doub_num)
            {
                visible = doub_num == default;
            }
            else
            {
                throw new ArgumentException($"Неизвестный тип {value.GetType()}");
            }

            if (visible)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DateToDate: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime)value;

            if (date == null ||
                date == Constants.EMPTY_DATETIME)
                return null;

            return date;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Constants.EMPTY_DATETIME;
            else
                return value;
        }
    }

    public class BoolInverterConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

        #endregion
    }

    public class OrderStateLocalize: IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (OrderState)value;
            return state.Description();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("OrderStateLocalize is a OneWay converter.");
        }
        #endregion
    }

    public class RightConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;
            if (!int.TryParse(parameterString, out var param))
                return DependencyProperty.UnsetValue;

            if (value is byte set)
            {
                if (param == 0) //галка видимости
                {
                    return set > 0;
                }
                else if (param == 1) //галка редактирования
                {
                    return set > 1;
                }
                else
                    return DependencyProperty.UnsetValue;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (! int.TryParse(parameterString, out var param))
                return DependencyProperty.UnsetValue;

            if (value is bool set)
                if (set)    // галка установлена
                {
                    if (param == 0) //установлена галка видимость
                    {
                        return 1;
                    }
                    else if (param == 1) //установлена галка редактирование
                    {
                        return 2;
                    }
                    else
                        return DependencyProperty.UnsetValue;
                }
                else    // галка снята
                {
                    if (param == 0) //снята галка видимость
                    {
                        return 0;
                    }
                    else if (param == 1) //снята галка редактирование
                    {
                        return 1;
                    }
                    else
                        return DependencyProperty.UnsetValue;
                }
            else
                return 0;
        }
        #endregion
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible && isVisible)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BoolToVisibilityConverter is a OneWay converter.");
        }
        #endregion
    }

    public class BoolToCollapseConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCollapse && isCollapse)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BoolToVisibilityConverter is a OneWay converter.");
        }
        #endregion
    }

    public class GridRowVisibleConverter : ConvertorBase<GridRowVisibleConverter>
    {
        public int Position { set; private get; }

        public int StarValue { set; private get; } = 1;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Locker locker)
            {
                if (locker.Rights[Position] == 255)
                    return new GridLength(0, GridUnitType.Auto);
                else if (locker.Rights[Position] > 0)
                    return new GridLength(StarValue, GridUnitType.Star);
                else
                    return new GridLength(0);
            }
            else
            {
                return new GridLength(0);
            }
        }
    }

    public class GridRowEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return UserGroupHelper.IsEditing(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("GridRowEnabledConverter is a OneWay converter.");
        }
    }

    public class CreateOrderRight : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Locker locker)
            {
                if (locker.Rights[UserGroupHelper.MAIN_1] > 1)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class TabVisibleConverter : ConvertorBase<GridRowVisibleConverter>
    {
        public int From { set; private get; }

        public int To { set; private get; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Locker locker)
            {
                for (int index = From; index <= To; index++)
                    if (locker.Rights[index] == 0)
                        return false;

                return true;
            }
            else
                return false;
        }
    }   

    public abstract class ConvertorBase<T> : MarkupExtension, IValueConverter
     where T : class, new()
    {
        /// <summary>
        /// Must be implemented in inheritor.
        /// </summary>
        public abstract object Convert(object value, Type targetType, object parameter,
            CultureInfo culture);

        /// <summary>
        /// Override if needed.
        /// </summary>
        public virtual object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class MultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    public class EnumToCollectionConverter 
        : MarkupExtension
        , IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EnumHelper.GetAllValuesAndDescriptions(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class ErrorTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string stringValue)
            {
                return stringValue.TrimEnd();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(ErrorTextConverter)} is a OneWay converter.");
        }
    }
}
