using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalServerSetupWPF
{
    public static class EnumHelper
    {
        public static string GetDisplayName(object value)
        {
            var type = value.GetType();
            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DisplayNameAttribute), false);

            return ((DisplayNameAttribute)attributes[0]).DisplayName;
        }
    }
}
