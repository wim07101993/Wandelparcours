using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebService.Services.Randomizer;

namespace WebService.Helpers.Extensions
{
    /// <summary>
    /// EnumerableExtensions is a static class that holds extension methods for the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// IsNullOrEmpty is a method that checks if an <see cref="IEnumerable{T}"/> exists and if it holds any elements
        /// </summary>
        /// <typeparam name="T">is the generic type of the <see cref="IEnumerable{T}"/></typeparam>
        /// <param name="This">is the <see cref="IEnumerable{T}"/> to check</param>
        /// <returns>
        /// - true if the <see cref="IEnumerable{T}"/> is null or if it holds no items
        /// - false if the <see cref="IEnumerable{T}"/> holds any items
        /// </returns>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> This)
            // check for null and if This holds any elements
            => This == null || !This.Any();

        public static T RandomItem<T>(this IList<T> This)
            => This[Randomizer.Instance.Next(This.Count)];


        public static bool Remove<T>(this IList<T> This, Func<T, bool> predicate)
            => This.Remove(This.First(predicate));

        public static void Remove(this IList This, Func<object, bool> predicate)
            => This.RemoveAt(This.IndexOf(predicate));
    }
}