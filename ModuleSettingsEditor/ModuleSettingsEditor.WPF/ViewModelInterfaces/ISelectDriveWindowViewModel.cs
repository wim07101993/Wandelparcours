using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace ModuleSettingsEditor.WPF.ViewModelInterfaces
{
    public interface ISelectDriveWindowViewModel : INotifyPropertyChanged
    {
        IEnumerable<string> Drives { get; }
        string SelectedDrive { get; set; }
        bool Ok { get; set; }

        ICommand OkCommand { get; }
        ICommand CancelCommand { get; }
    }
}