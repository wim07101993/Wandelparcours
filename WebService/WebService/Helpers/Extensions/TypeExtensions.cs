using System;

namespace WebService.Helpers.Extensions
{
    /// <summary>
    /// TypeExtensions is a static class that holds extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// GetDefault returns the default value of a type (if not null, it uses the <see cref="Activator.CreateInstance(Type)"/> method)
        /// </summary>
        /// <param name="This">is the type of which the default value should be returned</param>
        /// <returns>The default value of a type</returns>
        public static object GetDefault(this Type This)
            // check if the value is a value type (if it can be null)
            => This.IsValueType
                // return a new value if the type can't be null
                ? Activator.CreateInstance(This)
                // return null if ti can be
                : null;
    }
}