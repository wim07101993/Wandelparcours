using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers;

namespace WebService.Models
{
    /// <summary>
    /// ReceiverModule is the representation of a hardware module that detects tags of residents.
    /// </summary>
    public class ReceiverModule
    {
        /// <summary>
        /// ID is the id of the User
        /// <para/>
        /// When serialized to json it is converted to a string.
        /// <para/>
        /// In the database the value is stored under the field "id"
        /// </summary>
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId ID { get; set; }

        /// <summary>
        /// Mac is the mac address of the ReceiverModule
        /// <para/>
        /// In the database the value is stored under the field "mac"
        /// </summary>
        [BsonElement("mac")]
        public string Mac { get; set; }

        /// <summary>
        /// Position is the position of the ReceiverModule
        /// <para/>
        /// In the database the value is stored under the field "postition"
        /// </summary>
        [BsonElement("postition")]
        public Point Position { get; set; }

        /// <summary>
        /// IsActive is an indication to determine if the ReceiverModule is active or not
        /// <para/>
        /// In the database the value is stored under the field "isActive"
        /// </summary>
        [BsonElement("isActive")]
        public bool IsActive { get; set; }
    }
}
