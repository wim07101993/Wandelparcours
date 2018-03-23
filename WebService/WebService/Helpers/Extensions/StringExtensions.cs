using System.Text;
using WebService.Models;

namespace WebService.Helpers.Extensions
{
    /// <summary>
    /// StringExtensions is a static class that holds extension methods for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// ToLowerCamelCase converts a string to its camel case variant with a lower case first letter.
        /// <para/>
        /// It removes any leading and trailing spaces. If there are spaces in between, those are removed and the letter
        /// after the space is made upper case (camel case).
        /// </summary>
        /// <param name="This">is the <see cref="string"/> to convert</param>
        /// <returns>The camel case string</returns>
        public static string ToLowerCamelCase(this string This)
        {
            // remove leading and trailing white spaces
            This = This.Trim();

            // check for the length
            switch (This.Length)
            {
                // if the length of the string is 0, just return it
                case 0:
                    return This;
                // if the length is 1, make it lower case (the first letter should be lower case)
                case 1:
                    return This.ToLower();
                // else convert the string
                default:
                    // create a new string builder
                    var ret = new StringBuilder();
                    // add the first character in it's lower case variant
                    ret.Append(char.ToLower(This[0]));

                    // a variable to indicate if the next letter should be upper case (after a space)
                    var nextShouldBeCaps = false;
                    for (var i = 1; i < This.Length; i++)
                    {
                        // check for the letter
                        switch (This[i])
                        {
                            // if it is a white space, it should not be added but the next letter should be upper case
                            case ' ':
                                nextShouldBeCaps = true;
                                break;
                            // else add the letter (in caps if needed)
                            default:
                                if (!nextShouldBeCaps)
                                    // add letter
                                    ret.Append(This[i]);
                                else
                                {
                                    // convert to upper case and add
                                    ret.Append(char.ToUpper(This[i]));
                                    // reset variable (else everything will be upper case)
                                    nextShouldBeCaps = false;
                                }

                                break;
                        }
                    }

                    // return the string
                    return ret.ToString();
            }
        }

        /// <summary>
        /// ToUpperCamelCase converts a string to its camel case variant with a upper case first letter.
        /// <para/>
        /// It removes any leading and trailing spaces. If there are spaces in between, those are removed and the letter
        /// after the space is made upper case (camel case).
        /// </summary>
        /// <param name="This">is the <see cref="string"/> to convert</param>
        /// <returns>The camel case string</returns>
        public static string ToUpperCamelCase(this string This)
        {
            // remove leading and trailing white spaces
            This = This.Trim();

            // check for the length
            switch (This.Length)
            {
                // if the length of the string is 0, just return it
                case 0:
                    return This;
                // if the length is 1, make it lower case (the first letter should be lower case)
                case 1:
                    return This.ToLower();
                // else convert the string
                default:
                    // create a new string builder
                    var ret = new StringBuilder();
                    // add the first character in it's lower case variant
                    ret.Append(char.ToUpper(This[0]));

                    // a variable to indicate if the next letter should be upper case (after a space)
                    var nextShouldBeCaps = false;
                    for (var i = 1; i < This.Length; i++)
                    {
                        // check for the letter
                        switch (This[i])
                        {
                            // if it is a white space, it should not be added but the next letter should be upper case
                            case ' ':
                                nextShouldBeCaps = true;
                                break;
                            // else add the letter (in caps if needed)
                            default:
                                if (!nextShouldBeCaps)
                                    // add letter
                                    ret.Append(This[i]);
                                else
                                {
                                    // convert to upper case and add
                                    ret.Append(char.ToUpper(This[i]));
                                    // reset variable (else everything will be upper case)
                                    nextShouldBeCaps = false;
                                }

                                break;
                        }
                    }

                    // return the string
                    return ret.ToString();
            }
        }

        /// <summary>
        /// EqualsWithCamelCasing compares two <see cref="string"/>s using the <see cref="ToLowerCamelCase"/> method.
        /// </summary>
        /// <param name="This">is the <see cref="string"/> to compare with</param>
        /// <param name="propertyName">is the <see cref="StringBuilder"/> to compare</param>
        /// <returns>
        /// + true if the <see cref="string"/>'s lower camel case variants are equal
        /// + false if not
        /// </returns>
        public static bool EqualsWithCamelCasing(this string This, string propertyName)
            // check if the string's are equal after converting them to lower case
            => This.ToLowerCamelCase() == propertyName.ToLowerCamelCase();

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
    }
}