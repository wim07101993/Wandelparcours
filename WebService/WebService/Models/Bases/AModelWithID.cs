using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers.JsonConverters;

namespace WebService.Models.Bases
{
    public abstract class AModelWithID : IModelWithID
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public virtual ObjectId Id { get; set; }
    }
}