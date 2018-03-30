using System;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DatabaseImporter.Helpers.JsonConverters
{
    /// <inheritdoc cref="JsonConverter" />
    /// <summary>
    /// ObjectIdConverter is a class that extends from the abstract <see cref="JsonConverter"/> class.
    /// <para/>
    /// It converts an object id (of MongoDb) to a string by using the <see cref="ObjectId.ToString"/> method.
    /// </summary>
    public class ObjectIdConverter : JsonConverter
    {
        /// <inheritdoc cref="JsonConverter.WriteJson"/>
        /// <summary>
        /// Writes the JSON representation of the <see cref="ObjectId"/> to the <see cref="writer" />.
        /// </summary>
        /// <param name="writer">is the <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to</param>
        /// <param name="value">is the value to write</param>
        /// <param name="serializer">is the calling serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            // write the object id to the writer
            => serializer.Serialize(writer, value.ToString());

        /// <inheritdoc cref="JsonConverter.ReadJson" />
        /// <summary>Reads the JSON representation of the <see cref="ObjectId"/>.</summary>
        /// <param name="reader">is the <see cref="JsonReader" /> to read from</param>
        /// <param name="objectType">is the type of the object</param>
        /// <param name="existingValue">is the existing value of object being read</param>
        /// <param name="serializer">the calling serializer</param>
        /// <returns>The <see cref="ObjectId"/> value</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
            // return a new ObjectId
            => new ObjectId(JToken.Load(reader).ToObject<string>());

        /// <inheritdoc cref="JsonConverter.CanConvert" />
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">is the type of the object</param>
        /// <returns>
        /// - true if this instance can convert the specified object type (ObjectId is assignable from the passed type)
        /// - false if this instance can not convert the specified object type (ObjectId is not assignable from the passed type)
        /// </returns>
        public override bool CanConvert(Type objectType) 
            // return true if the type ObjectId type is assignable from the passed type
            => typeof(ObjectId).IsAssignableFrom(objectType);
    }
}