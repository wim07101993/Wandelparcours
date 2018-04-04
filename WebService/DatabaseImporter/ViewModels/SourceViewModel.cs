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
    public class SourceViewModel : BindableBase, ISourceViewModel
    {
        #region FIELDS

        private readonly ICsvService _csvService;
        private readonly IJsonService _jsonService;
        private readonly IXmlService _xmlService;

        private string _selectedSource = ESource.Json.ToString();

        private string _filePath;
        private string _connectionString;

        #endregion FIELDS


        #region CONSTRUCTOR

        public SourceViewModel(ICsvService csvService, IJsonService jsonService, IXmlService xmlService,
            IEventAggregator eventAggregator, IStateManager stateManager)
            : base(eventAggregator, stateManager)
        {
            _csvService = csvService;
            _jsonService = jsonService;
            _xmlService = xmlService;

            ChooseFileCommand = new DelegateCommand(ChooseFile);

            StateManager.StateChanged += OnStateChanged;
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        public IEnumerable<string> Sources { get; } = Enum.GetNames(typeof(ESource));

        public string SelectedSource
        {
            get => _selectedSource;
            set
            {
                if (!SetProperty(ref _selectedSource, value))
                    return;

                RaisePropertyChanged(nameof(SelectedESource));
                RaisePropertyChanged(nameof(IsFileSource));
                RaisePropertyChanged(nameof(IsDatabaseSource));
            }
        }

        private ESource SelectedESource
            => Enum.TryParse(SelectedSource, out ESource ret)
                ? ret
                : ESource.Json;

        public bool IsFileSource
        {
            get
            {
                switch (SelectedSource)
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

        public bool IsDatabaseSource
        {
            get
            {
                switch (SelectedSource)
                {
                    case "MongoDB":
                        return true;
                    default:
                        return false;
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
            get => StateManager.GetState<object>(EStateManagerKey.FileContent.ToString());
            set
            {
                StateManager.SetState(EStateManagerKey.FileContent.ToString(), value);
                RaisePropertyChanged(nameof(Value));
            }
        }

        #endregion PROPERTIES


        #region METHODS

        private void ChooseFile()
        {
            var service = GetService(SelectedESource);
#pragma warning disable 4014 // no await
            switch (StateManager.GetState<EDataType>(EStateManagerKey.DataType.ToString()))
            {
                case EDataType.User:
                    OpenFile<User>(service);
                    break;
                case EDataType.Resident:
                    OpenFile<Resident>(service);
                    break;
                case EDataType.ReceiverModule:
                    OpenFile<ReceiverModule>(service);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#pragma warning restore 4014
        }

        private ISerializationService GetService(ESource source)
        {
            switch (source)
            {
                case ESource.Json:
                    return _jsonService;
                case ESource.Csv:
                    return _csvService;
                case ESource.Xml:
                    return _xmlService;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        private async Task OpenFile<T>(IObjectReader service)
        {
            var file = await service.ReadObjectFromFileWithDialogAsync<T>();
            FilePath = file.Path;
            Value = file.Content;
        }

        private async Task ReloadFile()
        {
            var service = GetService(SelectedESource);
            switch (StateManager.GetState<EDataType>(EStateManagerKey.DataType.ToString()))
            {
                case EDataType.User:
                    var users = await service.ReadObjectFromFileAsync<User>(FilePath);
                    Value = users.Content;
                    break;
                case EDataType.Resident:
                    var residents = await service.ReadObjectFromFileAsync<Resident>(FilePath);
                    Value = residents.Content;
                    break;
                case EDataType.ReceiverModule:
                    var receiverModules = await service.ReadObjectFromFileAsync<ReceiverModule>(FilePath);
                    Value = receiverModules.Content;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected async void OnStateChanged(object sender, Helpers.Events.StateChangedEventArgs e)
        {
            StateManager.StateChanged -= OnStateChanged;

            if (!string.IsNullOrEmpty(FilePath))
                await ReloadFile();

            StateManager.StateChanged += OnStateChanged;
        }

        #endregion METHODS
    }
}