using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.ModelEx
{
    public class CheckedElementName
    {
        public bool Checked { get; set; }

        public BaseListItemDTO Element { get; set; }

        public CheckedElementName(BaseListItemDTO element)
        {
            Element = element;
        }
    }
}
