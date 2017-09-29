// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using Utf8Json;

namespace Core2D.Serializer.Utf8Json
{
    /// <summary>
    /// Json serializer.
    /// </summary>
    public sealed class Utf8JsonJsonSerializer : IJsonSerializer
    {
        /// <inheritdoc/>
        string IJsonSerializer.Serialize<T>(T value)
        {
            return JsonSerializer.ToJsonString(value);
        }

        /// <inheritdoc/>
        T IJsonSerializer.Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
