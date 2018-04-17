using System;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebService.Helpers.JsonConverters
{
    public class ObjectIdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => serializer.Serialize(writer, value.ToString());

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
            => new ObjectId(JToken.Load(reader).ToObject<string>());

        public override bool CanConvert(Type objectType)
            => typeof(ObjectId).IsAssignableFrom(objectType);
    }
}