#nullable disable
using Core2D.Model;
using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core2D.Modules.Serializer.Newtonsoft
{
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer(ILifetimeScope lifetimeScope)
        {
            _settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ProjectContractResolver(lifetimeScope),
                NullValueHandling = NullValueHandling.Ignore,
                Converters =
                {
                    new KeyValuePairConverter()
                }
            };
        }

        string IJsonSerializer.Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, _settings);
        }

        T IJsonSerializer.Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}
