using System.Collections.Generic;
using System.Linq;
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
            OpenCommand = new DelegateCommand(async () =>
            {
                var openedSettings = await dataService.OpenAsync();
                if (openedSettings != null)
                    Settings = openedSettings;
            });
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        public IEnumerable<string> PossibleKioskTypes { get; } =
            new[] {"Tag detector", "Video", "Afbeelding", "Muziek", "Spel"};

        public Settings Settings
        {
            get => _settings;
            set
            {
                if (!SetProperty(ref _settings, value))
                    return;

                if (PossibleKioskTypes.All(x => x != Settings.KioskType))
                    Settings.KioskType = PossibleKioskTypes.First();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }

        #endregion PROPERTIES
    }
}