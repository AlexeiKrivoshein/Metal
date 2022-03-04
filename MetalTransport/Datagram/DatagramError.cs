using System;

namespace MetalTransport.Datagram
{
    [Serializable]
    public class DatagramError
        : DatagramBase
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }

        public DatagramType InitialType { get; set; }

        public DatagramError(DatagramType initialType)
        {
            InitialType = initialType;
            DataType = DatagramType.Error;
        }
    }
}
