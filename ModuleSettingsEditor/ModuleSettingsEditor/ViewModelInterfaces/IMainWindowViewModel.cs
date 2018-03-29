using System.Collections.Generic;
using System.Windows.Input;
using ModuleSettingsEditor.Models;

namespace ModuleSettingsEditor.ViewModelInterfaces
{
    public interface IMainWindowViewModel
    {
        Settings Settings { get; set; }
        IDictionary<string, object> Properties { get; }

        ICommand SaveCommand { get; }
        ICommand OpenCommand { get; }
    }
}