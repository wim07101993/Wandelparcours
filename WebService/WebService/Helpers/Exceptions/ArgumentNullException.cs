namespace WebService.Helpers.Exceptions
{
    public class ArgumentNullException : ArgumentException
    {
        public ArgumentNullException()
        {
        }

        public ArgumentNullException(string paramName) : base($"The parameter {paramName} was null.")
        {
        }
    }
}