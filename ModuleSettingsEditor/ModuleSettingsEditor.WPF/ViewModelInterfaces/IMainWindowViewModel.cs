using System.Windows.Input;
using ModuleSettingsEditor.WPF.Models;

namespace ModuleSettingsEditor.WPF.ViewModelInterfaces
{
    public interface IMainWindowViewModel
    {
        Settings Settings { get; set; }

        ICommand SaveCommand { get; }
        ICommand OpenCommand { get; }
    }
}