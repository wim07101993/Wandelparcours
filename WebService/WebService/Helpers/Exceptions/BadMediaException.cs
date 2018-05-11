namespace WebService.Helpers.Exceptions
{
    public class BadMediaException : ArgumentException
    {
        public BadMediaException()
            : base("The given media type is not acceptable.")
        {
        }
        public BadMediaException(string message)
            : base(message)
        {
        }
    }
}