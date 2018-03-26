using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WebService.Helpers.Extensions;
using WebService.Models.Bases;

namespace WebService.Models
{
    /// <summary>
    /// User is a representation of a user to log in on webApp. 
    /// </summary>
    public class User : AModelWithID
    {
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
        /// UserType is the level of authorization of the User
        /// <para/>
        /// In the database the value is stored under the field "authLevel"
        /// </summary>
        [BsonElement("userType")]
        public EUserType UserType { get; set; } = EUserType.Guest;


        [BsonElement("residents")]
        public IEnumerable<Resident> Residents { get; set; }
    }
}