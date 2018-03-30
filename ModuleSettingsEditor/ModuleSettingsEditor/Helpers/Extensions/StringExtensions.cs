using Newtonsoft.Json;

namespace ModuleSettingsEditor.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static T Deserialize<T>(this string This)
            => JsonConvert.DeserializeObject<T>(This);

        public static string Serialize(this object This)
            => JsonConvert.SerializeObject(This);

        public static T CloneBySerialization<T>(this T This)
            => This.Serialize().Deserialize<T>();
    }
}