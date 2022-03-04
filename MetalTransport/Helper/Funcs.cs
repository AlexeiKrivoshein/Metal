using MetalTransport.Datagram.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.Helper
{
    public static class Funcs
    {
        public static string GetInnerExceptions(Exception exception, string previous = "")
        {
            var error = new StringBuilder();

            if (previous != exception.Message)
                error.Append($"{exception.Message}{Environment.NewLine}");

            if (exception is AggregateException agregate)
            {
                if (agregate.InnerExceptions != null && agregate.InnerExceptions.Any())
                {
                    foreach (var inner in agregate.InnerExceptions)
                    {
                        error.Append(GetInnerExceptions(inner, exception.Message));
                    }
                }
            }
            else if (exception.InnerException != null)
            {
                error.Append(GetInnerExceptions(exception.InnerException, exception.Message));
            }

            return error.ToString();
        }

        public static string GetShortName(string secondname, string name, string patronymic)
        {
            return (secondname +
                   (name.Length > 0 ? " " + name.Substring(0, 1) : "") +
                   (patronymic.Length > 0 ? " " + patronymic.Substring(0, 1) : "")).Trim();
        }

        public static string GetFullName(string secondname, string name, string patronymic)
        {
            return (secondname +
                   (name.Length > 0 ? " " + name : "") +
                   (patronymic.Length > 0 ? " " + patronymic : "")).Trim();
        }
    }
}
