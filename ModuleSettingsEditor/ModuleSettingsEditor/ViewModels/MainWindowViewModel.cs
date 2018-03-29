using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Popups;
using ModuleSettingsEditor.Helpers;
using ModuleSettingsEditor.Helpers.Extensions;
using ModuleSettingsEditor.Helpers.Mvvm;
using ModuleSettingsEditor.Helpers.Mvvm.Commands;
using ModuleSettingsEditor.Models;
using ModuleSettingsEditor.ViewModelInterfaces;
using Newtonsoft.Json;

namespace ModuleSettingsEditor.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        private Settings _settings;
        private IEnumerable<Property> _properties;


        public MainWindowViewModel()
        {
            Settings = new Settings();

            SaveCommand = new DelegateCommand<IStorageFile>(async x => await SaveSettingsToFileAsync(x, Settings));
            OpenCommand = new DelegateCommand<IStorageFile>(async x => Settings = await OpenSettingsFromFile(x));
        }


        public Settings Settings
        {
            get => _settings;
            set
            {
                if (!SetProperty(ref _settings, value))
                    return;
                Properties = CreatePropertiesFromSettings(value);
            }
        }

        public IEnumerable<Property> Properties
        {
            get => _properties;
            set => SetProperty(ref _properties, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }


        private static IEnumerable<Property> CreatePropertiesFromSettings(Settings settings)
            => typeof(Settings)
                .GetProperties()
                .Where(x => x.IsBrowsable())
                .Select(x => new Property(x, settings));
        
        private static async Task<Settings> OpenSettingsFromFile(IStorageFile file)
        {
            try
            {
                var json = await FileIO.ReadTextAsync(file);

                return json.Deserialize<Settings>();
            }
            catch (FileNotFoundException)
            {
                var dialog = new MessageDialog(
                    "Het geselecteerde bestand kon niet gevonden worden.",
                    "Bestand niet gevonden");
                await dialog.ShowAsync();
            }
            catch (JsonException)
            {
                var dialog = new MessageDialog(
                    "De inhoud van het bestand is van een verkeerd formaat.",
                    "Fout formaat");
                await dialog.ShowAsync();
            }

            return new Settings();
        }

        private static async Task SaveSettingsToFileAsync(IStorageFile file, Settings settings)
        {
            var json = settings.Serialize();

            await FileIO.WriteTextAsync(file, json);
        }
    }
}