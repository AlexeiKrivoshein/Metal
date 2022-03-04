using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetalTransport.Datagram.Properties
{
    public static partial class Constants
    {
        public static DateTime EMPTY_DATETIME = new DateTime(1800, 1, 1, 0, 0, 0);

        public const int CHUNK_SIZE = 255 * 1024;

        public const int MAX_FILE_SIZE_MB = 10;

        public const int DEFAULT_PAGE_ELEMENT_COUNT = 100;

        public const int ORDERS_REFRESH_TIMEOUT_MS = 10_000;

        public const int DEFAULT_TIMEOUT = 10_000;
    }
}
