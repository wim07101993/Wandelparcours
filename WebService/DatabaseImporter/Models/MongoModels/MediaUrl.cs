using DatabaseImporter.Models.MongoModels.Bases;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DatabaseImporter.Models.MongoModels
{
    public class MediaUrl : AModelWithObjectID
    {
        [BsonElement("url")]
        [JsonProperty("url")]
        public string Url { get; set; }

        [BsonElement("title")]
        [JsonProperty("title")]
        public string Title { get; set; }

        [BsonElement("extension")]
        [JsonProperty("extension")]
        public string Extension { get; set; }


        public override string ToString()
            => $"{Title}{Extension} {Url ?? ""}";
    }
}