using System;
using ImageClassifierApp.Models.Training;

namespace ImageClassifierApp.Commands
{
    public class StopTrainingCommand : ViewModelChangedCommand<TrainingModel>
    {
        public override Func<TrainingModel, bool> CanExecuteFunc => model => model.IsTraining && model.IsEpochStopping == false && model.IsImmediateStopping == false;

        public override async void Execute(object parameter)
        {
            await ViewModel.StopTraining();
        }

        public StopTrainingCommand(TrainingModel paViewModel) : base(paViewModel)
        {
        }
    }
}
