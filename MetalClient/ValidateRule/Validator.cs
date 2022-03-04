using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetalClient.ValidateRule
{
    public class Validator
    {
        public static void DoubleValid(object sender, TextCompositionEventArgs e)
        {
            if (!new Regex(@"[0-9\.]").IsMatch(e.Text))
            {
                e.Handled = true;
                return;
            }

            if (e.Text == "." &&
                sender is TextBox tb &&
                tb.Text.Contains('.'))
            {
                e.Handled = true;
                return;               
            }
        }

        public static bool DoubleValid(string value)
        {
            return new Regex(@"[0-9\.]").IsMatch(value);
        }

        public static void IntValid(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex(@"[0-9]").IsMatch(e.Text);
        }
    }
}
