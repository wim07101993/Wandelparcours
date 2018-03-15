using System;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebService.Helpers.JsonConverters
{
    public class ColorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is Color color))
                serializer.Serialize(writer, null);

            serializer.Serialize(writer, $"[{color.R}, {color.G}, {color.B}]");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var bytes = JToken.Load(reader).ToObject<byte[]>();

            return Color.FromArgb(bytes[0], bytes[1], bytes[2]);
        }

        public override bool CanConvert(Type objectType)
            => typeof(byte[]).IsAssignableFrom(objectType);
    }
}