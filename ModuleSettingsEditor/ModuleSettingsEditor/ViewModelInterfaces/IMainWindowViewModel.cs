using System.Collections.Generic;
using System.Windows.Input;
using ModuleSettingsEditor.Helpers;
using ModuleSettingsEditor.Models;

namespace ModuleSettingsEditor.ViewModelInterfaces
{
    public interface IMainWindowViewModel
    {
        Settings Settings { get; set; }
        IEnumerable<Property> Properties { get; }

        ICommand SaveCommand { get; }
        ICommand OpenCommand { get; }
    }
}