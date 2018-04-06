using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

            StateManager.StateChanged += OnStateChangedBase;
        }


        private void OnStateChangedBase(object sender, StateChangedEventArgs e)
        {
            if (!Enum.TryParse(e.State, out EState state))
                state = EState.UnKnown;

            OnStateChanged(sender, e, state);
        }

        protected abstract void OnStateChanged(object sender, StateChangedEventArgs e, EState state);

        protected bool SetPropertyState<T>(EState state, T value, [CallerMemberName] string propertyName = null)
        {
            if (!SetState(state, value))
                return false;

            RaisePropertyChanged(propertyName);
            return true;
        }

        protected bool SetState<T>(EState state, T value)
        {
            StateManager.StateChanged -= OnStateChangedBase;

            var isValueSet = StateManager.SetState(state, value);

            StateManager.StateChanged += OnStateChangedBase;

            return isValueSet;
        }

        protected async Task CatchExceptionAsync(Task method)
        {
            try
            {
                await method;
            }
            catch (Exception e)
            {
                SetState(EState.Exception, e);
            }
        }

        protected async Task<T> CatchExceptionAsync<T>(Task<T> method)
        {
            try
            {
                return await method;
            }
            catch (Exception e)
            {
                SetState(EState.Exception, e);
            }

            return default(T);
        }
    }
}