using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AiSdk.Configuration.Configurator;
using AiSdk.NeuralNet.Event;
using AiSdk.NeuralNet.Gpu;
using AiSdk.NeuralNet.Interfaces;
using ImageClassifierApp.Commands;
using ImageClassifierApp.Controls.Training;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.Objects.Extensions;
using ImageClassifierApp.Properties;
using ImageClassifierApp.Services.Notifications;
using ImageClassifierApp.Services.SharedResources;
using ImageClassifierApp.Services.Testing;
using OxyPlot;

namespace ImageClassifierApp.Models.Training
{
    public class TrainingModel : IDisposable, INotifyPropertyChanged
    {
        #region Props and fields

        public ObservableCollection<DataPoint> ErrorFunctionValues { get; set; }
        public ObservableCollection<DataPoint> SuccessValues { get; set; }
        public List<(int epoch, float learningRate, float momentum)> TrainingParams { get; set; }
        public NetModelBase Model { get; }
        public DataSetModel DataSetModel { get; }
        public GpuCard GpuCard { get; set; }
        public string CreationTime { get; set; }
        public string TrainingName { get; set; }
        public string GpuCardDescription { get; set; }
        public string MeanEpochTime { get; set; }
        public INetwork Network { get; private set; }
        public string NetType { get; set; }
        public float LearningRate { get; set; }
        public float Momentum { get; set; }
        public int Epochs { get; set; }
        public int CurrentEpoch { get; set; }
        public float CurrentError { get; set; }
        public Task TrainingTask { get; private set; }
        public bool IsTraining { get; private set; }
        public bool IsEpochStopping { get; set; }
        public bool IsImmediateStopping { get; set; }
        public bool UseStopRule { get; set; }
        public float? StopRule { get; set; }
        public bool IsDisposed { get; set; }
        public bool InTesting { get; set; }
        public TimeSpan MeanEpochTimeStruct { get; private set; }
        private int _measureSuccessRateEvery;
        public int MeasureSuccessRateEvery
        {
            get => _measureSuccessRateEvery;
            set => _measureSuccessRateEvery = Math.Max(1, value);
        }
        public GenerateSourceCodeCommand GenerateSourceCodeCommand { get; set; }
        public StopTrainingCommand StopTrainingCommand { get; set; }
        public StartTrainingCommand StartTrainingCommand { get; set; }
        public SaveTrainedModelCommand SaveTrainedModelCommand { get; set; }
        public ImmediateStopTrainingCommand ImmediateStopTrainingCommand { get; set; }
        public SaveTrainedNetForDllCommand SaveTrainedNetForDllCommand { get; set; }
        public SingleTrainingUserControl UserControl { get; set; }
        public SwitchToTestingCommand SwitchToTestingCommand { get; set; }
        public PrintReportCommand PrintReportCommand { get; set; }
        private float _stopWhenSuccessUnder;

        public float StopWhenSuccessUnder
        {
            get => _stopWhenSuccessUnder;
            set
            {
                if (value > 0 && value < 1)
                {
                    _stopWhenSuccessUnder = value;
                }
                else
                {
                    if (value < 0)
                    {
                        _stopWhenSuccessUnder = 0.01f;
                    }
                    else
                    {
                        var decimals = (float)Math.Pow(10, value.ToString().Split('.').First().Length);
                        _stopWhenSuccessUnder = value / decimals;
                    }
                }
            }
        }
        public bool UseSuccessRule { get; set; }
        public ObservableCollection<DataSetModel> CompatibleDataSets { get; set; }
        public DataSetModel TestingDataSet { get; set; }

        #endregion

        #region Construct

        public TrainingModel(NetModelBase paModel, DataSetModel paDataSetModel, GpuCard paGpu) : this()
        {
            CreationTime = DateTime.Now.ToString("f", new CultureInfo("en-US"));
            UserControl = new SingleTrainingUserControl();
            ErrorFunctionValues = new ObservableCollection<DataPoint>();
            Model = paModel;
            DataSetModel = paDataSetModel;
            GpuCard = paGpu;
            GpuCard.UseGpuCard();
            InitNetwork();
            UpdateDataSets();
        }

        public TrainingModel()
        {
            Epochs = 1000;
            LearningRate = 0.001f;
            Momentum = 0;
            StopRule = 0.02f;
            StopWhenSuccessUnder = 0.8f;
            UseStopRule = false;
            UseSuccessRule = false;
            InTesting = false;
            MeasureSuccessRateEvery = 1;
            GenerateSourceCodeCommand = new GenerateSourceCodeCommand(this);
            StopTrainingCommand = new StopTrainingCommand(this);
            StartTrainingCommand = new StartTrainingCommand(this);
            SaveTrainedModelCommand = new SaveTrainedModelCommand(this);
            ImmediateStopTrainingCommand = new ImmediateStopTrainingCommand(this);
            SaveTrainedNetForDllCommand = new SaveTrainedNetForDllCommand(this);
            SwitchToTestingCommand = new SwitchToTestingCommand(this);
            CompatibleDataSets = new ObservableCollection<DataSetModel>();
            SuccessValues = new ObservableCollection<DataPoint>();
            PrintReportCommand = new PrintReportCommand(this);
            TrainingParams = new List<(int epoch, float learningRate, float momentum)>();
            SharedDataService.Instance.LoadedDataSets.CollectionChanged += LoadedDataSetsOnCollectionChanged;
        }

