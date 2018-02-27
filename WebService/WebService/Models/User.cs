using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers;

namespace WebService.Models
{
    public class User
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId ID { get; set; }

        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("authLevel")]
        public EAuthLevel AuthLevel { get; set; }
    }
}
