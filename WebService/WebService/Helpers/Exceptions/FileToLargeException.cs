namespace WebService.Helpers.Exceptions
{
    public class FileToLargeException : ArgumentException
    {
        public FileToLargeException()
        {
        }

        public FileToLargeException(int maxFileSize) : base($"The maximum file size ({maxFileSize}) is exceeded")
        {
        }
    }
}