using DatabaseImporter.Helpers;
using DatabaseImporter.Helpers.Events;
using DatabaseImporter.Services;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        public MainWindowViewModel(
            ISourceViewModel sourceViewModel,
            IDestinationViewModel destinationViewModel,
            IDataSelectionViewModel dataSelectionViewModels,
            IEventAggregator eventAggregator,
            IStateManager stateManager)
            : base(eventAggregator, stateManager)
        {
            SourceViewModel = sourceViewModel;
            DestinationViewModel = destinationViewModel;
            DataSelectionViewModels = dataSelectionViewModels;

            StateManager.StateChanged += OnStateChanged;
        }


        public ISourceViewModel SourceViewModel { get; }
        public IDestinationViewModel DestinationViewModel { get; }
        public IDataSelectionViewModel DataSelectionViewModels { get; }

        public object FileContent
            => StateManager.GetState<object>(EStateManagerKey.FileContent.ToString());


        protected void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            StateManager.StateChanged -= OnStateChanged;

            if (e.State == EStateManagerKey.FileContent.ToString())
                RaisePropertyChanged(nameof(FileContent));

            StateManager.StateChanged += OnStateChanged;
        }
    }
}