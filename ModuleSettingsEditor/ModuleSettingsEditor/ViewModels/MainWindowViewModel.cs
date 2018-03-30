using System.Windows.Input;
using Windows.Storage;
using ModuleSettingsEditor.Helpers.Mvvm;
using ModuleSettingsEditor.Helpers.Mvvm.Commands;
using ModuleSettingsEditor.Models;
using ModuleSettingsEditor.Services;
using ModuleSettingsEditor.ViewModelInterfaces;

namespace ModuleSettingsEditor.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        #region FIELDS

        private ISettings _settings;

        #endregion FIELDS


        #region CONSTRUCTOR

        public MainWindowViewModel(IDataService<ISettings> dataService)
        {
            Settings = new Settings();

            SaveCommand = new DelegateCommand<IStorageFile>(async x => await dataService.SaveAsync(Settings));
            OpenCommand = new DelegateCommand<IStorageFile>(async x => Settings = await dataService.GetAsync());
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        //#region settings

        //public string RestServerIP
        //{
        //    get => Settings.RestServerIP;
        //    set => Settings.RestServerIP = value;
        //}

        //public int RestServerPort
        //{
        //    get => Settings.RestServerPort;
        //    set => Settings.RestServerPort = value;
        //}

        //public string SocketIP
        //{
        //    get => Settings.SocketIP;
        //    set => Settings.SocketIP = value;
        //}

        //public int SocketPort
        //{
        //    get => Settings.SocketPort;
        //    set => Settings.SocketPort = value;
        //}

        //public string WifiSSID
        //{
        //    get => Settings.WifiSSID;
        //    set => Settings.WifiSSID = value;
        //}

        //public string WifiWPA
        //{
        //    get => Settings.WifiWPA;
        //    set => Settings.WifiWPA = value;
        //}

        //#endregion settings


        public ISettings Settings
        {
            get => _settings;
            set
            {
                if (Equals(_settings, value))
                    return;

                //if (_settings != null)
                //    _settings.PropertyChanged -= OnSettingsPropertyChanged;

                _settings = value;

                //if (_settings != null)
                //    _settings.PropertyChanged += OnSettingsPropertyChanged;

                //RaisePropertyChanged();

                //RaisePropertyChanged(nameof(RestServerIP));
                //RaisePropertyChanged(nameof(RestServerPort));

                //RaisePropertyChanged(nameof(SocketIP));
                //RaisePropertyChanged(nameof(SocketPort));

                //RaisePropertyChanged(nameof(WifiSSID));
                //RaisePropertyChanged(nameof(WifiWPA));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }

        #endregion PROPERTIES


        #region METHODS

        //private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (typeof(ISettings).GetProperties().Select(x => x.Name).Contains(e.PropertyName))
        //        RaisePropertyChanged(e.PropertyName);
        //}

        #endregion METHODS
    }
}