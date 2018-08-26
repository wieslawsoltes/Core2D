// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Style;
using Newtonsoft.Json;

namespace Core2D.Serializer.Newtonsoft
{
    /// <inheritdoc/>
    internal class ArgbColorJsonConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IColor) || objectType == typeof(IArgbColor) || objectType == typeof(ArgbColor);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value as IColor)
            {
                case IArgbColor argbColor:
                    writer.WriteValue(ArgbColor.ToHtml(argbColor));
                    break;
                default:
                    throw new NotSupportedException($"The {value.GetType()} color type is not supported.");
            }
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(IColor) || objectType == typeof(IArgbColor) || objectType == typeof(ArgbColor))
            {
                return ArgbColor.Parse((string)reader.Value);
            }
            throw new ArgumentException("objectType");
        }
    }
}
