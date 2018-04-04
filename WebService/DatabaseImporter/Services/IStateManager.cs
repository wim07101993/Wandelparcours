using System;
using System.Collections.Generic;
using DatabaseImporter.Helpers.Events;

namespace DatabaseImporter.Services
{
    public interface IStateManager
    {
        ICollection<string> Keys { get; }

        T GetState<T>(string key);
        void SetState<T>(string key, T value);

        event EventHandler<StateChangedEventArgs> StateChanged;
    }
}