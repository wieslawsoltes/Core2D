// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Style;
using Newtonsoft.Json;

namespace Core2D.Serializer.Newtonsoft
{
    /// <inheritdoc/>
    internal class FontStyleConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IFontStyle) || objectType == typeof(FontStyle);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((IFontStyle)value).Flags.ToString());
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(IFontStyle) || objectType == typeof(FontStyle))
            {
                return FontStyle.Parse((string)reader.Value);
            }
            throw new ArgumentException("objectType");
        }
    }
}
