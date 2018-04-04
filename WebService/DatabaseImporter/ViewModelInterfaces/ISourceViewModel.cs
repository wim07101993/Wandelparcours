using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface ISourceViewModel : INotifyPropertyChanged
    {
        IEnumerable<string> Sources { get; }
        string SelectedSource { get; set; }
        bool IsFileSource { get; }
        bool IsDatabaseSource { get; }

        string FilePath { get; }
        string ConnectionString { get; set; }

        ICommand ChooseFileCommand { get; }
    }
}