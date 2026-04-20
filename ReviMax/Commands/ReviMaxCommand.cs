using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReviMax.GostSymbolManager.UI.Commands
{
    internal class ReviMaxCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool>? _canExecute;
        public event EventHandler? CanExecuteChanged;

        public ReviMaxCommand(Action action, Func<bool>? canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke()??true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
        public void RaiseCanExecuteChanged() 
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
