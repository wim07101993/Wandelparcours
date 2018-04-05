using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DatabaseImporter.Models.MongoModels
{
    /// <summary>
    /// Doctor is the representation of the doctor that takes care of the Value.
    /// </summary>
    public class Doctor
    {
        /// <summary>
        /// Name is the name of the User
        /// <para/>
        /// In the database the value is stored under the field "name"
        /// </summary>
        [BsonElement("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// PhoneNumber is the phone number of the User
        /// <para/>
        /// In the database the value is stored under the field "phoneNumber"
        /// </summary>
        [BsonElement("phoneNumber")]
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }


        public override string ToString()
            => $"{Name} - {PhoneNumber}";
    }
}