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
            return objectType == typeof(IShapeState) || objectType == typeof(ShapeState);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((IShapeState)value).Flags.ToString());
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(IShapeState) || objectType == typeof(ShapeState))
            {
                return ShapeState.Parse((string)reader.Value);
            }
            throw new ArgumentException("objectType");
        }
    }
}
