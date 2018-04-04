namespace DatabaseImporter.Helpers.Events
{
    public class ValueChangedEventArgs<T> : ValueChangedEventArgs
    {
        public new T OldValue => (T) base.OldValue;
        public new T NewValue => (T)base.NewValue;

        public ValueChangedEventArgs(T oldValue, T newValue)
            : base(oldValue, newValue)
        {
        }
    }
}