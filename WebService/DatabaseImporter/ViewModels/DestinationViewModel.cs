using System;
using System.Collections.Generic;
using System.Windows.Input;
using DatabaseImporter.Helpers;
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

            StateManager.StateChanged += OnStateChanged;
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


        public bool UserNeedsToInputConnectionString { get; }


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
        }

        private async void OnStateChanged(object sender, Helpers.Events.StateChangedEventArgs e)
        {
            StateManager.StateChanged -= OnStateChanged;

            StateManager.StateChanged += OnStateChanged;
        }

        #endregion METHODS
    }
}