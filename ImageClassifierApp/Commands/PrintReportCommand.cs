using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Objects.Extensions;
using ImageClassifierApp.Services.Notifications;

namespace ImageClassifierApp.Commands
{
    public class PrintReportCommand : ViewModelChangedCommand<TrainingModel>
    {
        public override Func<TrainingModel, bool> CanExecuteFunc => model => model.IsTraining == false;

        public PrintReportCommand(TrainingModel paViewModel) : base(paViewModel)
        {
            
        }

        public override void Execute(object parameter)
        {
            try
            {
                var builder = new StringBuilder();
                builder.AppendLine(
                    $"Training dataset: {ViewModel.DataSetModel?.Name}, info: {ViewModel.DataSetModel?.MetaData.Description}");
                builder.AppendLine(
                    $"Testing dataset: {ViewModel.TestingDataSet?.Name}, info: {ViewModel.TestingDataSet?.MetaData.Description}");
                builder.AppendLine(
                    $"Approximated training time {TimeSpan.FromMilliseconds(Enumerable.Repeat(ViewModel.MeanEpochTimeStruct, ViewModel.CurrentEpoch).Sum(t => t.TotalMilliseconds)).ToString("g", CultureInfo.InvariantCulture)}");
                builder.AppendLine(string.Join(string.Empty, Enumerable.Repeat('-', 100)));
                builder.AppendLine("Training params overview:");
                ViewModel.TrainingParams.ForEach(p =>
                    builder.AppendLine($"epoch: {p.epoch}, learning rate: {p.learningRate}, momentum: {p.momentum}"));
                builder.AppendLine(string.Join(string.Empty, Enumerable.Repeat('-', 100)));
                builder.AppendLine("Success rates:");
                ViewModel.SuccessValues.ForEach(s => builder.AppendLine($"epoch: {s.X}, success: {s.Y}")).ExecuteQuery();
                builder.AppendLine(string.Join(string.Empty, Enumerable.Repeat('-', 100)));
                builder.AppendLine("Error function values:");
                ViewModel.ErrorFunctionValues.ForEach(s => builder.AppendLine($"epoch: {s.X}, error: {s.Y}")).ExecuteQuery();
                var path = FilePickerHelper.AskForFilePath("txt");
                File.WriteAllText(path, builder.ToString());
            }
            catch (Exception e)
            {
                NotifyUser.NotifyUserByMessage("Something went wrong during printing report into file.", e);
            }
        }
    }
}
