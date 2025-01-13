using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LibraryManagementSystem.Commands
{
    public class RelayCommand : ICommand
    {
        //It's used to notify the UI that the CanExecute status has changed
        public event EventHandler CanExecuteChanged;
        private readonly Action _doWork; 
        private readonly Action<object> _doWorkWithParam; 

        public RelayCommand(Action doWork) //Accepts an action without parameters.
        {
            _doWork = doWork;
        }

        public RelayCommand(Action<object> doWorkWithParam) //Accepts an action with a parameter.
        {
            _doWorkWithParam = doWorkWithParam;
        }

        public bool CanExecute(object parameter) //Checks if the command can be executed based on the canExecute
        {
            return true;
        }

        public void Execute(object parameter) //Runs the logic defined in the _doWork
        {
            if (_doWork != null)
                _doWork();
            else if (_doWorkWithParam != null)
                _doWorkWithParam(parameter);
        }

    }
}
