using System;
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

        public Exception Exception
            => StateManager.GetState<Exception>(EState.Exception);


        protected override void OnStateChanged(object sender, StateChangedEventArgs e, EState state)
        {
            switch (state)
            {
                case EState.DataType:
                    break;
                case EState.FileContent:
                    RaisePropertyChanged(nameof(FileContent));
                    break;
                case EState.Exception:
                    RaisePropertyChanged(nameof(Exception));
                    break;
                case EState.UnKnown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}