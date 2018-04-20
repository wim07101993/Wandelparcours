using MongoDB.Bson.Serialization.Attributes;

namespace WebService.Models
{
    public class Doctor
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}