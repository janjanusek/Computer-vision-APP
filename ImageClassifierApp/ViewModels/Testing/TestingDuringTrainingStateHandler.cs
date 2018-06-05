using System.ComponentModel;
using System.Threading.Tasks;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Services.Notifications;
using ImageClassifierApp.Services.Testing;

namespace ImageClassifierApp.ViewModels.Testing
{
    public class TestingDuringTrainingStateHandler : TestingStateHandlerBase
    {
        public override DataSetMetaData DataSetMetaData => TrainingModel?.DataSetModel?.MetaData;
        public override string ModelName => TrainingModel?.Model.ModelName;
        public TrainingModel TrainingModel { get; set; }

        public TestingDuringTrainingStateHandler(TestingViewModel paTestingViewModel) : base(paTestingViewModel)
        {
        }

        public override async Task<int[][]> GenerateConfusionMatrix()
        {
            if (TrainingModel.IsTraining)
            {
                App.RunInUiThread(() =>
                {
                    NotifyUser.NotifyUserByMessage("Cannot run test. Neural network training is in progress. If you need test neural network please stop training first.");
                });
                return null;
            }
            if (TrainingModel.Network == null)
                return null;
            int[][] confusionMatrix = null;
            try
            {
                TrainingModel.InTesting = true;
                var tester = new NetworkTester(TrainingModel.Network, DataSetModel);
                var task = Task.Run(() =>
                {
                    tester.RunTest();
                    return tester.GenerateConfusionMatrixData();
                });
                confusionMatrix = await task;
                task.Dispose();
                SuccessRatio = tester.Correctness;
            }
            finally
            {
                TrainingModel.InTesting = false;
            }
            return confusionMatrix;
        }

        public override Task Init(object paData)
        {
            TrainingModel = (TrainingModel) paData;
            TrainingModel.PropertyChanged += TrainingModelOnPropertyChanged;
            return Task.CompletedTask;
        }

        private void TrainingModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (TrainingModel?.IsDisposed == true)
            {
                TrainingModel.PropertyChanged -= TrainingModelOnPropertyChanged;
                TrainingModel = null;
            }
        }

        public override void Dispose()
        {
            OnDisposing();
            TrainingModel = null;
        }
    }
}