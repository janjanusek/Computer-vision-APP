using System;
using System.Linq;
using System.Windows.Input;
using ImageClassifierApp.Controls.Datasets;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Services.Notifications;
using ImageClassifierApp.ViewModels;
using ImageClassifierApp.Views;

namespace ImageClassifierApp.Commands
{
    public class OpenDataSetCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var viewModel = (DatasetsViewModel)parameter;
            var dirs = FolderPickerHelper.PickFolder();
            if ((dirs?.Any() == true) == false)
            {
                NotifyUser.NotifyUserByMessage("Please select folder which contains only folders and those folders represents classification classes. Everyone of them must contains images which belongs to specified class. Supported file types are png, jpg, jpeg, bmp.");
                return;
            }

            var model = new DataSetModel()
            {
                Name = FolderPickerHelper.LastPickedFolderName ?? $"dataset {viewModel.PendingDataSets.Count + viewModel.DataSetModels.Count}"
            };

            var userControl = new DataSetConfigurationUserControl()
            {
                DataContext = model,
                DataSetConfirmationEventArgs = new DataSetConfirmationEventArgs()
                {
                    DataSetModel = model,
                    DatasetsViewModel = viewModel,
                    Dirs = dirs
                }
            };
            userControl.SettingsApproved += UserControlOnSettingsApproved;
            var window = UserControlWindow.ShowUserControlWindow(userControl, "Dataset settings");
            window.Width = 310;
            window.Height = 400;
            userControl.Parent = window;
            window.ShowDialog();
        }

        private void UserControlOnSettingsApproved(object sender, DataSetConfirmationEventArgs e)
        {
            App.RunInUiThread(() =>
            {
                ((DataSetConfigurationUserControl)sender).Parent.Close();
                e.DatasetsViewModel.LoadDataSet(e.Dirs, e.DataSetModel);
            });
        }

        public event EventHandler CanExecuteChanged;
    }
}
