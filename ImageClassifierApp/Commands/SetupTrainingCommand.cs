using System;
using System.Windows;
using System.Windows.Input;
using ImageClassifierApp.Controls.Training;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Services.Notifications;
using ImageClassifierApp.ViewModels;
using ImageClassifierApp.Views;

namespace ImageClassifierApp.Commands
{
    public class SetupTrainingCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var viewModel = (TrainingNetViewModel)parameter;
            var userControl = new TrainingSettingsUserControl()
            {
                DataContext = viewModel
            };
            var window = UserControlWindow.ShowUserControlWindow(userControl, "training setup");
            userControl.TrainingApproved += (sender, args) =>
            {
                if (args.IsValid())
                {
                    if(args.NetModel is TrainedNetModel model)
                    {
                        if (model.DataSetMetaData.IsCompatible(args.TrainingDataSet.MetaData) == false)
                        {
                            NotifyUser.NotifyUserByMessage("Data set and net model are not compatible.");
                            return;
                        }
                    }
                    viewModel.AddModel(args.NetModel, args.TrainingDataSet, args.GpuCard);
                    window.Close();
                }
                else
                {
                    NotifyUser.NotifyUserByMessage("Please select all fields.");
                }
            };
            window.ResizeMode = ResizeMode.NoResize;
            window.Width = 300;
            window.Height = 280;
            window.Show();
        }

        public event EventHandler CanExecuteChanged;
    }
}
