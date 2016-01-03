// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// 
    /// </summary>
    public class NewtonsoftSerializer : ISerializer
    {
        /// <inheritdoc/>
        public string Serialize<T>(T value)
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
            var text = JsonConvert.SerializeObject(value, settings);
            return text;
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string text)
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
            var value = JsonConvert.DeserializeObject<T>(text, settings);
            return value;
        }
    }
}
