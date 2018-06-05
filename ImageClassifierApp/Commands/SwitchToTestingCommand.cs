using System;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Services.SharedResources;
using ImageClassifierApp.ViewModels.Testing;

namespace ImageClassifierApp.Commands
{
    public class SwitchToTestingCommand : ViewModelChangedCommand<TrainingModel>
    {
        public override Func<TrainingModel, bool> CanExecuteFunc =>
            model => model.IsDisposed == false && model.IsTraining == false;

        public SwitchToTestingCommand(TrainingModel paViewModel) : base(paViewModel)
        {
        }

        public override async void Execute(object parameter)
        {
            await SharedDataService.OpenModuleAndSetData<TestingViewModel>(ViewModel);
        }
    }
}
