using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WebService.Models.Bases;

namespace WebService.Models
{
    public class Location : AModelWithID
    {
        /// <inheritdoc cref="AModelWithID.Id" />
        /// <summary>
        /// X is the x coordinate of the Point
        /// <para />
        /// In the database the value is stored under the field "x"
        /// </summary>
   

        /// <summary>
        /// X is the x coordinate of the Location
        /// <para/>
        /// In the database the value is stored under the field "x"
        /// </summary>
        [BsonElement("residentId")]
        public ObjectId ResidentId { get; set; }
        
        
        
        /// <summary>
        /// X is the x coordinate of the Location
        /// <para/>
        /// In the database the value is stored under the field "x"
        /// </summary>
        [BsonElement("x")]
        public double X { get; set; }

        /// <summary>
        /// Y is the y coordinate of the Location
        /// <para/>
        /// In the database the value is stored under the field "y"
        /// </summary>
        [BsonElement("y")]
        public double Y { get; set; }

        /// <summary>
        /// TimeStamp is the time when the person has been detected
        /// <para/>
        /// In the database the value is stored under the field "timeStamp"
        /// </summary>
        [BsonElement("timeStamp")]
        public DateTime TimeStamp { get; set; }
    }
}