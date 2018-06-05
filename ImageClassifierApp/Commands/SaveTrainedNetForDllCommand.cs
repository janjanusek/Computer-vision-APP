using System;
using System.Threading.Tasks;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Services.Notifications;

namespace ImageClassifierApp.Commands
{
    public class SaveTrainedNetForDllCommand : ViewModelChangedCommand<TrainingModel>
    {
        private bool _isSaving;
        public override Func<TrainingModel, bool> CanExecuteFunc => model => this._isSaving == false;

        public SaveTrainedNetForDllCommand(TrainingModel paViewModel) : base(paViewModel)
        {
            _isSaving = false;
        }

        public override async void Execute(object parameter)
        {
            _isSaving = true;
            OnCanExecuteChanged();
            var task = Task.Run(() =>
                {
                    try
                    {
                        var model = ViewModel.GetTrainedModel();
                        FilePickerHelper.SaveFile(model.ToJson(), "mementoModelJson");
                    }
                    catch (Exception e)
                    {
                        NotifyUser.NotifyUserByNotification(
                            new Notification()
                            {
                                Title = "SDK model to json error",
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
    }
}
