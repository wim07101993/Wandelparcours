using System;
using System.Collections.Generic;
using System.Drawing;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WebService.Helpers.JsonConverters;
using WebService.Models.Bases;
using ColorConverter = System.Drawing.ColorConverter;

namespace WebService.Models
{
    /// <summary>
    /// Value is a representation of a resident in the home to use for detection and animation at the kiosk.
    /// </summary>
    public class Resident : AModelWithID
    {
        /// <summary>
        /// FirstName is the first name of the Value
        /// <para/>
        /// In the database the value is stored under the field "firstName"
        /// </summary>
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// LastName is the last name of the Value
        /// <para/>
        /// In the database the value is stored under the field "lastName"
        /// </summary>
        [BsonElement("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Picture is a picture of the Value
        /// <para/>
        /// In the database the value is stored under the field "picture"
        /// </summary>
        [BsonElement("picture")]
        public byte[] Picture { get; set; } = { };

        /// <summary>
        /// Room is the room of the Value
        /// <para/>
        /// In the database the value is stored under the field "room"
        /// </summary>
        [BsonElement("room")]
        public string Room { get; set; }

        /// <summary>
        /// Birthday is the birthday of the Value
        /// <para/>
        /// In the database the value is stored under the field "birthday"
        /// </summary>
        [BsonElement("birthday")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Doctor is the doctor that takes care of the Value
        /// <para/>
        /// In the database the value is stored under the field "doctor"
        /// </summary>
        [BsonElement("doctor")]
        public Doctor Doctor { get; set; }

        /// <summary>
        /// Tags are the tags to track the Value
        /// <para/>
        /// In the database the value is stored under the field "tags"
        /// </summary>
        [BsonElement("tags")]
        public List<int> Tags { get; set; } = new List<int>();

        /// <summary>
        /// Music is the music-collection of the Value
        /// <para/>
        /// In the database the value is stored under the field "music"
        /// </summary>
        [BsonElement("music")]
        public List<MediaUrl> Music { get; set; } = new List<MediaUrl>();

        /// <summary>
        /// Videos is the video-collection of the Value
        /// <para/>
        /// In the database the value is stored under the field "videos"
        /// </summary>
        [BsonElement("videos")]
        public List<MediaUrl> Videos { get; set; } = new List<MediaUrl>();

        /// <summary>
        /// Images is the image-collection of the Value
        /// <para/>
        /// In the database the value is stored under the field "images"
        /// </summary>
        [BsonElement("images")]
        public List<MediaUrl> Images { get; set; } = new List<MediaUrl>();

        /// <summary>
        /// Colors are the favorite colors of the Value
        /// <para/>
        /// In the database the value is stored under the field "colors"
        /// </summary>
        [BsonElement("colors")]
        [JsonConverter(typeof(ColorConverter))]
        [BsonSerializer(typeof(ColorSerializer))]
        public List<Color> Colors { get; set; } = new List<Color>();

        /// <summary>
        /// LastRecordedPosition is position where the resident has last been tracked
        /// <para/>
        /// In the database the value is stored under the field "lastRecordedPosition"
        /// </summary>
        [BsonElement("lastRecordedPosition")]
        public Point LastRecordedPosition { get; set; }

        /// <summary>
        /// Locations are the locations where the residents has been tracked.
        /// <para/>
        /// In the database the value is stored under the field "locations"
        /// </summary>
        [BsonElement("locations")]
        public List<Point> Locations { get; set; } = new List<Point>();
    }
}