using System.Collections.Generic;
using DatabaseImporter.Models.MongoModels.Bases;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DatabaseImporter.Models.MongoModels
{
    /// <summary>
    /// User is a representation of a user to log in on webApp. 
    /// </summary>
    public class User : AModelWithObjectID
    {
        /// <summary>
        /// UserName is the name of the User
        /// <para/>
        /// In the database the value is stored under the field "userName"
        /// </summary>
        [BsonElement("userName")]
        [JsonProperty("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// Email is the email of the User
        /// <para/>
        /// In the database the value is stored under the field "email"
        /// </summary>
        [BsonElement("email")]
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Password is the password of the User
        /// <para/>
        /// In the database the value is stored under the field "password"
        /// </summary>
        [BsonElement("password")]
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// UserType is the level of authorization of the User
        /// <para/>
        /// In the database the value is stored under the field "authLevel"
        /// </summary>
        [BsonElement("userType")]
        [JsonProperty("userType")]
        public EUserType UserType { get; set; } = EUserType.Guest;

        [BsonElement("residents")]
        [JsonProperty("residents")]
        public IEnumerable<ObjectId> Residents { get; set; }

        [BsonElement("group")]
        [JsonProperty("group")]
        public string Group { get; set; }
    }
}