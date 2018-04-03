using System;
using System.IO;
using System.Threading.Tasks;
using DatabaseImporter.Models;
using Microsoft.Win32;

namespace DatabaseImporter.Services.FileIO
{
    public class FileService : IFileService
    {
        public async Task<File<string>> ReadFileAsync(string extensionFilter)
        {
            var dialog = new OpenFileDialog
            {
                Filter = extensionFilter,
                Title = "Bestand kiezen om te openen",
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() != true)
                return null;

            var path = dialog.FileName;
            string content;
            using (var stream = File.OpenText(path))
            {
                content = await stream.ReadToEndAsync();
            }

            return new File<string> {Content = content, Path = path};
        }

        public async Task WriteFileAsync(string content, string extensionFilter)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Bestand kiezen om naar op te slaan",
                Filter = extensionFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() != true)
                return;

            var path = dialog.FileName;
            using (var stream = File.CreateText(path))
            {
                await stream.WriteAsync(content);
            }
        }
    }
}