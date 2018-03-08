using MongoDB.Bson.Serialization.Attributes;
using WebService.Models.Bases;

namespace WebService.Models
{
    /// <summary>
    /// MediaWithId is a class that represents the personal media of a resident.
    /// </summary>
    public class MediaWithId : AModelWithID
    {
        private byte[] _data;
        private string _url;

        /// <summary>
        /// Data is the media data. If it is music, in the data would be the content of the music file.
        /// <para/>
        /// If the <see cref="MediaWithId"/> has data, there can't be an url and the other way around.
        /// </summary>
        [BsonElement("data")]
        public byte[] Data
        {
            get => _data;
            set
            {
                _data = value;

                if (value != null)
                    Url = null;
            }
        }

        /// <summary>
        /// Url is a string that holds the url to the data of the media.
        /// <para/>
        /// If the <see cref="MediaWithId"/> has an url, there can't be data and the other way around.
        /// </summary>
        [BsonElement("url")]
        public string Url
        {
            get => _url;
            set
            {
                _url = value;

                if (value != null)
                    Data = null;
            }
        }
    }
}