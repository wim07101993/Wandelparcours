using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using ModuleSettingsEditor.WPF.Helpers.Extensions;
using ModuleSettingsEditor.WPF.Models;
using Newtonsoft.Json;

namespace ModuleSettingsEditor.WPF.Services
{
    public class FileService<T> : IFileService<T>
    {
        public async Task<T> OpenAsync()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open settings",
                FileName = "settings.json",
                Filter = "Json bestanden (*.json)|*.json|Javascript bestanden (*.js)|*.js|Alle bestanden (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return default(T);

            var path = dialog.FileName;

            if (string.IsNullOrWhiteSpace(path))
                return default(T);

            string json;
            try
            {
                using (var stream = File.OpenText(path))
                {
                    json = await stream.ReadToEndAsync();
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    "Het opgevegen pad bestaat niet.",
                    "Bestand niet gevonden",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return default(T);
            }


            if (string.IsNullOrWhiteSpace(json))
                return default(T);

            try
            {
                return json.Deserialize<T>();
            }
            catch (JsonException e)
            {
                MessageBox.Show(
                    "De inhoud van het opgegeven bestand is in een verkeerd formaat, het kon niet worden ingelezen",
                    "Fout bestandsformaat",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return default(T);
            }
        }

        public async Task SaveAsync(T value)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Open settings",
                FileName = "settings.json",
                Filter = "Json bestanden (*.json)|*.json|Javascript bestanden (*.js)|*.js|Alle bestanden (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            if (dialog.ShowDialog() != true)
                return;

            var path = dialog.FileName;

            if (string.IsNullOrWhiteSpace(path))
                return;

            var json = "";

            try
            {
                json = value.Serialize();
            }
            catch (JsonException e)
            {
                MessageBox.Show(
                    $"Er liep iets mis met het schrijven naar het bestand.",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                Debug.WriteLine($"ERROR: {e.Message}");
            }

            var bytes = Encoding.UTF8.GetBytes(json);

            try
            {
                using (var stream = File.OpenWrite(path))
                {
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Er liep iets mis met het opslaan van het bestand.",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                Debug.WriteLine($"ERROR: {e.Message}");
            }
        }

        public Task<T> ImportAsync()
        {
            throw new NotImplementedException();
        }

        public Task ExportAsync(T settings)
        {
            throw new NotImplementedException();
        }
    }
}