using System;

namespace DatabaseImporter.Helpers.Extensions
{
    public static class EDestinationExtensions
    {
        public static bool IsFileDestination(this EDestination This)
        {
            switch (This)
            {
                case EDestination.Json:
                case EDestination.Csv:
                case EDestination.Xml:
                    return true;
                case EDestination.MongoDB:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool IsDatabaseDestination(this EDestination This)
        {
            switch (This)
            {
                case EDestination.Json:
                case EDestination.Csv:
                case EDestination.Xml:
                    return false;
                case EDestination.MongoDB:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}