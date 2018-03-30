using System.Windows.Input;
using ModuleSettingsEditor.Models;

namespace ModuleSettingsEditor.ViewModelInterfaces
{
    public interface IMainWindowViewModel
    {
        ISettings Settings { get; set; }

        ICommand SaveCommand { get; }
        ICommand OpenCommand { get; }
    }
}