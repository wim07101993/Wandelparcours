using MongoDB.Bson.Serialization.Attributes;

namespace WebService.Models
{
    public class Color
    {
        [BsonElement("r")]
        public byte R { get; set; }

        [BsonElement("g")]
        public byte G { get; set; }

        [BsonElement("b")]
        public byte B { get; set; }
    }
}