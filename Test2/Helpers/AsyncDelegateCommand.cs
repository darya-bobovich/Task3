using System.Windows.Input;

namespace Test2.Helpers
{
    public class AsyncDelegateCommand : ICommand
    {
        private readonly Func<object, Task> _execute;
        private bool _isExecuting;

        public AsyncDelegateCommand(Func<object, Task> execute)
        {
            _execute = execute;
        }

        public async void Execute(object parameter)
        {
            if (_isExecuting) return;

            _isExecuting = true;
            try
            {
                await _execute(parameter);
            }
            finally
            {
                _isExecuting = false;
            }
        }

        public bool CanExecute(object parameter) => !_isExecuting;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}