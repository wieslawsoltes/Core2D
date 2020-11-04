using System;
using Core2D.Style;
using Newtonsoft.Json;

namespace Core2D.Serializer.Newtonsoft
{
    internal class FontStyleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FontStyle) || objectType == typeof(FontStyle);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((FontStyle)value).Flags.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(FontStyle) || objectType == typeof(FontStyle))
            {
                return FontStyle.Parse((string)reader.Value);
            }
            throw new ArgumentException("objectType");
        }
    }
}
