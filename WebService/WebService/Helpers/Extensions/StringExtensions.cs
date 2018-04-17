using System.Linq;
using System.Text;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Services.Randomizer;
using WebService.Models;

namespace WebService.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerCamelCase(this string This)
        {
            This = This.Trim();
            switch (This.Length)
            {
                case 0:
                    return This;
                case 1:
                    return This.ToLower();
                default:
                    var ret = new StringBuilder();
                    ret.Append(char.ToLower(This[0]));

                    var nextShouldBeCaps = false;
                    for (var i = 1; i < This.Length; i++)
                    {
                        switch (This[i])
                        {
                            case ' ':
                                nextShouldBeCaps = true;
                                break;
                            default:
                                if (!nextShouldBeCaps)
                                    ret.Append(This[i]);
                                else
                                {
                                    ret.Append(char.ToUpper(This[i]));
                                    nextShouldBeCaps = false;
                                }

                                break;
                        }
                    }

                    return ret.ToString();
            }
        }

        public static string ToUpperCamelCase(this string This)
        {
            This = This.Trim();
            switch (This.Length)
            {
                case 0:
                    return This;
                case 1:
                    return This.ToLower();
                default:
                    var ret = new StringBuilder();
                    ret.Append(char.ToUpper(This[0]));

                    var nextShouldBeCaps = false;
                    for (var i = 1; i < This.Length; i++)
                    {
                        switch (This[i])
                        {
                            case ' ':
                                nextShouldBeCaps = true;
                                break;
                            default:
                                if (!nextShouldBeCaps)
                                    ret.Append(This[i]);
                                else
                                {
                                    ret.Append(char.ToUpper(This[i]));
                                    nextShouldBeCaps = false;
                                }

                                break;
                        }
                    }

                    return ret.ToString();
            }
        }

        public static bool EqualsWithCamelCasing(this string This, string propertyName)
            => This.ToLowerCamelCase() == propertyName.ToLowerCamelCase();

        public static string Hash(this string stringToHash, ObjectId id, bool useSalt = true, bool usePepper = true)
        {
            if (useSalt && usePepper)
                return BCrypt.Net.BCrypt.HashPassword($"{stringToHash}{id}{Randomizer.Instance.NextChar()}");
            if (useSalt)
                return BCrypt.Net.BCrypt.HashPassword($"{stringToHash}{id}");
            if (usePepper)
                return BCrypt.Net.BCrypt.HashPassword($"{stringToHash}{Randomizer.Instance.NextChar()}");
            return BCrypt.Net.BCrypt.HashPassword(stringToHash);
        }

        public static bool EqualsToHash(this string stringToCompare, ObjectId id, string hash, bool useSalt = true,
            bool usePepper = true)
        {
            if (useSalt && usePepper)
                return Randomizer.Instance.Chars.Any(c => BCrypt.Net.BCrypt.Verify($"{stringToCompare}{id}{c}", hash));
            if (useSalt)
                return BCrypt.Net.BCrypt.Verify($"{stringToCompare}{id}", hash);
            if (usePepper)
                return Randomizer.Instance.Chars.Any(c => BCrypt.Net.BCrypt.Verify($"{stringToCompare}{c}", hash));
            return BCrypt.Net.BCrypt.Verify(stringToCompare, hash);
        }

        public static EMediaType GetEMediaTypeFromExtension(this string This)
        {
            switch (This)
            {
                case "jpg":
                case "bmp":
                case "png":
                case "jpeg":
                case "gif":
                case "webp":
                    return EMediaType.Image;
                case "midi":
                case "mp3":
                case "mpeg":
                case "wav":
                case "m4a":
                case "aac":
                    return EMediaType.Audio;
                case "mp4":
                case "avi":
                case "webm":
                case "ogg":
                case "flv":
                case "wmv":
                case "mkv":
                    return EMediaType.Video;
                default:
                    return EMediaType.Image;
            }
        }

        public static ObjectId ToObjectId(this string This)
        {
            if (!ObjectId.TryParse(This, out var objectId))
                throw new ArgumentException($"Could not convert {This} to an ObjectId");
            return objectId;
        }
    }
}