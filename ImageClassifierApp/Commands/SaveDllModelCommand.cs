using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Services.Notifications;

namespace ImageClassifierApp.Commands
{
    public class SaveDllModelCommand : ICommand
    {
        private bool _isWorking;

        public bool CanExecute(object parameter)
        {
            return GetModel(parameter).ConfigurationModel != null && _isWorking == false;
        }

        public async void Execute(object parameter)
        {
            _isWorking = true;
            OnCanExecuteChanged();
            var task = Task.Run(() =>
            {
                try
                {
                    var model = GetModel(parameter).ConfigurationModel;
                    var json = model.ToJson();
                    FilePickerHelper.SaveFile(json, "json");
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
                    _isWorking = false;
                }
            });
            await task;
            task.Dispose();
            OnCanExecuteChanged();
        }

        private NetModelBase GetModel(object paObj)
        {
            return (paObj as NetModelBase) ?? (paObj as TrainingModel)?.Model;
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
