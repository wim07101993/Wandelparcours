using System;

namespace ModuleSettingsEditor.Helpers.Mvvm.Commands
{
    public class DelegateCommand<T> : ADelegateCommandBase
    {
        private readonly Action<T> _executeMethod;
        private readonly Func<T, bool> _canExecuteMethod;


        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, T => true)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }


        protected override bool CanExecute(object parameter)
            => CanExecute((T) parameter);

        protected override void Execute(object parameter)
            => Execute((T) parameter);

        public bool CanExecute(T parameter)
            => _canExecuteMethod(parameter);

        public void Execute(T paramter)
            => _executeMethod(paramter);
    }
}