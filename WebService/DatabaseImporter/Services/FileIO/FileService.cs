using System;
using System.IO;
using System.Threading.Tasks;
using DatabaseImporter.Models;
using Microsoft.Win32;

namespace DatabaseImporter.Services.FileIO
{
    public class FileService : IFileService
    {
        public async Task<File<string>> ReadFileWithDialogAsync(string extensionFilter)
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

            return await ReadFileAsync(dialog.FileName);
        }

        public async Task<File<string>> ReadFileAsync(string filePath)
        {
            string content;
            using (var stream = File.OpenText(filePath))
            {
                content = await stream.ReadToEndAsync();
            }

            return new File<string> {Content = content, Path = filePath};
        }


        public async Task WriteFileWithDialogsAsync(string content, string extensionFilter)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Bestand kiezen om naar op te slaan",
                Filter = extensionFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() != true)
                return;

            await WriteFileAsync(dialog.FileName, content);
        }

        public async Task WriteFileAsync(string filePath, string content)
        {
            using (var stream = File.CreateText(filePath))
            {
                await stream.WriteAsync(content);
            }
        }
    }
}