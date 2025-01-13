using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LibraryManagementSystem.Commands
{
    public class ActionCommand : ICommand
    {
        private readonly Action<object> _execute;
        //Action delegate represents a method that takes parameters but does not return a value
        //That will hold the logic to execute when the command is triggered. (_execute)

        private readonly Predicate<object> _canExecute;
        //The Predicate delegate represents a method that takes a parameter and returns a Boolean (true or false), typically used to test or validate conditions.
        //That will hold the logic for determining whether the command can be executed (whether a add category button should be enabled).

        public event EventHandler CanExecuteChanged;
        //It's used to notify the UI that the CanExecute status has changed

        public ActionCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) //Checks if the command can be executed based on the canExecute
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter) //Runs the logic defined in the _execute
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty); //This is typically used when the conditions for whether the command can be executed change.
        }
    }
}
