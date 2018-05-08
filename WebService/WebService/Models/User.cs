using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers.JsonConverters;
using WebService.Models.Bases;

namespace WebService.Models
{
    public class User : AModelWithID
    {
        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("userType")]
        public EUserType UserType { get; set; } = EUserType.Guest;

        [BsonElement("residents")]
        [JsonConverter(typeof(ObjectIdListConverter))]
        public IEnumerable<ObjectId> Residents { get; set; } = new List<ObjectId>();

        [BsonElement("group")]
        public string Group { get; set; }
    }
}