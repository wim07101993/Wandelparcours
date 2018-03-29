using System;
using System.Windows.Input;

namespace ModuleSettingsEditor.Helpers.Mvvm.Commands
{
    public abstract class ADelegateCommandBase : ICommand
    {
        bool ICommand.CanExecute(object parameter)
            => CanExecute(parameter);

        void ICommand.Execute(object parameter)
            => Execute(parameter);

        protected abstract bool CanExecute(object parameter);
        protected abstract void Execute(object parameter);

        protected virtual void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public event EventHandler CanExecuteChanged;
    }
}