using System;
using System.ComponentModel;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface IMainWindowViewModel : INotifyPropertyChanged
    {
        ISourceViewModel SourceViewModel { get; }
        IDestinationViewModel DestinationViewModel { get; }
        IDataSelectionViewModel DataSelectionViewModels { get; }

        object FileContent { get; }

        Exception Exception { get; }
    }
}