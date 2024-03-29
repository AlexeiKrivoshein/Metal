﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.ComponentModel;

namespace MetalClient
{
    public class EnumToItemsSource 
        : MarkupExtension
    {
        private readonly Type _type;

        public EnumToItemsSource(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _type.GetMembers().SelectMany(member => 
            member.GetCustomAttributes(typeof(DescriptionAttribute), true).
            Cast<DescriptionAttribute>()).Select(x => x.Description).
            ToList();
        }
    }
}
