// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// Json serializer.
    /// </summary>
    public class NewtonsoftTextSerializer : ITextSerializer
    {
        /// <summary>
        /// The class library assembly name.
        /// </summary>
        public static string AssemblyName = "Core2D";

        /// <summary>
        /// The class library namespace prefix.
        /// </summary>
        public static string NamespacePrefix = "Core2D";

        /// <summary>
        /// Specifies the settings on a <see cref="JsonSerializer"/> object.
        /// </summary>
        public static JsonSerializerSettings Settings;

        /// <summary>
        /// Initializes static data.
        /// </summary>
        static NewtonsoftTextSerializer()
        {
            Settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new ProjectContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Binder = new JsonSerializationBinder(AssemblyName, NamespacePrefix),
                Converters =
                {
                    new KeyValuePairConverter(),
                    new ArgbColorJsonConverter(),
                    new FontStyleConverter(),
                    new ShapeStateConverter()
                }
            };
        }

        /// <inheritdoc/>
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, Settings);
        }
    }
}
