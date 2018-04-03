using DatabaseImporter.Helpers.JsonConverters;
using DatabaseImporter.Models.MongoModels.Bases;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DatabaseImporter.Models.MongoModels
{
    public class MediaData : AModelWithObjectID
    {
        [BsonElement("data")]
        [JsonProperty("data")]
        public byte[] Data { get; set; }

        [BsonElement("extension")]
        [JsonProperty("extension")]
        public string Extension { get; set; }

        [BsonElement("ownerId")]
        [JsonProperty("ownerId")]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId OwnerId { get; set; }
    }
}