using System;
using ImageClassifierApp.Models.Training;

namespace ImageClassifierApp.Commands
{
    public class ImmediateStopTrainingCommand : ViewModelChangedCommand<TrainingModel>
    {
        public override Func<TrainingModel, bool> CanExecuteFunc => model => model.IsTraining && model.IsImmediateStopping == false;

        public ImmediateStopTrainingCommand(TrainingModel paViewModel) : base(paViewModel)
        {
        }

        public override async void Execute(object parameter)
        {
            await ViewModel.ImmediateStop();
        }
    }
}
