using System;
using WebService.Services.Data;

namespace WebService.Helpers.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException()
        {
        }

        public DatabaseException(EDatabaseMethod databaseMethod)
            : base($"Could not {databaseMethod.ToString()} to the database.")
        {
            DatabaseMethod = databaseMethod;
        }

        public DatabaseException(EDatabaseMethod databaseMethod, Exception e)
            : base(
                $"Something went wrong while trying to {databaseMethod.ToString()} an element to the database: \n{e.Message}")
        {
            DatabaseMethod = databaseMethod;
        }


        public EDatabaseMethod DatabaseMethod { get; set; }
    }
}