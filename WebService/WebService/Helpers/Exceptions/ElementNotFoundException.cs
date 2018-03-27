namespace WebService.Helpers.Exceptions
{
    public class ElementNotFoundException<TOwner> : NotFoundException
    {
        public ElementNotFoundException()
        {
        }

        public ElementNotFoundException(string listPropertyName, string elementName)
            : base($"The {elementName} is not found in the {listPropertyName} of {typeof(TOwner).Name}")
        {
        }
    }
}