using System.ComponentModel;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface ISelectableViewModel : INotifyPropertyChanged
    {
        bool IsSelected { get; set; }
        string Name { get; }
        string Description { get; }
    }
}