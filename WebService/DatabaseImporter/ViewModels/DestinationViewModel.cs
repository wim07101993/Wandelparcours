using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseImporter.Helpers;
using DatabaseImporter.Models.MongoModels;
using DatabaseImporter.Services;
using DatabaseImporter.Services.FileIO;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Commands;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public class DestinationViewModel : BindableBase, IDestinationViewModel
    {
        #region FIELDS

        private readonly ICsvService _csvService;
        private readonly IJsonService _jsonService;
        private readonly IXmlService _xmlService;

        private string _selectedDestination = EDestination.Json.ToString();
        private string _filePath;
        private string _connectionString;

        #endregion FIELDS


        #region CONSTRUCTOR

        public DestinationViewModel(ICsvService csvService, IJsonService jsonService, IXmlService xmlService,
            IEventAggregator eventAggregator, IStateManager stateManager)
            : base(eventAggregator, stateManager)
        {
            _csvService = csvService;
            _jsonService = jsonService;
            _xmlService = xmlService;

            ChooseFileCommand = new DelegateCommand(ChooseFile);
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        public IEnumerable<string> Destinations { get; } = Enum.GetNames(typeof(EDestination));

        public string SelectedDestination
        {
            get => _selectedDestination;
            set
            {
                if (!SetProperty(ref _selectedDestination, value))
                    return;

                RaisePropertyChanged(nameof(SelectedEDestination));
                RaisePropertyChanged(nameof(IsFileDestination));
                RaisePropertyChanged(nameof(IsDatabaseDestination));
            }
        }

        private EDestination SelectedEDestination
            => Enum.TryParse(SelectedDestination, out EDestination ret)
                ? ret
                : EDestination.Json;

        public bool IsFileDestination
        {
            get
            {
                switch (SelectedDestination)
                {
                    case "Json":
                    case "Csv":
                    case "Xml":
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool IsDatabaseDestination
        {
            get
            {
                switch (SelectedDestination)
                {
                    case "MongoDB":
                        return true;
                    default:
                        return false;
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

        #endregion PROPERTIES


        #region METHODS

        private void ChooseFile()
        {
            var service = GetService(SelectedEDestination);
#pragma warning disable 4014 // no await
            switch (StateManager.GetState<EDataType>(EStateManagerKey.DataType.ToString()))
            {
                case EDataType.User:
                    SaveFile<User>(service);
                    break;
                case EDataType.Resident:
                    SaveFile<Resident>(service);
                    break;
                case EDataType.ReceiverModule:
                    SaveFile<ReceiverModule>(service);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#pragma warning restore 4014
        }

        private ISerializationService GetService(EDestination source)
        {
            switch (source)
            {
                case EDestination.Json:
                    return _jsonService;
                case EDestination.Csv:
                    return _csvService;
                case EDestination.Xml:
                    return _xmlService;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        private async Task SaveFile<T>(IObjectWriter service)
        {
            await service.WriteObjectToFileWithDialogAsync(
                StateManager.GetState<IEnumerable<T>>(EStateManagerKey.FileContent.ToString()));
        }

        #endregion METHODS
    }
}