using System.Windows.Input;

namespace Test2.Helpers
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _execute;

        public DelegateCommand(Action execute)
        {
            _execute = execute;
        }

        public void Execute(object parameter) => _execute();
        public bool CanExecute(object parameter) => true;
        public event EventHandler CanExecuteChanged;
    }
}