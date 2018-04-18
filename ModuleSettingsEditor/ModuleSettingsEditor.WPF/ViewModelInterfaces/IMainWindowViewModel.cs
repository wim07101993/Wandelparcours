using System.Collections.Generic;
using System.Windows.Input;
using ModuleSettingsEditor.WPF.Models;

namespace ModuleSettingsEditor.WPF.ViewModelInterfaces
{
    public interface IMainWindowViewModel
    {
        IEnumerable<string> PossibleKioskTypes { get; }

        ISelectDriveViewModel DialogViewModel { get; }

        Settings Settings { get; set; }

        bool IsDriveDialogVisible { get; set; }

        ICommand OpenCommand { get; }
        ICommand SaveCommand { get; }

        ICommand ExportCommand { get; }
        ICommand ImportCommand { get; }

    }
}