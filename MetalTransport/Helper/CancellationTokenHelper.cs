using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetalTransport.Helper
{
    public static class CancellationTokenHelper
    {
        public static CancellationTokenSource WithTimeout(double milliseconds)
        {
            var cts = new CancellationTokenSource();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) =>
            {
                cts.Cancel();
            };
            timer.Interval = milliseconds;
            timer.AutoReset = false;
            timer.Start();
            return cts;
        }
    }
}
