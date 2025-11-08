// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Reflection;
using System.Text.Json;
using Core2D.Model;
using Core2D.ViewModels.Containers;

namespace Core2D.Model.OpenXml;

public static class OpenXmlPayloadSerializer
{
    private sealed class PayloadManifest
    {
        public string? Type { get; set; }
        public string? Data { get; set; }
    }

    public static string Create(IJsonSerializer serializer, object item)
    {
        ArgumentNullException.ThrowIfNull(serializer);
        ArgumentNullException.ThrowIfNull(item);

        var json = serializer.Serialize(item);
        var typeName = item.GetType().AssemblyQualifiedName
                       ?? item.GetType().FullName
                       ?? item.GetType().Name;

        var manifest = new PayloadManifest
        {
            Type = typeName,
            Data = json
        };

        return JsonSerializer.Serialize(manifest);
    }

    public static object? Deserialize(IJsonSerializer serializer, string payloadJson)
    {
        ArgumentNullException.ThrowIfNull(serializer);

        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            return null;
        }

        var manifest = JsonSerializer.Deserialize<PayloadManifest>(payloadJson);
        if (manifest?.Data is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(manifest.Type))
        {
            var type = Type.GetType(manifest.Type, throwOnError: false);
            if (type is { })
            {
                var method = serializer.GetType().GetMethod(nameof(IJsonSerializer.Deserialize));
                if (method is { })
                {
                    var generic = method.MakeGenericMethod(type);
                    return generic.Invoke(serializer, new object[] { manifest.Data });
                }
            }
        }

        return serializer.Deserialize<ProjectContainerViewModel>(manifest.Data);
    }
}
