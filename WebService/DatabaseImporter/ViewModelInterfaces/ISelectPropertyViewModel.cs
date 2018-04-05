using System.Reflection;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface ISelectPropertyViewModel : ISelectableViewModel
    {
        PropertyInfo Property { get; set; }
    }
}
