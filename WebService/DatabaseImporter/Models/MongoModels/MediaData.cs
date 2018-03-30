using DatabaseImporter.Helpers.JsonConverters;
using DatabaseImporter.Models.MongoModels.Bases;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DatabaseImporter.Models.MongoModels
{
    public class MediaData :AModelWithObjectID
    {
        [BsonElement("data")]
        public byte[] Data { get; set; }

        [BsonElement("extension")]
        public string Extension { get; set; }

        [BsonElement("ownerId")]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId OwnerId { get; set; }
    }
}