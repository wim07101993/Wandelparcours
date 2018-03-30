using System.Collections.Generic;
using System.Windows.Input;
using ModuleSettingsEditor.WPF.Models;

namespace ModuleSettingsEditor.WPF.ViewModelInterfaces
{
    public interface IMainWindowViewModel
    {
        IEnumerable<string> PossibleKioskTypes { get; }

        Settings Settings { get; set; }

        ICommand SaveCommand { get; }
        ICommand OpenCommand { get; }
    }
}