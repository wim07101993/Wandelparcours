using System.Windows.Input;
using ModuleSettingsEditor.WPF.Models;
using ModuleSettingsEditor.WPF.Services;
using ModuleSettingsEditor.WPF.ViewModelInterfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace ModuleSettingsEditor.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        #region FIELDS

        private Settings _settings;

        #endregion FIELDS


        #region CONSTRUCTOR

        public MainWindowViewModel(IFileService<Settings> dataService)
        {
            Settings = new Settings();

            SaveCommand = new DelegateCommand(async () => await dataService.SaveAsync(Settings));
            OpenCommand = new DelegateCommand(async () => Settings = await dataService.OpenAsync());
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES
        
        public Settings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }

        #endregion PROPERTIES
    }
}