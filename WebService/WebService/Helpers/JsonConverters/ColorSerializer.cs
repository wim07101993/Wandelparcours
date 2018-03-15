using System.Drawing;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace WebService.Helpers.JsonConverters
{
    public class ColorSerializer : SerializerBase<Color>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Color value)
        {
            context.Writer.WriteBytes(new[] {value.R, value.G, value.B});
        }

        public override Color Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Binary)
            {
                var bytes = context.Reader.ReadBytes();
                return Color.FromArgb(bytes[0], bytes[1], bytes[2]);
            }

            context.Reader.SkipValue();
            return Color.Transparent;
        }
    }
}