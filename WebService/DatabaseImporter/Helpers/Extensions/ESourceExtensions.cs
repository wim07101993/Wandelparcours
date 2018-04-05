using System;

namespace DatabaseImporter.Helpers.Extensions
{
    public static class ESourceExtensions
    {
        public static bool IsFileSource(this ESource This)
        {
            switch (This)
            {
                case ESource.Json:
                case ESource.Csv:
                case ESource.Xml:
                    return true;
                case ESource.MongoDB:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool IsDatabaseSource(this ESource This)
        {
            switch (This)
            {
                case ESource.Json:
                case ESource.Csv:
                case ESource.Xml:
                    return true;
                case ESource.MongoDB:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}