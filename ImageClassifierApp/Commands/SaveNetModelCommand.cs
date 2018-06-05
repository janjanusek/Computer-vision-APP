using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.Services.Notifications;

namespace ImageClassifierApp.Commands
{
    public class SaveNetModelCommand : ICommand
    {
        private bool _isSaving;

        public bool CanExecute(object parameter)
        {
            return _isSaving == false;
        }

        public void Execute(object parameter)
        {
            var viewModel = (NetModelCreator) parameter;
            if (string.IsNullOrEmpty(viewModel.ModelName))
            {
                NotifyUser.NotifyUserByNotification(new Notification()
                {
                    Title = "model name error",
                    Message = "You have to specify name of model"
                });
                return;
            }
            else
            {
                try
                {
                    var plainModel = viewModel.GenerateModel();
                    plainModel.ConfigurationModel.ValidateModel();
                }
                catch (Exception e)
                {
                    NotifyUser.NotifyUserByNotification(new Notification()
                    {
                        Title = "Model structure is invalid",
                        Message = $"Something happend during savig neural net model.",
                        Exception = e
                    });
                    return;
                }
            }
            SaveFile(viewModel);
        }

        private async void SaveFile(NetModelCreator paModelCreator)
        {
            _isSaving = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            await Task.Run(() =>
            {
                try
                {
                    var model = paModelCreator.GenerateModel();
                    var json = model.ToJson();
                    FilePickerHelper.SaveFile(json, ViewModels.NetBuilderViewModel.CleanNetModel);
                }
                catch (Exception e)
                {
                    NotifyUser.NotifyUserByNotification(new Notification()
                    {
                        Title = "Something happend during saving model.",
                        Message = $"Make sure you have all permissions to write.",
                        Exception = e
                    });
                }
            });
            _isSaving = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}
