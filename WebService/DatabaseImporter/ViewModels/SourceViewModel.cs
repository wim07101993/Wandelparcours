using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseImporter.Helpers;
using DatabaseImporter.Helpers.Events;
using DatabaseImporter.Helpers.Extensions;
using DatabaseImporter.Models.MongoModels;
using DatabaseImporter.Models.MongoModels.Bases;
using DatabaseImporter.Services;
using DatabaseImporter.Services.Data;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Commands;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public class SourceViewModel : AViewModelBase, ISourceViewModel
    {
        #region FIELDS

        private readonly IDataServiceSelector _dataServiceSelector;
        private readonly IDialogService _dialogService;

        private string _selectedSource = ESource.Json.ToString();

        private string _filePath;
        private string _ipAddress;
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

            ImportCommand = new DelegateCommand(Import);
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
                RaisePropertyChanged(nameof(DatabaseService));
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

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
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

        public ICommand ImportCommand { get; }

        public IDatabaseService DatabaseService => _dataServiceSelector.GetService(SelectedESource) as IDatabaseService;

        #endregion PROPERTIES


        #region METHODS

        private void Import()
        {
            Task method;

            switch (StateManager.GetState<EDataType>(EState.DataType))
            {
                case EDataType.User:
                    method = ImportAsync<User>();
                    break;
                case EDataType.Resident:
                    method = ImportAsync<Resident>();
                    break;
                case EDataType.ReceiverModule:
                    method = ImportAsync<ReceiverModule>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

#pragma warning disable 4014 // no await
            CatchExceptionAsync(method);
#pragma warning restore 4014
        }

        private async Task ImportAsync<T>() where T : IModelWithObjectID
        {
            var service = _dataServiceSelector.GetService(SelectedESource);
            IEnumerable items;

            if (IsDatabaseSource)
                items = await service.GetAsync<T>(null, IpAddress, DatabaseName, TableName);
            else
            {
                var extensions = ((IFileDataService) service).ExtensionFilter;
                FilePath = _dialogService.OpenFileDialog(extensions);
                items = await service.GetAsync<T>(null, FilePath);
            }

            StateManager.SetState(EState.FileContent, items);
        }

        private async Task ReloadDataAsync<T>() where T : IModelWithObjectID
        {
            var service = _dataServiceSelector.GetService(SelectedESource);
            IEnumerable items;

            if (IsDatabaseSource)
                items = await service.GetAsync<T>(null, IpAddress, DatabaseName, TableName);
            else
                items = await service.GetAsync<T>(null, FilePath);

            StateManager.SetState(EState.FileContent, items);
        }

        protected override void OnStateChanged(object sender, StateChangedEventArgs e, EState state)
        {
            switch (state)
            {
                case EState.DataType:
                    if (string.IsNullOrEmpty(FilePath))
                        return;
                    Task method;

                    switch (StateManager.GetState<EDataType>(EState.DataType))
                    {
                        case EDataType.User:
                            method = ReloadDataAsync<User>();
                            break;
                        case EDataType.Resident:
                            method = ReloadDataAsync<Resident>();
                            break;
                        case EDataType.ReceiverModule:
                            method = ReloadDataAsync<ReceiverModule>();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

#pragma warning disable 4014 // no await
                    CatchExceptionAsync(method);
#pragma warning restore 4014
                    break;
                case EState.FileContent:
                case EState.Exception:
                case EState.UnKnown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        #endregion METHODS
    }
}