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
            => This.Count > 0
                ? This[Randomizer.Instance.Next(This.Count)]
                : throw new IndexOutOfRangeException();

        public static bool Remove<T>(this IList<T> This, Func<T, bool> predicate)
        {
            for (var i = 0; i < This.Count; i++)
                if (predicate(This[i]))
                {
                    This.RemoveAt(i);
                    return true;
                }

            return false;
        }
    }
}