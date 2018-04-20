using MongoDB.Bson.Serialization.Attributes;
using WebService.Models.Bases;

namespace WebService.Models
{
    public class ReceiverModule : AModelWithID
    {
        [BsonElement("mac")]
        public string Mac { get; set; }

        [BsonElement("position")]
        public Point Position { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }
    }
}