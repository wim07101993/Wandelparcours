using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using ModuleSettingsEditor.WPF.Models;
using ModuleSettingsEditor.WPF.Services;
using ModuleSettingsEditor.WPF.ViewModelInterfaces;
using ModuleSettingsEditor.WPF.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Unity;

namespace ModuleSettingsEditor.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        #region FIELDS

        private readonly IFileService<Settings> _fileService;

        private Settings _settings;

        #endregion FIELDS


        #region CONSTRUCTOR

        public MainWindowViewModel(IFileService<Settings> fileService)
        {
            _fileService = fileService;
            Settings = new Settings();

            OpenCommand = new DelegateCommand(async () => await OpenSettings());
            SaveCommand = new DelegateCommand(async () => await SaveSettings());

            ImportCommand = new DelegateCommand(async () => await ImportSettings());
            ExportCommand = new DelegateCommand(async () => await ExportSettings());
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

        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }

        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }

        #endregion PROPERTIES


        #region METHODS

        public async Task OpenSettings()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open settings",
                FileName = "settings.json",
                Filter = "Json bestanden (*.json)|*.json|Javascript bestanden (*.js)|*.js|Alle bestanden (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false
            };

            if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.FileName))
                return;

            var openedSettings = await _fileService.OpenAsync(dialog.FileName);
            if (openedSettings != null)
                Settings = openedSettings;
        }

        public async Task SaveSettings()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Open settings",
                FileName = "settings.json",
                Filter = "Json bestanden (*.json)|*.json|Javascript bestanden (*.js)|*.js|Alle bestanden (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.FileName))
                return;

            await _fileService.SaveAsync(Settings, dialog.FileName);
        }

        public async Task ImportSettings()
        {
            var viewmodel = App.Bootstrapper.Container.TryResolve<ISelectDriveWindowViewModel>();
            var window = new SelectDriveWindow(viewmodel);

            window.ShowDialog();
            if (!viewmodel.Ok || string.IsNullOrWhiteSpace(viewmodel.SelectedDrive))
                return;

            await _fileService.ImportAsync(viewmodel.SelectedDrive);
        }

        public async Task ExportSettings()
        {
            var viewmodel = App.Bootstrapper.Container.TryResolve<ISelectDriveWindowViewModel>();
            var window = new SelectDriveWindow(viewmodel);

            window.ShowDialog();
            if (!viewmodel.Ok || string.IsNullOrWhiteSpace(viewmodel.SelectedDrive))
                return;

            await _fileService.ExportAsync(Settings, viewmodel.SelectedDrive);
        }

        #endregion METHODS
    }
}