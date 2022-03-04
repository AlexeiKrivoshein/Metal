using MetalTransport.Datagram;
using MetalTransport.ModelEx;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MetalTransport.Helper
{
    public static class SerializationHelper
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Serialize };

        public static byte[] Serialize<T>(T value, out string content) 
            where T: BaseDTO
        {
            if (!typeof(T).IsSerializable)
                throw new InvalidOperationException("A serializable Type is required");

            try
            {
                content = JsonConvert.SerializeObject(value);
                return Encoding.UTF8.GetBytes(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static T Deserialize<T>(byte[] value, out string content)
            where T : BaseDTO
        {
            if (!typeof(T).IsSerializable)
                throw new InvalidOperationException("A serializable Type is required");

            try
            {
                content = Encoding.UTF8.GetString(value);
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
