using System;
using System.Threading.Tasks;
using AiSdk.Configuration.Configurator;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Services.Notifications;
using ImageClassifierApp.ViewModels;

namespace ImageClassifierApp.Commands
{
    public class SaveTrainedModelCommand : ViewModelChangedCommand<TrainingModel>
    {
        private bool _isSaving;
        public override Func<TrainingModel, bool> CanExecuteFunc => model => model.IsTraining == false && _isSaving == false;

        public override async void Execute(object parameter)
        {
            _isSaving = true;
            var task = Task.Run(() =>
            {
                try
                {
                    var model = ViewModel.GetTrainedModel();
                    if (model == null)
                        return;
                    var trainedModel = new TrainedNetModel((MementoConfigurationModel) model.Clone())
                    {
                        ModelName = ViewModel.Model.ModelName,
                        DataSetMetaData = ViewModel.DataSetModel.MetaData
                    };
                    FilePickerHelper.SaveFile(trainedModel.ToJson(), NetBuilderViewModel.DirtyNetModel);
                }
                catch (Exception e)
                {
                    NotifyUser.NotifyUserByNotification(
                        new Notification()
                        {
                            Title = "Ooooops!",
                            Message =
                                $"Something happend during generation of your code.",
                            Exception = e
                        });
                }
                finally
                {
                    _isSaving = false;
                    App.RunInUiThread(OnCanExecuteChanged);
                }
            });
            await task;
            task.Dispose();
        }

        public SaveTrainedModelCommand(TrainingModel paViewModel) : base(paViewModel)
        {
        }
    }
}
