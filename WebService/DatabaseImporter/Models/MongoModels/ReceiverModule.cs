using DatabaseImporter.Models.MongoModels.Bases;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseImporter.Models.MongoModels
{
    /// <summary>
    /// ReceiverModule is the representation of a hardware module that detects tags of residents.
    /// </summary>
    public class ReceiverModule :AModelWithObjectID
    {
        /// <inheritdoc cref="AModelWithObjectID.Id" />
        /// <summary>
        /// X is the x coordinate of the Point
        /// <para />
        /// In the database the value is stored under the field "x"
        /// </summary>
   

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
        [BsonElement("position")]
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