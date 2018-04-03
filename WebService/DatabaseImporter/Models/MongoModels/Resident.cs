using System;
using System.Collections.Generic;
using DatabaseImporter.Models.MongoModels.Bases;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DatabaseImporter.Models.MongoModels
{
    /// <summary>
    /// Value is a representation of a resident in the home to use for detection and animation at the kiosk.
    /// </summary>
    public class Resident : AModelWithObjectID
    {
        /// <summary>
        /// FirstName is the first name of the Value
        /// <para/>
        /// In the database the value is stored under the field "firstName"
        /// </summary>
        [BsonElement("firstName")]
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// LastName is the last name of the Value
        /// <para/>
        /// In the database the value is stored under the field "lastName"
        /// </summary>
        [BsonElement("lastName")]
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// ImagePicture is a picture of the Value
        /// <para/>
        /// In the database the value is stored under the field "picture"
        /// </summary>
        [BsonElement("picture")]
        [JsonProperty("picture")]
        public byte[] Picture { get; set; }

        /// <summary>
        /// Room is the room of the Value
        /// <para/>
        /// In the database the value is stored under the field "room"
        /// </summary>
        [BsonElement("room")]
        [JsonProperty("room")]
        public string Room { get; set; }

        /// <summary>
        /// Birthday is the birthday of the Value
        /// <para/>
        /// In the database the value is stored under the field "birthday"
        /// </summary>
        [BsonElement("birthday")]
        [JsonProperty("birthday")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Doctor is the doctor that takes care of the Value
        /// <para/>
        /// In the database the value is stored under the field "doctor"
        /// </summary>
        [BsonElement("doctor")]
        [JsonProperty("doctor")]
        public Doctor Doctor { get; set; }

        /// <summary>
        /// Tags are the tags to track the Value
        /// <para/>
        /// In the database the value is stored under the field "tags"
        /// </summary>
        [BsonElement("tags")]
        [JsonProperty("tags")]
        public List<int> Tags { get; set; }

        /// <summary>
        /// Music is the music-collection of the Value
        /// <para/>
        /// In the database the value is stored under the field "music"
        /// </summary>
        [BsonElement("music")]
        [JsonProperty("music")]
        public List<MediaUrl> Music { get; set; }

        /// <summary>
        /// Videos is the video-collection of the Value
        /// <para/>
        /// In the database the value is stored under the field "videos"
        /// </summary>
        [BsonElement("videos")]
        [JsonProperty("videos")]
        public List<MediaUrl> Videos { get; set; }

        /// <summary>
        /// Images is the image-collection of the Value
        /// <para/>
        /// In the database the value is stored under the field "images"
        /// </summary>
        [BsonElement("images")]
        [JsonProperty("images")]
        public List<MediaUrl> Images { get; set; }

        /// <summary>
        /// Colors are the favorite colors of the Value
        /// <para/>
        /// In the database the value is stored under the field "colors"
        /// </summary>
        [BsonElement("colors")]
        [JsonProperty("colors")]
        public List<Color> Colors { get; set; }

        /// <summary>
        /// LastRecordedPosition is position where the resident has last been tracked
        /// <para/>
        /// In the database the value is stored under the field "lastRecordedPosition"
        /// </summary>
        [BsonElement("lastRecordedPosition")]
        [JsonProperty("lastRecordedPosition")]
        public Point LastRecordedPosition { get; set; }

        /// <summary>
        /// Locations are the locations where the residents has been tracked.
        /// <para/>
        /// In the database the value is stored under the field "locations"
        /// </summary>
        [BsonElement("locations")]
        [JsonProperty("locations")]
        public List<Point> Locations { get; set; }
    }
}