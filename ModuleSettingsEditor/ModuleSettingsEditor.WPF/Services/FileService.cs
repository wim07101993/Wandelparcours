using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ModuleSettingsEditor.WPF.Helpers.Extensions;
using Newtonsoft.Json;

namespace ModuleSettingsEditor.WPF.Services
{
    public class FileService<T> : IFileService<T>
    {
        public const string PiBootDir = "boot";
        private const string Extension = ".json";


        public string ExportDir => PiBootDir;

        public async Task<T> OpenAsync(string path)
        {
            try
            {
                return await OpenFileAsync(path);
            }
            catch (JsonException)
            {
                MessageBox.Show(
                    "De inhoud van het opgegeven bestand is in een verkeerd formaat, het kon niet worden ingelezen",
                    "Fout bestandsformaat",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return default(T);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    "Het opgevegen bestand bestaat niet.",
                    "Bestand niet gevonden",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return default(T);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show(
                    "De opgevegen map bestaat niet.",
                    "Map niet gevonden",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return default(T);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Er is iets mis gelopen.",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Debug.WriteLine($"ERROR: {e.Message}");

                return default(T);
            }
        }

        public async Task SaveAsync(T value, string path)
        {
            try
            {
                await WriteFileAsync(value, path);
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
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show(
                    "De opgevegen map bestaat niet.",
                    "Map niet gevonden",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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


        public async Task<T> ImportAsync(string drive)
        {
            try
            {
                return await OpenFileAsync($"{drive}{PiBootDir}{typeof(T).Name}{Extension}");
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show(
                    "Het opgegeven station bevat niet de nodige mappen van eem Pi",
                    "Map niet gevonden",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return default(T);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Er is iets mis gelopen.",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Debug.WriteLine($"ERROR: {e.Message}");

                return default(T);
            }
        }

        public async Task ExportAsync(T value, string drive)
        {
            try
            {
                await WriteFileAsync(value, $"{drive}{PiBootDir}/{typeof(T).Name}{Extension}");
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show(
                    "Het opgegeven station bevat niet de nodige mappen van een Pi",
                    "Map niet gevonden",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Er is iets mis gelopen.",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Debug.WriteLine($"ERROR: {e.Message}");
            }
        }


        private static async Task<T> OpenFileAsync(string path)
        {
            string json;
            using (var stream = File.OpenText(path))
            {
                json = await stream.ReadToEndAsync();
            }

            return string.IsNullOrWhiteSpace(json)
                ? default(T)
                : json.Deserialize<T>();
        }

        private static async Task WriteFileAsync(T value, string path)
        {
            var json = value.Serialize();
            var bytes = Encoding.UTF8.GetBytes(json);

            using (var stream = File.OpenWrite(path))
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}