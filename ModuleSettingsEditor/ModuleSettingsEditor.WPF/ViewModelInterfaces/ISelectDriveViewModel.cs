using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace ModuleSettingsEditor.WPF.ViewModelInterfaces
{
    public interface ISelectDriveViewModel : INotifyPropertyChanged
    {
        IEnumerable<string> Drives { get; }

        string SelectedDrive { get; set; }

        bool Ok { get; set; }

        bool IsNoSelectedDriveVisible { get; set; }
        string NoDriveSelectedMessage { get; set; }

        ICommand OkCommand { get; }
        ICommand CancelCommand { get; }
        ICommand NoSelectedDriveOkCommand { get; }
    }
}