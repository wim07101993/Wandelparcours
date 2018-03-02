namespace WebService.Helpers
{
    public static class StringExtensions
    {
        public static string ToLowerCamelCase(this string This)
        {
            switch (This.Length)
            {
                case 0:
                    return This;
                case 1:
                    return This.ToLower();
                default:
                    return $"{char.ToLower(This[0])}{This.Substring(1)}";
            }
        }

        public static bool EqualsWithCamelCasing(this string This, string propertyName)
            => This == propertyName ||
               This.ToLowerCamelCase() == propertyName.ToLowerCamelCase();
    }
}