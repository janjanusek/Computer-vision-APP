using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using ImageClassifierApp.Commands;
using ImageClassifierApp.Controls.Buttons;
using ImageClassifierApp.Controls.DragAndDrop;
using ImageClassifierApp.Controls.Testing;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Services.Notifications;
using ImageClassifierApp.Services.SharedResources;
using ImageClassifierApp.Views;

namespace ImageClassifierApp.ViewModels.Testing
{
    public class TestingViewModel : PartialViewModelBase, INotifyPropertyChanged
    {
        private TestingStateHandlerBase _stateHandler;
        public ObservableCollection<NetModelBase> TrainedModels => SharedDataService.Instance.LoadedModels;
        public ObservableCollection<DataSetModel> SupportedDataSets { get; set; }
        public DataSetModel TestingDataSet { get; set; }
        private readonly object _lock = new object();
        public TestingStateHandlerBase StateHandler
        {
            get => _stateHandler;
            set
            {
                SharedDataService.Instance.LoadedDataSets.CollectionChanged -= LoadedDataSetsOnCollectionChanged;
                _stateHandler = value;
                UpdateDataSets();
                if (value != null)
                    SharedDataService.Instance.LoadedDataSets.CollectionChanged += LoadedDataSetsOnCollectionChanged;
            }
        }

        public TestingViewModel()
        {
            Title = "testing";
        }

        private void LoadedDataSetsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs collectionChanged) => UpdateDataSets();

        private void UpdateDataSets()
        {
            lock (_lock)
            {
                if (StateHandler == null)
                {
                    SupportedDataSets.Clear();
                    TestingDataSet = null;
                }
                else
                {
                    SupportedDataSets = new ObservableCollection<DataSetModel>
                    (
                        SharedDataService.Instance.LoadedDataSets
                            .Where(d => StateHandler?.DataSetMetaData?.IsCompatible(d.MetaData) == true)
                    );
                }
            }
        }

        public async Task<int[][]> GenerateConfusionMatrix()
        {
            IsWorking = true;
            if (TestingDataSet == null)
            {
                NotifyUser.NotifyUserByNotification(new Notification()
                {
                    Title = "Missing data set?",
                    Message = "Please select one of compatible data sets. If there is no any please load one."
                });
                IsWorking = false;
                return null;
            }
            try
            {
                if (_stateHandler != null)
                    return await _stateHandler.GenerateConfusionMatrix();
                else
                    return null;
            }
            catch (Exception e)
            {
                NotifyUser.NotifyUserByMessage(
                    $"Something happend during testing. Please make sure you have valid data.",e);
                return null;
            }
            finally
            {
                IsWorking = false;
            }
        }

        protected override void RegisterButtons()
        {
            base.RegisterButton(new CharCodeButtonModel()
            {
                CharCode = char.ConvertFromUtf32(0xE711),
                Title = "reset",
                Command = new ResetTesterCommand(this)
            });
        }

        protected override UserControl GetUserControl()
        {
            return new TestingUserControl();
        }

        private async Task<T> CreadeStateHandler<T>(object paData) where T : TestingStateHandlerBase
        {
            var t = (T)Activator.CreateInstance(typeof(T), this);
            await t.Init(paData);
            t.Disposing += OnStateHandlerDisposing;
            return t;
        }

        private void OnStateHandlerDisposing(object sender, EventArgs eventArgs)
        {
            lock (StateHandler)
            {
                if (StateHandler == sender)
                {
                    StateHandler.Disposing -= OnStateHandlerDisposing;
                    StateHandler = null;
                }
            }
        }

        public async void OnTrainetNetModelsRecieved(object sender, DragAndDropEventArgs e)
        {
            StateHandler?.Dispose();
            StateHandler = await CreadeStateHandler<TestingTrainedNetModelStateHandlerBase>(e);
        }

        /// <summary>
        /// Method is used as switch from training into testing
        /// If PlainNetModel is Recieved it must be first converted into TrainedNetModel
        /// </summary>
        /// <param name="paData"></param>
        /// <returns></returns>
        public override async Task<bool> SetData(object paData)
        {
            var allowed = await Task.Run(() =>
            {
                if (!(paData is TrainingModel trainingModel))
                    return false;
                return App.RunInUiThread(() =>
                {
                    if (this.StateHandler != null)
                    {
                        var result = YesNoDialog.ShowDialog(
                            $"Currently trainer has opened model {StateHandler.ModelName}. Are you sure you want to close this model and load model {trainingModel.Model.ModelName} instead?");
                        if (result != true)
                            return false;
                    }
                    return true;
                });
            });
            if (allowed)
            {
                StateHandler?.Dispose();
                StateHandler = await CreadeStateHandler<TestingDuringTrainingStateHandler>(paData);
            }
            return allowed;
        }

        public void Reset()
        {
            StateHandler?.Dispose();
            TestingDataSet = null;
        }

        public async Task LoadedNetModelSelected(TrainedNetModel paTrainedNetModel)
        {
            StateHandler?.Dispose();
            StateHandler = await CreadeStateHandler<TestingTrainedNetModelStateHandlerBase>(paTrainedNetModel);
        }
    }
}
