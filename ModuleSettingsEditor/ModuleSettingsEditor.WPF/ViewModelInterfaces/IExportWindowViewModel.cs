using System.Collections.Generic;
using System.ComponentModel;

namespace ModuleSettingsEditor.WPF.ViewModelInterfaces
{
    public interface IExportWindowViewModel: INotifyPropertyChanged
    {
        IEnumerable<string> Partitions { get; }
        string SelectedPartition { get; set }
    }
}