using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers.JsonConverters;
using WebService.Models.Bases;

namespace WebService.Models
{
    public class ResidentLocation : Point, IModelWithID
    {
        /// <summary>
        /// Id is the id of the Value
        /// <para/>
        /// When serialized to json it is converted to a string.
        /// <para/>
        /// In the database the value is stored under the field "id"
        /// </summary>
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public virtual ObjectId Id { get; set; }
   

        /// <summary>
        /// X is the x coordinate of the Location
        /// <para/>
        /// In the database the value is stored under the field "x"
        /// </summary>
        [BsonElement("residentId")]
        [JsonConverter(typeof(ObjectIdConverter))]
         public ObjectId ResidentId { get; set; }
    }
}