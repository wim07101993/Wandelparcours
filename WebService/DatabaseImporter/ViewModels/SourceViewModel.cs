using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseImporter.Helpers;
using DatabaseImporter.Helpers.Events;
using DatabaseImporter.Models.MongoModels;
using DatabaseImporter.Models.MongoModels.Bases;
using DatabaseImporter.Services;
using DatabaseImporter.Services.Data;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Commands;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public class SourceViewModel : BindableBase, ISourceViewModel
    {
        #region FIELDS

        private readonly IDataServiceSelector _dataServiceSelector;
        private readonly IDialogService _dialogService;

        private string _selectedSource = ESource.Json.ToString();

        private string _filePath;
        private string _connectionString;
        private string _databaseName;
        private string _tableName;

        #endregion FIELDS


        #region CONSTRUCTOR

        public SourceViewModel(IEventAggregator eventAggregator, IStateManager stateManager,
            IDataServiceSelector dataServiceSelector, IDialogService dialogService)
            : base(eventAggregator, stateManager)
        {
            _dataServiceSelector = dataServiceSelector;
            _dialogService = dialogService;

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
                switch (SelectedESource)
                {
                    case ESource.Json:
                    case ESource.Csv:
                    case ESource.Xml:
                        return true;
                    case ESource.MongoDB:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool IsDatabaseSource
        {
            get
            {
                switch (SelectedESource)
                {
                    case ESource.Json:
                    case ESource.Csv:
                    case ESource.Xml:
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

        public string DatabaseName
        {
            get => _databaseName;
            set => SetProperty(ref _databaseName, value);
        }

        public string TableName
        {
            get => _tableName;
            set => SetProperty(ref _tableName, value);
        }

        public ICommand ChooseFileCommand { get; }

        #endregion PROPERTIES


        #region METHODS

        private void ChooseFile()
        {
#pragma warning disable 4014 // no await
            switch (StateManager.GetState<EDataType>(EState.DataType.ToString()))
            {
                case EDataType.User:
                    OpenAsync<User>();
                    break;
                case EDataType.Resident:
                    OpenAsync<Resident>();
                    break;
                case EDataType.ReceiverModule:
                    OpenAsync<ReceiverModule>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#pragma warning restore 4014
        }

        private async Task OpenAsync<T>() where T : IModelWithObjectID
        {
            var service = _dataServiceSelector.GetService(SelectedESource);
            IEnumerable items;

            if (IsDatabaseSource)
                items = await service.GetAsync<T>(null, ConnectionString, DatabaseName, TableName);
            else
            {
                var extensions = ((IFileDataService) service).ExtensionFilter;
                FilePath = _dialogService.OpenFileDialog(extensions);
                items = await service.GetAsync<T>(null, FilePath);
            }

            StateManager.SetState(EState.FileContent.ToString(), items);
        }

        private async Task ReloadFileAsync<T>() where T : IModelWithObjectID
        {
            var service = _dataServiceSelector.GetService(SelectedESource);
            IEnumerable items;

            if (IsDatabaseSource)
                items = await service.GetAsync<T>(null, ConnectionString, DatabaseName, TableName);
            else
                items = await service.GetAsync<T>(null, FilePath);

            StateManager.SetState(EState.FileContent.ToString(), items);
        }

        private async void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            StateManager.StateChanged -= OnStateChanged;

            if (e.State == EState.DataType.ToString() && !string.IsNullOrEmpty(FilePath))
            {
                switch (StateManager.GetState<EDataType>(EState.DataType.ToString()))
                {
                    case EDataType.User:
                        await ReloadFileAsync<User>();
                        break;
                    case EDataType.Resident:
                        await ReloadFileAsync<Resident>();
                        break;
                    case EDataType.ReceiverModule:
                        await ReloadFileAsync<ReceiverModule>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            StateManager.StateChanged += OnStateChanged;
        }

        #endregion METHODS
    }
}