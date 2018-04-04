using System.Collections.Generic;
using System.Windows.Input;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface IDestinationViewModel
    {
        IEnumerable<string> Destinations { get; }
        string SelectedDestination { get; set; }
        bool IsFileDestination { get; }
        bool IsDatabaseDestination { get; }

        bool UserNeedsToInputConnectionString { get; }

        string FilePath { get; }
        string ConnectionString { get; set; }

        ICommand ChooseFileCommand { get; }
    }
}