using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ModuleSettingsEditor.Helpers.Extensions
{
    public static class MemberInfoExtensions
    {
        public static string GetDisplayName(this MemberInfo This)
            => This
                    .GetCustomAttributes()
                    .FirstOrDefault(x => x is DisplayNameAttribute)
                is DisplayNameAttribute attribute
                ? attribute.DisplayName
                : This.Name;

        public static bool HasAttribute<T>(this MemberInfo This) where T : Attribute
            => This.GetCustomAttributes<T>().Any();
    }
}