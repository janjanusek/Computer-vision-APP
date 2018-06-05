using System;
using ImageClassifierApp.Models.Training;

namespace ImageClassifierApp.Commands
{
    public class StartTrainingCommand : ViewModelChangedCommand<TrainingModel>
    {
        public override Func<TrainingModel, bool> CanExecuteFunc => model => model.IsTraining == false;

        public override async void Execute(object parameter)
        {
            if (ViewModel != null)
                await ViewModel.TrainNet();
        }

        public StartTrainingCommand(TrainingModel paViewModel) : base(paViewModel)
        {
        }
    }
}
