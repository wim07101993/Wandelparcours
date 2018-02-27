using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers;

namespace WebService.Models
{
    public class Inhabitant
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId ID { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("picture")]
        public byte[] Picture { get; set; }

        [BsonElement("room")]
        public string Room { get; set; }

        [BsonElement("birthday")]
        public DateTime Birthday { get; set; }

        [BsonElement("doctor")]
        public Doctor Doctor { get; set; }

        [BsonElement("tags")]
        public IEnumerable<int> Tags { get; set; }

        [BsonElement("music")]
        public IEnumerable<byte[]> Music { get; set; }

        [BsonElement("videos")]
        public IEnumerable<byte[]> Videos { get; set; }

        [BsonElement("images")]
        public IEnumerable<byte[]> Images { get; set; }

        [BsonElement("colors")]
        public IEnumerable<byte[]> Colors { get; set; }

        [BsonElement("lastRecordedPosition")]
        public Point LastRecordedPosition { get; set; }

        [BsonElement("locations")]
        public IEnumerable<Point> Locations { get; set; }
    }
}