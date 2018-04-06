﻿using System;
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
        private IEnumerable<string> _databases;
        private IEnumerable<string> _tables;

        #endregion FIELDS


        #region CONSTRUCTOR

        public SourceViewModel(IEventAggregator eventAggregator, IStateManager stateManager,
            IDataServiceSelector dataServiceSelector, IDialogService dialogService)
            : base(eventAggregator, stateManager)
        {
            _dataServiceSelector = dataServiceSelector;
            _dialogService = dialogService;

            ImportCommand = new DelegateCommand(ImportSource);
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

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (!SetProperty(ref _ipAddress, value))
                    return;

                UpdateDatabasesList();
            }
        }

        public IEnumerable<string> Databases
        {
            get => _databases;
            private set => SetProperty(ref _databases, value);
        }

        public string DatabaseName
        {
            get => _databaseName;
            set
            {
                if (!SetProperty(ref _databaseName, value))
                    return;

                UpdateTables();
            }
        }

        public IEnumerable<string> Tables
        {
            get => _tables;
            private set => SetProperty(ref _tables, value);
        }

        public string TableName
        {
            get => _tableName;
            set => SetProperty(ref _tableName, value);
        }

        public ICommand ImportCommand { get; }

        #endregion PROPERTIES


        #region METHODS

        private void ImportSource()
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

        private async Task ReloadFileAsync<T>() where T : IModelWithObjectID
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
                            method = ReloadFileAsync<User>();
                            break;
                        case EDataType.Resident:
                            method = ReloadFileAsync<Resident>();
                            break;
                        case EDataType.ReceiverModule:
                            method = ReloadFileAsync<ReceiverModule>();
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

        private async void UpdateDatabasesList()
        {
            var service = _dataServiceSelector.GetService(SelectedESource);
            Databases = await CatchExceptionAsync(((IDatabaseService)service).GetDatabasesAsync(IpAddress));
        }

        private async void UpdateTables()
        {
            var service = _dataServiceSelector.GetService(SelectedESource);
            Tables = await CatchExceptionAsync(((IDatabaseService)service).GetTables(IpAddress, DatabaseName));
        }

        #endregion METHODS
    }
}