// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.ObjectModel;
using Core2D.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core2D.Modules.Serializer.Newtonsoft;

public sealed class NewtonsoftJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerSettings _settings;

    public NewtonsoftJsonSerializer(IServiceProvider serviceProvider)
    {
        _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Objects,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ContractResolver = new ProjectContractResolver(serviceProvider, typeof(ObservableCollection<>)),
            NullValueHandling = NullValueHandling.Ignore,
            Converters =
            {
                new KeyValuePairConverter()
            }
        };
    }

    string IJsonSerializer.Serialize<T>(T? value) where T : default
    {
        return JsonConvert.SerializeObject(value, _settings);
    }

    T? IJsonSerializer.Deserialize<T>(string json) where T : default
    {
        return JsonConvert.DeserializeObject<T>(json, _settings);
    }
}
