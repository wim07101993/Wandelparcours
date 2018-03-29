using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using ModuleSettingsEditor.Helpers.Extensions;
using ModuleSettingsEditor.Helpers.Mvvm;
using ModuleSettingsEditor.Helpers.Mvvm.Commands;
using ModuleSettingsEditor.Models;

namespace ModuleSettingsEditor.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private const string FileName = "settings.json";
        private Settings _settings;
        private IDictionary<string, object> _properties;


        public MainWindowViewModel()
        {
            Settings = new Settings();

            SaveCommand = new DelegateCommand(async () => await SaveSettingsToFileAsync(FileName, Settings));
            OpenCommand = new DelegateCommand<string>(async x => await OpenSettingsFromFile(x));
        }


        public Settings Settings
        {
            get => _settings;
            set
            {
                if (!SetProperty(ref _settings, value))
                    return;

                Properties = CreatePropertiesDictionary(Settings);
            }
        }

        public IDictionary<string, object> Properties
        {
            get => _properties;
            set => SetProperty(ref _properties, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }


        private static IDictionary<string, object> CreatePropertiesDictionary(Settings settings)
            => typeof(Settings)
                .GetProperties()
                .Where(x => x.IsBrowsable())
                .ToDictionary(
                    keySelector: property => property.GetDisplayName(),
                    elementSelector: property => property.GetValue(settings));

        private static async Task<Settings> OpenSettingsFromFile(string fileName)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                var json = await FileIO.ReadTextAsync(file);

                return json.Deserialize<Settings>();
            }
            catch (FileNotFoundException)
            {
                return new Settings();
            }
        }

        private static async Task SaveSettingsToFileAsync( string fileName, Settings settings)
        {
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.CreateFileAsync(fileName);
            var json = settings.Serialize();

            await FileIO.WriteTextAsync(file, json);
        }
    }
}