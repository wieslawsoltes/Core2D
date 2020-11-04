using System;
using Core2D.Renderer;
using Newtonsoft.Json;

namespace Core2D.Serializer.Newtonsoft
{
    internal class ShapeStateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ShapeState) || objectType == typeof(ShapeState);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ShapeState)value).Flags.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(ShapeState) || objectType == typeof(ShapeState))
            {
                return ShapeState.Parse((string)reader.Value);
            }
            throw new ArgumentException("objectType");
        }
    }
}
