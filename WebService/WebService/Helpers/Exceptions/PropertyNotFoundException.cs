namespace WebService.Helpers.Exceptions
{
    public class PropertyNotFoundException<T> : ArgumentException
    {
        public PropertyNotFoundException()
            : base($"Could not find property on type {typeof(T).Name}")
        {
        }

        public PropertyNotFoundException(string propertyName)
            : base($"The property {propertyName} could not be found on type {typeof(T).Name}.")
        {
        }
    }
}