using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Popups;
using ModuleSettingsEditor.Helpers.Extensions;
using ModuleSettingsEditor.Helpers.Mvvm;
using ModuleSettingsEditor.Helpers.Mvvm.Commands;
using ModuleSettingsEditor.Models;
using Newtonsoft.Json;

namespace ModuleSettingsEditor.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IDictionary<string, object> _properties;


        public MainWindowViewModel()
        {
            Settings = new Settings();

            SaveCommand = new DelegateCommand<IStorageFile>(async x => { await SaveSettingsToFileAsync(x, Settings); });
            OpenCommand = new DelegateCommand<IStorageFile>(async x => await OpenSettingsFromFile(x));
        }


        public Settings Settings
        {
            get => CreateSettingsFromDictionary(Properties);
            set => Properties = CreateDictionaryFromSettings(value);
        }

        public IDictionary<string, object> Properties
        {
            get => _properties;
            set => SetProperty(ref _properties, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }


        private static IDictionary<string, object> CreateDictionaryFromSettings(Settings settings)
            => typeof(Settings)
                .GetProperties()
                .Where(x => x.IsBrowsable())
                .ToDictionary(
                    keySelector: property => property.GetDisplayName(),
                    elementSelector: property => property.GetValue(settings));

        private static Settings CreateSettingsFromDictionary(IDictionary<string, object> properties)
        {
            var settings = new Settings();

            var settingsProperties = typeof(Settings).GetProperties();
            foreach (var property in properties)
                settingsProperties
                    .FirstOrDefault(x => x.GetDisplayName() == property.Key)
                    ?.SetValue(settings, property.Value);

            return settings;
        }

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