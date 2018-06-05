using System;
using ImageClassifierApp.ViewModels.Testing;

namespace ImageClassifierApp.Commands
{
    public class ResetTesterCommand : ViewModelChangedCommand<TestingViewModel>
    {
        public override Func<TestingViewModel, bool> CanExecuteFunc => model => model.IsWorking == false;

        public ResetTesterCommand(TestingViewModel paViewModel) : base(paViewModel)
        {
        }

        public override void Execute(object parameter)
        {
            ViewModel.Reset();
        }
    }
}