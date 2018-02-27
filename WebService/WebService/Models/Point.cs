using System;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService.Models
{
    public struct Point
    {
        [BsonElement("x")]
        public double X { get; set; }

        [BsonElement("y")]
        public double Y { get; set; }

        [BsonElement("timeStamp")]
        public DateTime TimeStamp { get; set; }
    }
}