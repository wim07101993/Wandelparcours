using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ModuleSettingsEditor.Helpers.Extensions
{
    public static class MemberInfoExtensions
    {
        public static string GetDisplayName(this MemberInfo This)
            => This.GetCustomAttribute<DisplayNameAttribute>().DisplayName ?? This.Name;

        public static bool HasAttribute<T>(this MemberInfo This) where T : Attribute
            => This.GetCustomAttributes<T>().Any();

        public static bool IsBrowsable(this MemberInfo This)
            => This.GetCustomAttribute<BrowsableAttribute>()?.Browsable
               ?? BrowsableAttribute.Default.Browsable;
    }
}