using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using ModuleSettingsEditor.Helpers.Extensions;
using ModuleSettingsEditor.Models;
using Newtonsoft.Json;

namespace ModuleSettingsEditor.Services
{
    public class DataService : IDataService<ISettings>
    {
        public async Task<ISettings> GetAsync()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = { ".json", ".js" }
            };

            var file = await picker.PickSingleFileAsync();

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

        public async Task SaveAsync(ISettings value)
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeChoices = { { "Json bestand", new List<string> { ".json" } } },
                SuggestedFileName = "Settings"
            };

            var file = await picker.PickSaveFileAsync();

            if (file == null)
                return;

            var json = value.Serialize();

            await FileIO.WriteTextAsync(file, json);
        }
    }
}
