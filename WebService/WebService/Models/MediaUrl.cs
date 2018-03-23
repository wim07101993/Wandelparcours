using MongoDB.Bson.Serialization.Attributes;
using WebService.Models.Bases;

namespace WebService.Models
{
    public class MediaUrl : AModelWithID
    {
        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("extension")]
        public string Extension { get; set; }
    }
}