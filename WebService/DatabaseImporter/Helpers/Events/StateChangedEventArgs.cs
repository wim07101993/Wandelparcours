namespace DatabaseImporter.Helpers.Events
{
    public class StateChangedEventArgs : ValueChangedEventArgs
    {
        public StateChangedEventArgs(string state, object oldValue, object newValue) : base(oldValue, newValue)
        {
            State = state;
        }

        public string State { get; }
    }
}