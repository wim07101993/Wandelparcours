using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseImporter.Helpers;
using DatabaseImporter.Helpers.Events;
using DatabaseImporter.Models.MongoModels;
using DatabaseImporter.Services;
using DatabaseImporter.Services.DataIO;
using DatabaseImporter.Services.Serialization;
using DatabaseImporter.ViewModelInterfaces;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public class SourceViewModel : BindableBase, ISourceViewModel
    {
        #region FIELDS
        
        private string _selectedSource = ESource.Json.ToString();

        private string _filePath;
        private string _connectionString;

        #endregion FIELDS


        #region CONSTRUCTOR

        public SourceViewModel(IEventAggregator eventAggregator, IStateManager stateManager)
            : base(eventAggregator, stateManager)
        {
            ChooseFileCommand = new DelegateCommand(ChooseFile);

            StateManager.StateChanged += OnStateChanged;
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        private ISerializationService SerializationService
        {
            get
            {
                switch (SelectedESource)
                {
                    case ESource.Json:
                        return App.Bootstrapper.Container.Resolve<IJsonService>();
                    case ESource.Csv:
                        return App.Bootstrapper.Container.Resolve<ICsvService>();
                    case ESource.Xml:
                        return App.Bootstrapper.Container.Resolve<IXmlService>();
                    case ESource.MongoDB:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

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

        public ICommand ChooseFileCommand { get; }
        
        #endregion PROPERTIES


        #region METHODS

        private void ChooseFile()
        { 
#pragma warning disable 4014 // no await
            switch (StateManager.GetState<EDataType>(EStateManagerKey.DataType.ToString()))
            {
                case EDataType.User:
                    OpenFileAsync<User>(SerializationService);
                    break;
                case EDataType.Resident:
                    OpenFileAsync<Resident>(SerializationService);
                    break;
                case EDataType.ReceiverModule:
                    OpenFileAsync<ReceiverModule>(SerializationService);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#pragma warning restore 4014
        }

        private async Task OpenFileAsync<T>(IObjectReader service)
        {
            var file = await service.ReadObjectFromFileWithDialogAsync<T>();
            FilePath = file.Path;
            StateManager.SetState<IEnumerable>(EStateManagerKey.FileContent.ToString(), file.Content);
        }

        private async Task ReloadFileAsync()
        {
           switch (StateManager.GetState<EDataType>(EStateManagerKey.DataType.ToString()))
            {
                case EDataType.User:
                    var users = await SerializationService.ReadObjectFromFileAsync<User>(FilePath);
                    StateManager.SetState<IEnumerable>(EStateManagerKey.FileContent.ToString(), users.Content);
                    break;
                case EDataType.Resident:
                    var residents = await SerializationService.ReadObjectFromFileAsync<Resident>(FilePath);
                    StateManager.SetState<IEnumerable>(EStateManagerKey.FileContent.ToString(), residents.Content);
                    break;
                case EDataType.ReceiverModule:
                    var receiverModules = await SerializationService.ReadObjectFromFileAsync<ReceiverModule>(FilePath);
                    StateManager.SetState<IEnumerable>(EStateManagerKey.FileContent.ToString(), receiverModules.Content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            StateManager.StateChanged -= OnStateChanged;

            if (!string.IsNullOrEmpty(FilePath))
                await ReloadFileAsync();

            StateManager.StateChanged += OnStateChanged;
        }

        #endregion METHODS
    }
}