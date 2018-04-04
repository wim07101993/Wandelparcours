using DatabaseImporter.Services;
using Prism.Events;

namespace DatabaseImporter.Helpers
{
    public abstract class BindableBase : Prism.Mvvm.BindableBase
    {
        protected readonly IEventAggregator EventAggregator;
        protected readonly IStateManager StateManager;
        

        protected BindableBase(IEventAggregator eventAggregator, IStateManager stateManager)
        {
            EventAggregator = eventAggregator;
            StateManager = stateManager;
        }
    }
}