namespace WebService.Helpers.Exceptions
{
    public class ArgumentException : System.ArgumentException
    {
        public ArgumentException()
        {
        }

        public ArgumentException(string message) : base(message)
        {
        }

        public ArgumentException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}