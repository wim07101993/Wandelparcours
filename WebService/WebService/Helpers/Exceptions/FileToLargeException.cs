using System;

namespace WebService.Helpers.Exceptions
{
    public class FileToLargeException : Exception
    {
        /// <inheritdoc cref="Exception()" />
        /// <summary>
        /// Initializes a new instance of the <see cref="FileToLargeException"></see> class.
        /// </summary>
        public FileToLargeException()
        {
        }

        /// <inheritdoc cref="Exception(string)"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="FileToLargeException"></see> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public FileToLargeException(string message) : base(message)
        {
        }

        /// <inheritdoc cref="Exception(string, Exception)"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="FileToLargeException"></see> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the innerException parameter is not a null reference, 
        /// the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public FileToLargeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
