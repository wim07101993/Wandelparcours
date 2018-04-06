using System.Runtime.CompilerServices;
using DatabaseImporter.Helpers;
using DatabaseImporter.Helpers.Events;
using DatabaseImporter.Helpers.Extensions;
using DatabaseImporter.Services;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public abstract class AViewModelBase : Prism.Mvvm.BindableBase
    {
        protected readonly IEventAggregator EventAggregator;
        protected readonly IStateManager StateManager;


        protected AViewModelBase(IEventAggregator eventAggregator, IStateManager stateManager)
        {
            EventAggregator = eventAggregator;
            StateManager = stateManager;

            StateManager.StateChanged += OnStateChanged;
        }


        protected abstract void OnStateChanged(object sender, StateChangedEventArgs e);

        protected bool SetState<T>(EState state, T value, [CallerMemberName] string propertyName = null)
        {
            StateManager.StateChanged -= OnStateChanged;

            var isValueSet = StateManager.SetState(state, value);

            StateManager.StateChanged += OnStateChanged;

            if (!isValueSet)
                return false;

            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}