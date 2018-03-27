using System;

namespace WebService.Helpers.Exceptions
{
    public class WrongArgumentTypeException : ArgumentException
    {
        public Type ExpectedType { get; set; }
        public Type ReceivedType { get; set; }

        public WrongArgumentTypeException()
        {
        }

        public WrongArgumentTypeException(string json, Type expectedType)
            : base($"The value could not be deserialized to a {expectedType.Name}. The received value :\n{json}")
        {
            ExpectedType = expectedType;
        }
    }
}