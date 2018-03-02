using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers;

namespace WebService.Models.Bases
{
    public abstract class AModelWithID : IModelWithID
    {
        /// <summary>
        /// ID is the id of the Value
        /// <para/>
        /// When serialized to json it is converted to a string.
        /// <para/>
        /// In the database the value is stored under the field "id"
        /// </summary>
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId ID { get; set; }
    }
}