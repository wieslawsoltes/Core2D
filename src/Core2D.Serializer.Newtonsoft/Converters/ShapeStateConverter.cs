// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using Newtonsoft.Json;
using System;

namespace Core2D.Serializer.Newtonsoft
{
    /// <inheritdoc/>
    internal class ShapeStateConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ShapeState);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ShapeState)value).Flags.ToString());
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(ShapeState))
            {
                return ShapeState.Parse((string)reader.Value);
            }
            throw new ArgumentException("objectType");
        }
    }
}
