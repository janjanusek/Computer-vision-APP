using System;
using System.Windows.Input;
using ImageClassifierApp.Views;

namespace ImageClassifierApp.Commands
{
    public class OpenNetCreatorWindowCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var window = new CreateNetModelWindow();
            window.Show();
        }

        public event EventHandler CanExecuteChanged;
    }
}
