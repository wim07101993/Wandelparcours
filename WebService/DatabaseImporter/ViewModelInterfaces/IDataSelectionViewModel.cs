using System.Collections.Generic;
using System.ComponentModel;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface IDataSelectionViewModel : INotifyPropertyChanged
    {
        IEnumerable<ISelectPropertyViewModel> SelectableProperties { get; }

        IEnumerable<string> DataTypes { get; }
        string SelectedDataType { get; set; }
   }
}