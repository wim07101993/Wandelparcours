using System.Collections.Generic;
using System.Windows.Input;
using ModuleSettingsEditor.WPF.Models;

namespace ModuleSettingsEditor.WPF.ViewModelInterfaces
{
    public interface IMainWindowViewModel
    {
        IEnumerable<string> PossibleKioskTypes { get; }

        Settings Settings { get; set; }

        ICommand OpenCommand { get; }
        ICommand SaveCommand { get; }

        ICommand ExportCommand { get; }
        ICommand ImportCommand { get; }
    }
}