using log4net.Core;
using MetalTransport.Datagram.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalClient.Helper
{
    public static class Funcs
    {
        public static bool DateIsEmpty(DateTime value)
        {
            return value == null
                || value == DateTime.MinValue
                || value == Constants.EMPTY_DATETIME;

        }

        public static DateTime FillHour(string hour, DateTime date)
        {
            if (!string.IsNullOrEmpty(hour) &&
                int.TryParse(hour, out var sel_hour))
            {
                return new DateTime(date.Year, date.Month, date.Day, sel_hour, date.Minute, date.Second);
            }
            else
                return date;
        }

        public static DateTime FillDateTime(DateTime date, string hour, string minute)
        {
            if (!string.IsNullOrEmpty(hour) && int.TryParse(hour, out var sel_hour) &&
                !string.IsNullOrEmpty(minute) && int.TryParse(minute, out var sel_minute))
            {
                return new DateTime(date.Year, date.Month, date.Day, sel_hour > 23 ? 23 : sel_hour, sel_minute > 59 ? 59 : sel_minute, date.Second > 59 ? 59 : date.Second);
            }
            else
            {
                return date;
            }
        }

        public static string GetMonthName(int month)
        {
            var monthName = "";
            switch (month)
            {
                case 1:
                    monthName = "январь";
                    break;
                case 2:
                    monthName = "февраль";
                    break;
                case 3:
                    monthName = "март";
                    break;
                case 4:
                    monthName = "апрель";
                    break;
                case 5:
                    monthName = "май";
                    break;
                case 6:
                    monthName = "июнь";
                    break;
                case 7:
                    monthName = "июль";
                    break;
                case 8:
                    monthName = "август";
                    break;
                case 9:
                    monthName = "сентябрь";
                    break;
                case 10:
                    monthName = "октябрь";
                    break;
                case 11:
                    monthName = "ноябрь";
                    break;
                case 12:
                    monthName = "декабрь";
                    break;
            }

            return monthName;
        }

        public static DateTime GetDateTimeValue(object value)
        {
            bool isSet = false;

            switch (value)
            {
                case bool bValue:
                    isSet = bValue;
                    break;
                case string sValue:
                    isSet = !string.IsNullOrEmpty(sValue);
                    break;
                case float fValue:
                    isSet = fValue > 0;
                    break;
                case int iValue:
                    isSet = iValue > 0;
                    break;
                case double dValue:
                    isSet = dValue > 0;
                    break;
            }

            if (isSet)
            {
                return DateTime.Now;
            }
            else
            {
                return Constants.EMPTY_DATETIME;
            }
        }
    }
}
