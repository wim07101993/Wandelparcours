using System;

namespace WebService.Helpers.Exceptions
{
    /// <inheritdoc cref="Exception" />
    /// <summary>
    /// The exception that is thrown when a page or item cannot be found.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <inheritdoc cref="ArgumentException()" />
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"></see> class.
        /// </summary>
        public NotFoundException()
        {
        }

        /// <inheritdoc cref="ArgumentException(string)"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"></see> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public NotFoundException(string message) : base(message)
        {
        }

        /// <inheritdoc cref="ArgumentException(string, Exception)"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"></see> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the innerException parameter is not a null reference, 
        /// the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}