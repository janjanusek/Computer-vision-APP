using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using AiSdk.NeuralNet.Gpu;
using ImageClassifierApp.Controls.Training;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.Objects.Extensions;
using ImageClassifierApp.Services.SharedResources;
using PropertyChanged;

namespace ImageClassifierApp.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class TrainingNetViewModel : PartialViewModelBase
    {
        public ObservableCollection<GpuCard> GpuCards { get; set; }
        public ObservableCollection<DataSetModel> TrainingDataSets => SharedDataService.Instance.LoadedDataSets;
        public ObservableCollection<NetModelBase> LoadedNetModels => SharedDataService.Instance.LoadedModels;
        public ObservableCollection<TrainingModel> TrainingModels { get; set; }
        public TrainingModel SelectedTraining { get; set; }

        public TrainingNetViewModel()
        {
            Title = "training";
            GpuCards = new ObservableCollection<GpuCard>();
            GpusRefresh();
            TrainingModels = new ObservableCollection<TrainingModel>();
            TrainingModels.CollectionChanged += (sender, args) => GpusRefresh();
        }

        private void GpusRefresh()
        {
            GpuCards.Clear();
            GpuCardManager.Instance.Where(g => g.IsInUse == false).ForEach(GpuCards.Add).ExecuteQuery();
        }

        public void AddModel(NetModelBase paModel, DataSetModel paTrainingDataSet, GpuCard paGpuCard)
        {
            var model = new TrainingModel(paModel, paTrainingDataSet, paGpuCard);
            model.PropertyChanged += ModelOnPropertyChanged;
            TrainingModels.Add(model);
        }

        private void ModelOnPropertyChanged(object o, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(TrainingModel.GpuCard))
            {
                var model = (TrainingModel)o;
                if (model.GpuCard == null)
                {
                    this.GpusRefresh();
                    model.PropertyChanged -= ModelOnPropertyChanged;
                }
            }
        }

        public async Task RemoveModel(TrainingModel paModel)
        {
            TrainingModels.Remove(paModel);
            await paModel.ImmediateStop();
            paModel.Dispose();
        }

        protected override void RegisterButtons()
        {

        }

        protected override UserControl GetUserControl()
        {
            return new TrainingUserControl() { DataContext = this };
        }
    }
}
