using System;
using System.Collections.Generic;
using DatabaseImporter.Helpers.Events;

namespace DatabaseImporter.Services
{
    public class StateManager : IStateManager
    {
        private readonly IDictionary<string, object> _states = new Dictionary<string, object>();


        public ICollection<string> Keys => _states.Keys;


        public T GetState<T>(string key)
        {
            if (!_states.ContainsKey(key))
                return default(T);

            return (T) _states[key];
        }

        public bool SetState<T>(string key, T value)
        {
            var oldValue = GetState<T>(key);

            if (Equals(oldValue, value))
                return false;

            _states[key] = value;
            StateChanged?.Invoke(this, new StateChangedEventArgs(key, oldValue, value));
            return true;
        }


        public event EventHandler<StateChangedEventArgs> StateChanged;
    }
}