using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseImporter.Helpers;
using DatabaseImporter.ViewModelInterfaces;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;

namespace DatabaseImporter.ViewModels
{
    public class SourceViewModel : BindableBase, ISourceViewModel
    {
        #region FIELDS

        private string _selectedSource = ESource.Json.ToString();
        private string _filePath;
        private object _value;
        private string _connectionString;

        #endregion FIELDS


        #region CONSTRUCTOR

        public SourceViewModel()
        {
            ChooseFileCommand = new DelegateCommand(ChooseFile);
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        public IEnumerable<string> Sources { get; } = Enum.GetNames(typeof(ESource));

        public string SelectedSource
        {
            get => _selectedSource;
            set
            {
                if (SetProperty(ref _selectedSource, value))
                    return;

                RaisePropertyChanged(nameof(SelectedESource));
                RaisePropertyChanged(nameof(UserNeedsToChooseFile));
            }
        }

        private ESource SelectedESource
            => Enum.TryParse(SelectedSource, out ESource ret)
                ? ret
                : ESource.Json;

        public bool UserNeedsToChooseFile
        {
            get
            {
                switch (SelectedESource)
                {
                    case ESource.Json:
                    case ESource.Csv:
                        return true;
                    case ESource.MongoDB:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool UserNeedsToInputConnectionString
        {
            get
            {
                switch (SelectedESource)
                {
                    case ESource.Json:
                    case ESource.Csv:
                        return false;
                    case ESource.MongoDB:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public string ConnectionString
        {
            get => _connectionString;
            set => SetProperty(ref _connectionString, value);
        }

        public ICommand ChooseFileCommand { get; }

        public object Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        #endregion PROPERTIES


        #region METHODS

        private void ChooseFile()
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false
            };

            switch (SelectedESource)
            {
                case ESource.Csv:
                    dialog.Filter = "Csv Bestanden (*.csv)|*.csv|Alle bestanden (*.*)|*.*";
                    break;
                case ESource.Json:
                    dialog.Filter = "Json Bestanden (*.json;*.js)|*.json;*.js|Alle bestanden (*.*)|*.*";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (dialog.ShowDialog() != true)
                return;

            FilePath = dialog.FileName;
#pragma warning disable 4014 // no await
            ReadFileAsync();
#pragma warning restore 4014
        }

        private async Task ReadFileAsync()
        {
            string json;
            using (var stream = File.OpenText(FilePath))
            {
                json = await stream.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                Value = null;
                return;
            }

            Value = JsonConvert.DeserializeObject(json);
        }

        #endregion METHODS
    }
}