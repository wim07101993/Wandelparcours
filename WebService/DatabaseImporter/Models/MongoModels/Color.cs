using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DatabaseImporter.Models.MongoModels
{
    public class Color
    {
        [BsonElement("r")]
        [JsonProperty("r")]
        public byte R { get; set; }

        [BsonElement("g")]
        [JsonProperty("g")]
        public byte G { get; set; }

        [BsonElement("b")]
        [JsonProperty("b")]
        public byte B { get; set; }


        public override string ToString()
            => $"R:{R}, G:{G}, B:{B}";
    }
}