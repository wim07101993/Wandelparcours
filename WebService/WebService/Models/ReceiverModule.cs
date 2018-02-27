using MongoDB.Bson.Serialization.Attributes;

namespace WebService.Models
{
    public class ReceiverModule
    {
        [BsonElement("mac")]
        public string Mac { get; set; }

        [BsonElement("postition")]
        public Point Position { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }
    }
}
