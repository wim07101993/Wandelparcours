using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers.JsonConverters;
using WebService.Models.Bases;

namespace WebService.Models
{
    public class MediaData : AModelWithID
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