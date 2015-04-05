using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Core;

namespace Test.Util
{
    public class ContainerSerializer
    {
        public static string Serialize(Container container)
        {
            var json = JsonConvert.SerializeObject(
                container,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Objects,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });
            return json;
        }

        public static Container Deserialize(string json)
        {
            var container = JsonConvert.DeserializeObject<Container>(
                json,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Objects,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    ContractResolver = new ListContractResolver()
                });
            return container;
        }
    }
}
