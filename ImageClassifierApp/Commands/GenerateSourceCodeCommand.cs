using System;
using AiSdk.CodeGeneration;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Services.Notifications;

namespace ImageClassifierApp.Commands
{
    public class GenerateSourceCodeCommand : ViewModelChangedCommand<TrainingModel>
    {
        public override Func<TrainingModel, bool> CanExecuteFunc => model => model.IsTraining == false;

        public override void Execute(object parameter)
        {
            try
            {
                var model = ViewModel.GetTrainedModel();
                if (model == null)
                    return;
                var generator = new CplusPlusCodeGenerator(model);
                generator.GenerateCode();
                FilePickerHelper.SaveFile(generator.Result, "h");
            }
            catch (Exception e)
            {
                NotifyUser.NotifyUserByNotification(
                    new Notification()
                    {
                        Title = "Ooooops!",
                        Message = $"Something happend during generation of your code.",
                        Exception = e
                    });
            }
        }

        public GenerateSourceCodeCommand(TrainingModel paViewModel) : base(paViewModel)
        {
        }
    }
}
