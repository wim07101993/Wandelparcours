using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DatabaseImporter.Helpers.Extensions
{
    public static class ObjectExtensions
    {
        public static string SerializeToJson(this object This)
            => JsonConvert.SerializeObject(This);

        public static string SerializeToCsv<T>(this IEnumerable<T> This, char delimiter = ',',
            bool withFieldsRow = true)
        {
            var builder = new StringBuilder();
            var properties = typeof(T).GetProperties();

            if (withFieldsRow)
            {
                builder.Append(properties[0].Name);
                for (var i = 1; i < properties.Length; i++)
                {
                    builder.Append(',');
                    builder.Append(properties[i].Name);
                }

                builder.Append("\n");
            }

            foreach (var t in This)
            {
                builder.Append(properties[0].GetValue(t));
                for (var i = 1; i < properties.Length; i++)
                {
                    builder.Append(',');
                    builder.Append(properties[i].GetValue(t));
                }

                builder.Append("\n");
            }

            return builder.ToString();
        }

        public static T CloneBySerialization<T>(this T This)
            => This.SerializeToJson().DeserializeJson<T>();
    }
}