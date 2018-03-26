using System;

namespace WebService.Helpers.Exceptions
{
    public class NotFoundException<T> : NotFoundException
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string identifier, string value)
            : base($"The {typeof(T).Name} with {identifier} {value} was not found.")
        {
        }
    }

    public abstract class NotFoundException : Exception
    {
        protected NotFoundException()
        {
        }

        protected NotFoundException(string message)
            : base(message)
        {
        }
    }
}