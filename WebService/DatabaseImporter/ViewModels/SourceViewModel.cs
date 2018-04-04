using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseImporter.Helpers;
using DatabaseImporter.Models.MongoModels;
using DatabaseImporter.Services;
using DatabaseImporter.Services.Serialization;
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
            var service = GetService(SelectedESource);
#pragma warning disable 4014 // no await
            switch (StateManager.GetState<EDataType>(EStateManagerKey.DataType.ToString()))
            {
                case EDataType.User:
                    OpenFileAsync<User>(service);
                    break;
                case EDataType.Resident:
                    OpenFileAsync<Resident>(service);
                    break;
                case EDataType.ReceiverModule:
                    OpenFileAsync<ReceiverModule>(service);
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

        private async Task OpenFileAsync<T>(IObjectReader service)
        {
            var file = await service.ReadObjectFromFileWithDialogAsync<T>();
            FilePath = file.Path;
            StateManager.SetState(EStateManagerKey.FileContent.ToString(), file.Content);
        }

        private async Task ReloadFileAsync()
        {
            var service = GetService(SelectedESource);
            switch (StateManager.GetState<EDataType>(EStateManagerKey.DataType.ToString()))
            {
                case EDataType.User:
                    var users = await service.ReadObjectFromFileAsync<User>(FilePath);
                    StateManager.SetState(EStateManagerKey.FileContent.ToString(), users.Content);
                    break;
                case EDataType.Resident:
                    var residents = await service.ReadObjectFromFileAsync<Resident>(FilePath);
                    StateManager.SetState(EStateManagerKey.FileContent.ToString(), residents.Content);
                    break;
                case EDataType.ReceiverModule:
                    var receiverModules = await service.ReadObjectFromFileAsync<ReceiverModule>(FilePath);
                    StateManager.SetState(EStateManagerKey.FileContent.ToString(), receiverModules.Content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void OnStateChanged(object sender, Helpers.Events.StateChangedEventArgs e)
        {
            StateManager.StateChanged -= OnStateChanged;

            if (!string.IsNullOrEmpty(FilePath))
                await ReloadFileAsync();

            StateManager.StateChanged += OnStateChanged;
        }

        #endregion METHODS
    }
}