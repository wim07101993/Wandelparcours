using System;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService.Models
{
    /// <summary>
    /// Point is a X,Y-location where something (or someone) is at a given point in time.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// X is the x coordinate of the Point
        /// <para/>
        /// In the database the value is stored under the field "x"
        /// </summary>
        [BsonElement("x")]
        public double X { get; set; }

        /// <summary>
        /// Y is the y coordinate of the Point
        /// <para/>
        /// In the database the value is stored under the field "y"
        /// </summary>
        [BsonElement("y")]
        public double Y { get; set; }

        // TODO use datetime instead of string
        /// <summary>
        /// TimeStamp is the time when the element/person has been detected
        /// <para/>
        /// In the database the value is stored under the field "timeStamp"
        /// </summary>
        [BsonElement("timeStamp")]
        public string TimeStamp { get; set; }
    }
}