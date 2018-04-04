using System.Collections.Generic;
using System.ComponentModel;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface IMainWindowViewModel : INotifyPropertyChanged
    {
        IEnumerable<string> DataTypes { get; }
        string SelectedDataType { get; set; }

        ISourceViewModel SourceViewModel { get; }

        object FileContent { get; }
    }
}