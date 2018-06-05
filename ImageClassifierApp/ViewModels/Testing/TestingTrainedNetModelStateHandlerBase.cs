using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AiSdk.NeuralNet.Interfaces;
using ImageClassifierApp.Controls.DragAndDrop;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Objects.Extensions;
using ImageClassifierApp.Services.Notifications;
using ImageClassifierApp.Services.Testing;

namespace ImageClassifierApp.ViewModels.Testing
{
    public class TestingTrainedNetModelStateHandlerBase : TestingStateHandlerBase
    {
        public override DataSetMetaData DataSetMetaData => TrainedNetModel?.DataSetMetaData;
        public override string ModelName => TrainedNetModel?.ModelName;
        public TrainedNetModel TrainedNetModel { get; set; }
        private INetwork _networkToTest;

        public TestingTrainedNetModelStateHandlerBase(TestingViewModel paTestingViewModel) : base(paTestingViewModel)
        {
        }

        public override async Task<int[][]> GenerateConfusionMatrix()
        {
            try
            {
                if (_networkToTest == null)
                    _networkToTest = TrainedNetModel.GetNetwork(null, DataSetModel);
                NetworkTester tester = null;
                var matrix = await Task.Run(() =>
                {
                    tester = new NetworkTester(_networkToTest, DataSetModel);
                    tester.RunTest();

                    return tester.GenerateConfusionMatrixData();
                });
                SuccessRatio = tester.Correctness;
                return matrix;
            }
            catch (Exception e)
            {
                _networkToTest = null;
                NotifyUser.NotifyUserByMessage("Network failed to load.", e);
                return null;
            }
        }

        public override async Task Init(object paData)
        {
            var files = paData as DragAndDropEventArgs;
            if (paData is TrainedNetModel netModel)
            {
                TrainedNetModel = netModel;
                return;
            }
            if (files == null)
                return;

            const string message = "Loading of file '{0}' failed. Make sure that it's not corrupted.";
            if (files.FilesPaths.ContainsKey(NetBuilderViewModel.DirtyNetModel))
            {
                IsWorking = true;
                await files.FilesPaths[NetBuilderViewModel.DirtyNetModel]
                    .TryOrDefault<string, Exception>(File.ReadAllText, (ex, s) =>
                    {
                        NotifyUser.NotifyUserByMessage(string.Format(message, s));
                    }).Where(json => string.IsNullOrEmpty(json) == false)
                    .Select(ParseTrainedModel).ForEach(model => TrainedNetModel = model).ExecuteQueryAsync();
                IsWorking = false;
            }
        }

        private TrainedNetModel ParseTrainedModel(string paJson)
        {
            try
            {
                var model = NetModelBase.FromJsonToInstance<TrainedNetModel>(paJson);
                if (model?.DataSetMetaData?.ClassLabels?.Any() != true)
                {
                    NotifyUser.NotifyUserByNotification(new Notification()
                    {
                        Title = "Invalid model",
                        Message = "This model doesn't seems to have valid dataset metadata. Please make sure this file is not corrupted."
                    });
                    return null;
                }
                return model;
            }
            catch (Exception e)
            {
                NotifyUser.NotifyUserByMessage("Something bad happend during model loading.", e);
                Debug.WriteLine(e);
                return default(TrainedNetModel);
            }
        }

        public override void Dispose()
        {
            OnDisposing();
            TrainedNetModel = null;
            _networkToTest?.Dispose();
            _networkToTest = null;
        }
    }
}