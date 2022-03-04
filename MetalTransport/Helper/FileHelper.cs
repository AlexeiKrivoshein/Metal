using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MetalTransport.Helper
{
    public static class FileHelper
    {
        public static string PrepareFileName(string value)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            value = r.Replace(value, "");

            return value;
        }
    }
}
