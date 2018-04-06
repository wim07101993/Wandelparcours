using DatabaseImporter.Helpers;
using DatabaseImporter.Helpers.Events;
using DatabaseImporter.Helpers.Extensions;
using DatabaseImporter.Services;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public class MainWindowViewModel : AViewModelBase, IMainWindowViewModel
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
        }


        public ISourceViewModel SourceViewModel { get; }
        public IDestinationViewModel DestinationViewModel { get; }
        public IDataSelectionViewModel DataSelectionViewModels { get; }

        public object FileContent
            => StateManager.GetState<object>(EState.FileContent);


        protected override void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.State == EState.FileContent.ToString())
                RaisePropertyChanged(nameof(FileContent));
        }
    }
}