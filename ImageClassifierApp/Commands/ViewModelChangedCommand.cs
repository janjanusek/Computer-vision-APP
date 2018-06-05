using System;
using System.ComponentModel;
using System.Windows.Input;
using PropertyChanged;

namespace ImageClassifierApp.Commands
{
    [AddINotifyPropertyChangedInterface]
    public abstract class ViewModelChangedCommand<TViewModel> : ICommand where TViewModel : INotifyPropertyChanged
    {
        public virtual Func<TViewModel, bool> CanExecuteFunc => null;

        private TViewModel _viewModel;

        public TViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                if(value == null)
                    return;
                if (_viewModel != null)
                    _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
                _viewModel = value;
                if (_viewModel != null)
                    _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            }
        }

        protected ViewModelChangedCommand(TViewModel paViewModel)
        {
            ViewModel = paViewModel;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            ViewModel = (TViewModel)parameter;

            return ViewModel != null && (CanExecuteFunc == null || CanExecuteFunc.Invoke(ViewModel));
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
