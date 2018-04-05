using System.ComponentModel;
using System.Reflection;

namespace DatabaseImporter.Helpers.Extensions
{
    public static class MemberInfoExtensions
    {
        public static string GetDisplayName(this MemberInfo This) 
            => This.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? This.Name;
    }
}