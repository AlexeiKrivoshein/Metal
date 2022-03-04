﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalServerSetupWPF
{
    [AttributeUsage(AttributeTargets.All)]
    public class DisplayNameAttribute
        : Attribute
    {
        public string DisplayName { get; set; }

        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
