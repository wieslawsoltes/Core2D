using System;
using Core2D.Renderer;
using Newtonsoft.Json;

namespace Core2D.Serializer.Newtonsoft
{
    /// <inheritdoc/>
    internal class ShapeStateConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ShapeState) || objectType == typeof(ShapeState);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ShapeState)value).Flags.ToString());
        }

        /// <inheritdoc/>
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
