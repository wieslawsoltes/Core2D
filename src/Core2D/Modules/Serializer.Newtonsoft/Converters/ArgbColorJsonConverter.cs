﻿using System;
using Core2D.Style;
using Newtonsoft.Json;

namespace Core2D.Serializer.Newtonsoft
{
    internal class ArgbColorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BaseColor) || objectType == typeof(ArgbColor) || objectType == typeof(ArgbColor);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value as BaseColor)
            {
                case ArgbColor argbColor:
                    writer.WriteValue(ArgbColor.ToXamlHex(argbColor));
                    break;

                default:
                    throw new NotSupportedException($"The {value.GetType()} color type is not supported.");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(BaseColor) || objectType == typeof(ArgbColor) || objectType == typeof(ArgbColor))
            {
                return ArgbColor.Parse((string)reader.Value);
            }
            throw new ArgumentException("objectType");
        }
    }
}