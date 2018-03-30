using DatabaseImporter.Models.MongoModels.Bases;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseImporter.Models.MongoModels
{
    public class MediaUrl : AModelWithObjectID
    {
        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("extension")]
        public string Extension { get; set; }
    }
}