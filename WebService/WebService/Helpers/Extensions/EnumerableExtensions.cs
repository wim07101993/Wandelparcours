using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebService.Services.Randomizer;

namespace WebService.Helpers.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(IEnumerable<T> This)
            => This == null || !This.Any();

        public static object RandomItem(this IList This)
            => This[Randomizer.Instance.Next(This.Count)];


        public static bool Remove<T>(this IList<T> This, Func<T, bool> predicate)
            => This.Remove(This.First(predicate));

        public static void Remove(this IList This, Func<object, bool> predicate)
            => This.RemoveAt(This.IndexOf(predicate));
    }
}