using System;
using Core2D;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core2D.Serializer.Newtonsoft
{
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly JsonSerializerSettings Settings;

        static NewtonsoftJsonSerializer()
        {
            Settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ProjectContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Converters =
                {
                    new KeyValuePairConverter(),
                    new FontStyleConverter(),
                    new ShapeStateConverter()
                }
            };
        }

        public NewtonsoftJsonSerializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        string IJsonSerializer.Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        T IJsonSerializer.Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
