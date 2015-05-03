using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;

namespace TestJSON
{
    public class ContainerSerializer
    {
        public static string Serialize<T>(T value)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            settings.Converters.Add(new KeyValuePairConverter());
            var json = JsonConvert.SerializeObject(value, settings);
            return json;
        }

        public static T Deserialize<T>(string json)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ListContractResolver()
            };
            settings.Converters.Add(new KeyValuePairConverter());
            var container = JsonConvert.DeserializeObject<T>(json, settings);
            return container;
        }
    }
}
