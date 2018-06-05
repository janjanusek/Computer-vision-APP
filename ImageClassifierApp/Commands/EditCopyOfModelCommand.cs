using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.Properties;
using ImageClassifierApp.Views;

namespace ImageClassifierApp.Commands
{
    public class EditCopyOfModelCommand : ICommand, INotifyPropertyChanged
    {
        public NetModelBase NetModelBase { get; set; }

        public bool CanExecute(object parameter)
        {
            return (NetModelBase = parameter as NetModelBase) != null;
        }

        public void Execute(object parameter)
        {
            var window = new CreateNetModelWindow()
            {
                DataContext = new NetModelCreator(NetModelBase)
            };
            window.Show();
        }

        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
