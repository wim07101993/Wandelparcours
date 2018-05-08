using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebService.Helpers.Extensions;

namespace WebService.Helpers.JsonConverters
{
    public class ObjectIdListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsonList = ((IEnumerable<ObjectId>)value)?.Select(x => x.ToString());
            
            serializer.Serialize(writer, jsonList.Serialize());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var str = JToken.Load(reader).ToObject<string>();
            var list = str.Deserialize<List<string>>();
            return list.Select(x => new ObjectId(x));
        }

        public override bool CanConvert(Type objectType)
            => typeof(IEnumerable<ObjectId>).IsAssignableFrom(objectType);
    }
}