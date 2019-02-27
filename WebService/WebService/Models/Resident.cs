using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WebService.Models.Bases;

namespace WebService.Models
{
    public class Resident : AModelWithID
    {
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("picture")]
        public byte[] Picture { get; set; }

        [BsonElement("room")]
        public string Room { get; set; }

        [BsonElement("birthday")]
        public DateTime? Birthday { get; set; }

        [BsonElement("doctor")]
        public Doctor Doctor { get; set; }

        [BsonElement("tags")]
        public List<int> Tags { get; set; } = new List<int>();

        [BsonElement("music")]
        public List<MediaUrl> Music { get; set; } = new List<MediaUrl>();

        [BsonElement("tunes")]
        public List<MediaUrl> Tunes { get; set; } = new List<MediaUrl>();

        [BsonElement("videos")]
        public List<MediaUrl> Videos { get; set; } = new List<MediaUrl>();

        [BsonElement("images")]
        public List<MediaUrl> Images { get; set; } = new List<MediaUrl>();

        [BsonElement("colors")]
        public List<Color> Colors { get; set; }

        [BsonElement("lastRecordedPosition")]
        public ResidentLocation LastRecordedPosition { get; set; }

        [BsonElement("locations")]
        public List<ObjectId> Locations { get; set; } = new List<ObjectId>();
    }
}