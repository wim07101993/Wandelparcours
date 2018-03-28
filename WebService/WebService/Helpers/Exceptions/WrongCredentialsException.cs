namespace WebService.Helpers.Exceptions
{
    public class WrongCredentialsException : ArgumentException
    {
        public WrongCredentialsException()
            : base("The entered user name and/or password do not match")
        {
        }
    }
}