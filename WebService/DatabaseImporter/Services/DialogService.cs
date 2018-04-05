using System;
using Microsoft.Win32;

namespace DatabaseImporter.Services
{
    public class DialogService : IDialogService
    {
        public string OpenFileDialog(string extensionFilter)
        {
            var dialog = new OpenFileDialog
            {
                Filter = extensionFilter,
                Title = "Bestand kiezen om te openen",
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            return dialog.ShowDialog() != true
                ? null
                : dialog.FileName;
        }

        public string WriteFileDialog(string extensionFilter)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Bestand kiezen om naar op te slaan",
                Filter = extensionFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            return dialog.ShowDialog() != true
                ? null
                : dialog.FileName;
        }
    }
}