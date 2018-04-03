using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface ISourceViewModel : INotifyPropertyChanged
    {
        IEnumerable<string> DataTypes { get; }
        string SelectedSource { get; set; }

        IEnumerable<string> Sources { get; }
        string SelectedDataType { get; set; }
        bool IsFileSource { get; }
        bool IsDatabaseSource { get; }

        bool UserNeedsToInputConnectionString { get; }

        string FilePath { get; }
        string ConnectionString { get; set; }

        ICommand ChooseFileCommand { get; }

        object Value { get; set; }
    }
}