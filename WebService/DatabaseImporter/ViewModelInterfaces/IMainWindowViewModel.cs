using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface IMainWindowViewModel : INotifyPropertyChanged
    {
        ISourceViewModel SourceViewModel { get; }
        IDestinationViewModel DestinationViewModel { get; }
        IDataSelectionViewModel DataSelectionViewModels { get; }

        object FileContent { get; }

        ObservableCollection<Exception> Exceptions { get; }

        string CurrentTime { get; }
    }
}