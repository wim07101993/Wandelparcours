using Newtonsoft.Json;

namespace ModuleSettingsEditor.WPF.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static T Deserialize<T>(this string This)
            => JsonConvert.DeserializeObject<T>(This);
    }
}