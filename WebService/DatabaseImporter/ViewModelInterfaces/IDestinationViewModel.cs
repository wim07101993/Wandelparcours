using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface IDestinationViewModel : INotifyPropertyChanged
    {
        IEnumerable<string> Destinations { get; }
        string SelectedDestination { get; set; }
        bool IsFileDestination { get; }
        bool IsDatabaseDestination { get; }

        string FilePath { get; }

        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string TableName { get; set; }

        ICommand ChooseFileCommand { get; }
    }
}