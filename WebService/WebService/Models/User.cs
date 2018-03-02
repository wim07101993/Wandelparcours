using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers;

namespace WebService.Models
{
    /// <summary>
    /// User is a representation of a user to log in on webApp. 
    /// </summary>
    public class User
    {
        /// <summary>
        /// ID is the id of the User
        /// <para/>
        /// When serialized to json it is converted to a string.
        /// <para/>
        /// In the database the value is stored under the field "id"
        /// </summary>
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId ID { get; set; }

        /// <summary>
        /// UserName is the name of the User
        /// <para/>
        /// In the database the value is stored under the field "userName"
        /// </summary>
        [BsonElement("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// Email is the email of the User
        /// <para/>
        /// In the database the value is stored under the field "email"
        /// </summary>
        [BsonElement("email")]
        public string Email { get; set; }

        /// <summary>
        /// Password is the password of the User
        /// <para/>
        /// In the database the value is stored under the field "password"
        /// </summary>
        [BsonElement("password")]
        public string Password { get; set; }

        /// <summary>
        /// AuthLevel is the level of authorization of the User
        /// <para/>
        /// In the database the value is stored under the field "authLevel"
        /// </summary>
        [BsonElement("authLevel")]
        public EAuthLevel AuthLevel { get; set; }
    }
}
