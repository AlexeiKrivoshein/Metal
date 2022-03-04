using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MetalTransport.Datagram
{
    [Serializable]
    public class DatagramBase
        : BaseDTO
    {
        public DatagramType DataType { get; set; }
        public byte[] Data { get; set; }

        public Guid CorelationId { get; set; } = Guid.Empty;

        public static DatagramBase Response = new DatagramBase(Guid.Empty, DatagramType.Response, new byte[0]);

        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        public DatagramBase()
        {
            DataType = DatagramType.NONE;
            Data = new byte[0];
            Id = Guid.NewGuid();
        }

        public DatagramBase(DatagramType type, byte[] data, Dictionary<string, string> properties = null)
        {
            DataType = type;
            Data = data;
            Id = Guid.NewGuid();
            Properties = properties;
        }

        public DatagramBase(Guid id, DatagramType type, byte[] data, Dictionary<string, string>  properties = null)
        {
            DataType = type;
            Data = data;
            Id = id;
            Properties = properties;
        }

        public override string ToString()
        {
            return $"[{DataType}, id:{Id}, corelation:{CorelationId}]";
        }
    }
}
