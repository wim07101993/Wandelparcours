using MongoDB.Bson.Serialization.Attributes;

namespace WebService.Models
{
    /// <summary>
    /// Doctor is the representation of the doctor that takes care of the Resident.
    /// </summary>
    public class Doctor
    {
        /// <summary>
        /// Name is the name of the User
        /// <para/>
        /// In the database the value is stored under the field "name"
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// PhoneNumber is the phone number of the User
        /// <para/>
        /// In the database the value is stored under the field "phoneNumber"
        /// </summary>
        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }

    }
}