        #endregion

        private void LoadedDataSetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateDataSets();
        }

        private void UpdateDataSets()
        {
            CompatibleDataSets = new ObservableCollection<DataSetModel>
            (
                SharedDataService.Instance.LoadedDataSets
                    .Where(d => DataSetModel?.MetaData?.IsCompatible(d.MetaData) == true)
            );
        }

        private bool InitNetwork()
        {
            try
            {
                if (Network == null)
                {
                    Network = Model.GetNetwork(GpuCard, DataSetModel);
                    Network.EpochTrainingInfo += NetworkOnEpochTrainingInfo;
                    NetType = Model.GeneratedNetworkType;
                }
            }
            catch (Exception e)
            {
                NotifyUser.NotifyUserByMessage("Something bad happend during initiating neural network.", e);
            }
            return Network != null;
        }

        private void NetworkOnEpochTrainingInfo(object sender, TrainingInfoEventArgs trainingInfoEventArgs)
        {
            var network = trainingInfoEventArgs.Network;
            var epoch = CurrentEpoch + 1;

            if (float.IsNaN(trainingInfoEventArgs.Network.ErrorFunction.Mean) == false)
                this.MeasureSuccess(trainingInfoEventArgs, epoch);
            else
                trainingInfoEventArgs.Network.ImmediateStop();

            App.RunInUiThread(() =>
            {
                CurrentEpoch++;
                Epochs--;
                ErrorFunctionValues.Add(new DataPoint(epoch, network.ErrorFunction.Mean));
                CurrentError = trainingInfoEventArgs.EpochError;
                MeanEpochTimeStruct = trainingInfoEventArgs.MeanEpochTime;
                MeanEpochTime = trainingInfoEventArgs.MeanEpochTime.ToString("g", CultureInfo.InvariantCulture);
                TrainingParams.Add((epoch, LearningRate, Momentum));
            });
        }

        private void MeasureSuccess(TrainingInfoEventArgs paTrainingInfo, int paCurrentEpoch)
        {
            if (UseSuccessRule && TestingDataSet != null && paCurrentEpoch % MeasureSuccessRateEvery == 0)
            {
                if (Network?.ImmediteStop != true)
                {
                    var networkTester = new NetworkTester(Network, TestingDataSet);
                    networkTester.RunTest();
                    paTrainingInfo.StopTraining = networkTester.Correctness > StopWhenSuccessUnder;
                    App.RunInUiThread(() =>
                    {
                        SuccessValues.Add(new DataPoint(paCurrentEpoch, networkTester.Correctness));
                    });
                }
            }
        }

        public async Task TrainNet()
        {
            if (InTesting)
            {
                NotifyUser.NotifyUserByMessage("You cannot train this neural net until testing procedure finish.");
                return;
            }

            if (IsTraining)
                return;
            if (InitNetwork() == false)
                return;
            else
                IsTraining = true;

            Network?.SetStopRule(UseStopRule ? StopRule : null);
            TrainingTask = Task.Run(() =>
            {
                try
                {
                    Network.TrainRandomBatch(DataSetModel.DataSet, Epochs, LearningRate, Momentum);
                }
                catch (Exception e)
                {
                    NotifyUser.NotifyUserByNotification(
                        new Notification()
                        {
                            Title = "Training issue",
                            Message = $"Something bad happend during training. Please make sure that you have enough free memory left.",
                            Exception = e
                        });
                }
            });
            await TrainingTask;
            DisposeTask();
        }

        private void DisposeTask()
        {
            if (TrainingTask != null)
                Task.WaitAll(TrainingTask.AsCollection<Task, Task>().ToArray());
            App.RunInUiThread(() =>
            {
                TrainingTask?.Dispose();
                TrainingTask = null;
                IsTraining = false;
            });
        }

        public async Task StopTraining()
        {
            if (IsTraining && IsEpochStopping == false && IsImmediateStopping == false)
            {
                IsEpochStopping = true;
                var task = Task.Run(() =>
                {
                    Network.StopAfterEpoch();
                    this.DisposeTask();
                });
                await task;
                task.Dispose();
                IsEpochStopping = false;
            }
        }

        public async Task ImmediateStop()
        {
            if (IsTraining && IsImmediateStopping == false)
            {
                IsImmediateStopping = true;
                var task = Task.Run(() =>
                {
                    Network.ImmediateStop();
                    this.DisposeTask();
                });
                await task;
                task.Dispose();
                IsImmediateStopping = false;
            }
        }

        public void Dispose()
        {
            DisposeTask();
            if (Network != null)
            {
                Network.ImmediateStop();
                Network.EpochTrainingInfo -= NetworkOnEpochTrainingInfo;
                Network.Dispose();
            }
            SharedDataService.Instance.LoadedDataSets.CollectionChanged += LoadedDataSetsOnCollectionChanged;
            UserControl = null;
            GpuCard.Dispose();
            GpuCard = null;
            IsDisposed = true;
        }

        public MementoConfigurationModel GetTrainedModel()
        {
            if (IsTraining || Network == null)
                return null;
            return new MementoConfigurationModel(Network.GetMemento());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
