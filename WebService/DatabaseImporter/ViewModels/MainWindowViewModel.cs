using System;
using System.Collections.Generic;
using DatabaseImporter.Helpers;
using DatabaseImporter.Helpers.Events;
using DatabaseImporter.Services;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        public MainWindowViewModel(ISourceViewModel sourceViewModel, IDestinationViewModel destinationViewModel,
            IEventAggregator eventAggregator, IStateManager stateManager)
            : base(eventAggregator, stateManager)
        {
            SourceViewModel = sourceViewModel;
            DestinationViewModel = destinationViewModel;

            SelectedDataType = EDataType.Resident.ToString();
            StateManager.StateChanged += OnStateChanged;
        }


        public IEnumerable<string> DataTypes { get; } = Enum.GetNames(typeof(EDataType));

        public ISourceViewModel SourceViewModel { get; }
        public IDestinationViewModel DestinationViewModel { get; }

        public string SelectedDataType
        {
            get => SelectedEDataType.ToString();
            set
            {
                if (Enum.TryParse(value, out EDataType dataType))
                    StateManager.SetState(EStateManagerKey.DataType.ToString(), dataType);
                else
                    throw new ArgumentOutOfRangeException();
            }
        }

        private EDataType SelectedEDataType
            => StateManager.GetState<EDataType>(EStateManagerKey.DataType.ToString());

        public object FileContent
            => StateManager.GetState<object>(EStateManagerKey.FileContent.ToString());


        protected void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            StateManager.StateChanged -= OnStateChanged;

            if (e.State == EStateManagerKey.DataType.ToString())
                RaisePropertyChanged(nameof(SelectedEDataType));
            else if (e.State == EStateManagerKey.FileContent.ToString())
                RaisePropertyChanged(nameof(FileContent));

            StateManager.StateChanged += OnStateChanged;
        }
    }
}