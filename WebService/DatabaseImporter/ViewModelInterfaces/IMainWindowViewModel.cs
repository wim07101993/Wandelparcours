using System.ComponentModel;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface IMainWindowViewModel : INotifyPropertyChanged
    {
        ISourceViewModel SourceViewModel { get; }
    }
}