using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using DatabaseImporter.Services.Data;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface ISourceViewModel : INotifyPropertyChanged
    {
        IEnumerable<string> Sources { get; }
        string SelectedSource { get; set; }
        bool IsFileSource { get; }
        bool IsDatabaseSource { get; }

        string FilePath { get; }

        string IpAddress { get; set; }
        string DatabaseName { get; set; }
        string TableName { get; set; }

        ICommand ImportCommand { get; }

        IDatabaseService DatabaseService { get; }
    }
}