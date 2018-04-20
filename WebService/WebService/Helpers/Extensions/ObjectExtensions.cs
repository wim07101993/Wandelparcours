using Newtonsoft.Json;

namespace WebService.Helpers.Extensions
{
    public static class ObjectExtensions
    {
        public static string Serialize(this object This)
            => JsonConvert.SerializeObject(This);

        public static T Deserialize<T>(this string This)
            => JsonConvert.DeserializeObject<T>(This);

        public static T Clone<T>(this T This)
            => This.Serialize().Deserialize<T>();
    }
}