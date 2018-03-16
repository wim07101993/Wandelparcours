using System;
using WebService.Services.Data;

namespace WebService.Helpers.Exceptions
{
    public class DatabaseException : Exception
    {
        /// <inheritdoc cref="Exception()" />
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"></see> class.
        /// </summary>
        public DatabaseException()
        {
        }

        /// <inheritdoc cref="Exception(string)"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"></see> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="databaseMethod">The method used in the database</param>
        public DatabaseException(string message, EDatabaseMethod databaseMethod) : base(message)
        {
            DatabaseMethod = databaseMethod;
        }


        public EDatabaseMethod DatabaseMethod { get; set; }
    }
}