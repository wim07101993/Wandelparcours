using System;
using System.Linq;

namespace WebService.Services.Randomizer
{
    /// <summary>
    /// Static class to use an object of the class <see cref="Randomizer"/>.
    /// </summary>
    public class Randomizer : IRandomizer
    {
        #region FIELDS

        // ReSharper disable once InconsistentNaming
        private static readonly char[] _Chars =
        {
            '$', '%', '#', '@', '!', '*', '?', ';', ':', '^', '&',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
        };

        #endregion FIELDS


        #region PROPERTIES

        /// <summary>
        /// The instance of the Random Class
        /// </summary>
        public Random Random { get; } = new Random();

        public static Randomizer Instance { get; } = new Randomizer();

        public char[] Chars => _Chars;

        #endregion PROEPRTIES


        #region METHODS

        /// <summary>
        /// Returns a random floating-point number that is greater or equal than 0.0, and less than 1.0.
        /// </summary>
        /// <returns></returns>
        public double NextDouble()
            => Random.NextDouble();

        /// <summary>
        /// Returns a non negative random integer.
        /// </summary>
        /// <returns></returns>
        public int Next()
            => Random.Next();

        /// <summary>
        /// Returns a non negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int Next(int maxValue)
            => Random.Next(maxValue);

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int Next(int minValue, int maxValue)
            => Random.Next(minValue, maxValue);

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>the filled array</returns>
        public byte[] Next(byte[] buffer)
        {
            Random.NextBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Returns a random string of the specified length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string NextString(int length)
        {
            var bytes = new byte[length];
            Next(bytes);

            return bytes.Aggregate("", (current, b) => current + b);
        }

        public char NextChar()
            => Chars[Next(Chars.Length)];

        #endregion METHODS

    }
}