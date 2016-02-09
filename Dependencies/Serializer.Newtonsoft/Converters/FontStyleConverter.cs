// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Style;
using Newtonsoft.Json;
using System;

namespace Serializer.Newtonsoft
{
    /// <inheritdoc/>
    internal class FontStyleConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FontStyle);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((FontStyle)value).Flags.ToString());
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(FontStyle))
            {
                return FontStyle.Parse((string)reader.Value);
            }
            throw new ArgumentException("objectType");
        }
    }
}
