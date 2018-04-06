using System;
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
    public class DestinationViewModel : AViewModelBase, IDestinationViewModel
    {
        #region FIELDS

        private readonly IDataServiceSelector _dataServiceSelector;
        private readonly IDialogService _dialogService;

        private string _selectedDestination = EDestination.Json.ToString();
        private string _filePath;
        private string _ipAddress;
        private string _databaseName;
        private string _tableName;

        #endregion FIELDS


        #region CONSTRUCTOR

        public DestinationViewModel(IEventAggregator eventAggregator, IStateManager stateManager,
            IDataServiceSelector dataServiceSelector, IDialogService dialogService)
            : base(eventAggregator, stateManager)
        {
            _dataServiceSelector = dataServiceSelector;
            _dialogService = dialogService;

            TransferCommand = new DelegateCommand(SaveData);
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
                RaisePropertyChanged(nameof(DatabaseService));
            }
        }

        private EDestination SelectedEDestination
            => Enum.TryParse(SelectedDestination, out EDestination ret)
                ? ret
                : EDestination.Json;

        public bool IsFileDestination
            => SelectedEDestination.IsFileDestination();

        public bool IsDatabaseDestination
            => SelectedEDestination.IsDatabaseDestination();


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

        public ICommand TransferCommand { get; }

        public IDatabaseService DatabaseService 
            => _dataServiceSelector.GetService(SelectedEDestination) as IDatabaseService;

        #endregion PROPERTIES


        #region METHODS

        private void SaveData()
        {
            Task method;

            switch (StateManager.GetState<EDataType>(EState.DataType))
            {
                case EDataType.User:
                    method = SaveDataAsync<User>();
                    break;
                case EDataType.Resident:
                    method = SaveDataAsync<Resident>();
                    break;
                case EDataType.ReceiverModule:
                    method = SaveDataAsync<ReceiverModule>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

#pragma warning disable 4014 // no await
            CatchExceptionAsync(method);
#pragma warning restore 4014
        }

        private async Task SaveDataAsync<T>() where T : IModelWithObjectID
        {
            var service = _dataServiceSelector.GetService(SelectedEDestination);
            var items = StateManager.GetState<IEnumerable<T>>(EState.FileContent);

            if (IsDatabaseDestination)
                await service.AddAsync(items, IpAddress, DatabaseName, TableName);
            else
            {
                var extensions = ((IFileDataService) service).ExtensionFilter;
                FilePath = _dialogService.WriteFileDialog(extensions);
                await service.AddAsync(items, FilePath);
            }
        }

        protected override void OnStateChanged(object sender, StateChangedEventArgs e, EState state)
        {
        }

        #endregion METHODS
    }
}