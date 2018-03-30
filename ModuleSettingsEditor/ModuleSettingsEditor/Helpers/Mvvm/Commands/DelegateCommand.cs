using System;

namespace ModuleSettingsEditor.Helpers.Mvvm.Commands
{
    public class DelegateCommand : ADelegateCommandBase
    {
        private readonly Action _executeMethod;
        private readonly Func<bool> _canExecuteMethod;


        public DelegateCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }


        protected override bool CanExecute(object parameter)
            => CanExecute();

        protected override void Execute(object parameter)
            => Execute();

        public bool CanExecute()
            => _canExecuteMethod();

        public void Execute()
            => _executeMethod();
    }
}