using log4net;
using MetalDiagnostic.Logger;
using MetalTransport.Helper;
using MetalTransport.ModelEx;
using System;
using System.Collections.Generic;

namespace MetalTransport.Datagram
{
    public sealed class DatagramFactory
    {
        private ILog _log = LogService.GetLogger(nameof(DatagramFactory));

        private byte[] _data;
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private DatagramType _type;
        private Guid _corelationId = Guid.Empty;

        public DatagramFactory()
        {
        }

        public DatagramFactory WithCorelationId(Guid id)
        {
            _corelationId = id;
            return this;
        }
        
        public DatagramFactory WithDTOObject<T>(T value) where T : BaseDTO
        {
            if (!typeof(T).IsSerializable)
                throw new InvalidOperationException("A serializable Type is required");

            try
            {
                _data = SerializationHelper.Serialize(value, out var content);
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка сериализации {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                throw;
            }

            return this;
        }

        public DatagramFactory WithProperty(string name, string value)
        {
            _properties[name] = value;
            return this;
        }

        public DatagramFactory WithProperty(string name, Guid value)
        {
            _properties[name] = value.ToString();
            return this;
        }

        public DatagramFactory WithProperty(string name, int value)
        {
            _properties[name] = value.ToString();
            return this;
        }

        public DatagramFactory WithProperty(string name, long value)
        {
            _properties[name] = value.ToString();
            return this;
        }

        public DatagramFactory WithProperty(Dictionary<string, string> properties)
        {
            _properties.Clear();

            foreach (var property in properties)
                _properties.Add(property.Key, property.Value);
            
            return this;
        }

        public DatagramFactory WithType(DatagramType type)
        {
            _type = type;

            return this;
        }

        public DatagramBase Build()
        {
            return new DatagramBase(Guid.NewGuid(), _type, _data, _properties)
            {
                CorelationId = _corelationId
            };

        }
    }
}
