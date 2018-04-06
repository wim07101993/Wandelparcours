using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseImporter.Helpers;
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
    public class DestinationViewModel : BindableBase, IDestinationViewModel
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

            TransferCommand = new DelegateCommand(ChooseFile);
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

        #endregion PROPERTIES


        #region METHODS

        private void ChooseFile()
        {
#pragma warning disable 4014 // no await
            switch (StateManager.GetState<EDataType>(EState.DataType))
            {
                case EDataType.User:
                    SaveData<User>();
                    break;
                case EDataType.Resident:
                    SaveData<Resident>();
                    break;
                case EDataType.ReceiverModule:
                    SaveData<ReceiverModule>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#pragma warning restore 4014
        }

        private async Task SaveData<T>() where T : IModelWithObjectID
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

        #endregion METHODS
    }
}