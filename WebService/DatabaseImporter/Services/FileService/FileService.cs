using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DatabaseImporter.Helpers.Extensions;
using Newtonsoft.Json;

namespace DatabaseImporter.Services.FileService
{
    public class FileService<T> : IFileService<T>
    {
        public async Task<T> OpenJsonAsync(string path)
        {
            try
            {
                var json = await ReadFileAsync(path);

                return string.IsNullOrWhiteSpace(json)
                    ? default(T)
                    : json.Deserialize<T>();
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
        }

        public async Task SaveJsonAsync(T value, string path)
        {
            try
            {
                await WriteFileAsync(value.Serialize(), path);
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
        }


        public Task<T> OpenCsvAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task SaveCsvAsync(T value, string path)
        {
            throw new NotImplementedException();
        }


        public async Task<string> ReadFileAsync(string path)
        {
            try
            {
                string content;
                using (var stream = File.OpenText(path))
                {
                    content = await stream.ReadToEndAsync();
                }

                return content;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    "Het opgevegen bestand bestaat niet.",
                    "Bestand niet gevonden",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return null;
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show(
                    "De opgevegen map bestaat niet.",
                    "Map niet gevonden",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Er is iets mis gelopen.",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Debug.WriteLine($"ERROR: {e.Message}");

                return null;
            }
        }

        public async Task WriteFileAsync(string content, string path)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(content);

                using (var stream = File.OpenWrite(path))
                {
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    "Het opgevegen bestand bestaat niet.",
                    "Bestand niet gevonden",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return null;
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
    }
}