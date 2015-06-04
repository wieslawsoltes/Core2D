using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;

namespace TestEDITOR
{
    /// <summary>
    /// 
    /// </summary>
    public class NewtonsoftSerializer : ISerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ToJson<T>(T value)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ProjectContractResolver()
            };
            settings.Converters.Add(new KeyValuePairConverter());
            var json = JsonConvert.SerializeObject(value, settings);
            return json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] ToBson<T>(T value)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var writer = new BsonWriter(ms))
                {
                    var serializer = new JsonSerializer()
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        ContractResolver = new ProjectContractResolver()
                    };
                    serializer.Serialize(writer, value);
                }
                return ms.ToArray();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bson"></param>
        /// <returns></returns>
        public T FromBson<T>(byte[] bson) 
        {
            using (var ms = new System.IO.MemoryStream(bson))
            {
                using (var reader = new BsonReader(ms))
                {
                    var serializer = new JsonSerializer()
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        ContractResolver = new ProjectContractResolver()
                    };
                    var value = serializer.Deserialize<T>(reader);
                    return value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T FromJson<T>(string json)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ProjectContractResolver()
            };
            settings.Converters.Add(new KeyValuePairConverter());
            var value = JsonConvert.DeserializeObject<T>(json, settings);
            return value;
        }
    }
}
