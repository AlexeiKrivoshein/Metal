using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalClient
{
    internal class StoreEventArgs: EventArgs
    {
        public bool IsSucces { get; set; }
        public string Error { get; set; }
    }
}
