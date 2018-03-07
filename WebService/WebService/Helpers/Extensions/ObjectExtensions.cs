using Newtonsoft.Json;

namespace WebService.Helpers.Extensions
{
    /// <summary>
    /// ObjectExtensions is a static class that holds extension methods for the <see cref="object"/> class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Serialize converts <see cref="This"/> to a string in json format using the <see cref="Newtonsoft.Json"/> library.
        /// </summary>
        /// <param name="This">is the object to convert</param>
        /// <returns>A string that represents this object in json format</returns>
        public static string Serialize(this object This)
            // Convert the object using the SerializeObject method of Newtonsoft
            => JsonConvert.SerializeObject(This);

        /// <summary>
        /// Deserialize converts <see cref="This"/> to an object of type <see cref="T"/> using the <see cref="Newtonsoft.Json"/> library.
        /// </summary>
        /// <typeparam name="T">is the type the string should be converted to</typeparam>
        /// <param name="This">is the string to convert</param>
        /// <returns>An object of type <see cref="T"/> that holds the field of the json</returns>
        public static T Deserialize<T>(this string This)
            // Convert the string using the DeserializeObject method of Newtonsoft
            => JsonConvert.DeserializeObject<T>(This);

        /// <summary>
        /// Clone makes a deep copy of an object using serialisation and deserialisation.
        /// </summary>
        /// <typeparam name="T">is the type of <see cref="This"/></typeparam>
        /// <param name="This">is the object to clone</param>
        /// <returns>A deep copy (a clone) of <see cref="This"/></returns>
        public static T Clone<T>(this T This)
            => This.Serialize().Deserialize<T>();
    }
}