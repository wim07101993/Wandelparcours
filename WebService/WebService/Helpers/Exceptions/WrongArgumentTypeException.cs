using System;
using System.Runtime.Serialization;

namespace WebService.Helpers.Exceptions
{
    public class WrongArgumentTypeException : WebArgumentException
    {
        public Type ExpectedType { get; set; }
        public Type ReceivedType { get; set; }

        /// <inheritdoc cref="ArgumentException()" />
        /// <summary>
        /// Initializes a new instance of the <see cref="WebArgumentException"></see> class.
        /// </summary>
        public WrongArgumentTypeException()
        {
        }

        /// <inheritdoc cref="ArgumentException(string, string)"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="WebArgumentException"></see> class with a specified error
        /// message and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="expectedType">The expected type</param>
        /// <param name="receivedType">The type of the parameter</param>
        public WrongArgumentTypeException(string message, Type expectedType, Type receivedType) : base(message)
        {
            ExpectedType = expectedType;
            ReceivedType = receivedType;
        }
    }
}