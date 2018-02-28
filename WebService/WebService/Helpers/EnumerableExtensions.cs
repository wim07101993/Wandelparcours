using System.Collections.Generic;
using System.Linq;

namespace WebService.Helpers
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(IEnumerable<T> This)
            => This == null || !This.Any();
    }
}
