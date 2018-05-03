using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using ModuleSettingsEditor.WPF.Helpers.Events;
using ModuleSettingsEditor.WPF.Models;
using ModuleSettingsEditor.WPF.Services;
using ModuleSettingsEditor.WPF.ViewModelInterfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace ModuleSettingsEditor.WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        #region FIELDS

        private readonly IFileService<Settings> _fileService;

        private Settings _settings;
        private bool _isDriveDialogVisible;

        private bool _isImporting;
        private bool _isExporting;

        #endregion FIELDS


        #region CONSTRUCTOR

        public MainWindowViewModel(IFileService<Settings> fileService, IEventAggregator eventAggregator)
        {
            _fileService = fileService;
            Settings = new Settings();

            DialogViewModel = App.Bootstrapper.Container.Resolve<ISelectDriveViewModel>();

            OpenCommand = new DelegateCommand(async () => await OpenSettings());
            SaveCommand = new DelegateCommand(async () => await SaveSettings());

            ImportCommand = new DelegateCommand(() =>
            {
                _isImporting = true;
                DialogViewModel.NoDriveSelectedMessage = "Selecteer een station om van te importeren.";
                IsDriveDialogVisible = true;
            });
            ExportCommand = new DelegateCommand(() =>
            {
                _isExporting = true;
                DialogViewModel.NoDriveSelectedMessage = "Selecteer een station om naar te exporteren.";
                IsDriveDialogVisible = true;
            });

            eventAggregator
                .GetEvent<CloseDialogEvent>()
                .Subscribe(() => IsDriveDialogVisible = false);
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        public IEnumerable<string> PossibleKioskTypes { get; } =
            new[] {"Tag detector", "Video", "Afbeelding", "Muziek", "Spel"};

        public ISelectDriveViewModel DialogViewModel { get; }

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
        
        public bool IsDriveDialogVisible
        {
            get => _isDriveDialogVisible;
            set
            {
                if (!SetProperty(ref _isDriveDialogVisible, value))
                    return;

                if (IsDriveDialogVisible)
                    return;

                if (_isImporting)
#pragma warning disable 4014 // disable no await on task
                    ImportSettings();
                else if (_isExporting)
                    ExportSettings();
#pragma warning restore 4014
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
            _isImporting = false;
            IsDriveDialogVisible = false;

            
            await _fileService.ImportAsync(DialogViewModel.SelectedDrive);
        }

        public async Task ExportSettings()
        {
            _isExporting = false;
            IsDriveDialogVisible = false;
            await _fileService.ExportAsync(Settings, DialogViewModel.SelectedDrive);
        }

        #endregion METHODS
    }
}