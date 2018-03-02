using System;

namespace WebService.Helpers
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type This)
            => This.IsValueType
                ? Activator.CreateInstance(This)
                : null;
    }
}