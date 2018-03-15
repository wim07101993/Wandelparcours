using MongoDB.Bson.Serialization.Attributes;
using WebService.Models.Bases;

namespace WebService.Models
{
    /// <summary>
    /// MediaData is a class that represents the personal media of a resident.
    /// </summary>
    public class MediaData : AModelWithID
    {
        /// <summary>
        /// Data is the media data. If it is music, in the data would be the content of the music file.
        /// <para/>
        /// If the <see cref="MediaData"/> has data, there can't be an url and the other way around.
        /// </summary>
        [BsonElement("data")]
        public byte[] Data { get; set; }
    }
}