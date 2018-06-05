using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using ImageClassifierApp.Commands;
using ImageClassifierApp.Controls.Buttons;
using ImageClassifierApp.Controls.Datasets;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Services.Async;
using ImageClassifierApp.Services.Notifications;
using ImageClassifierApp.Services.SharedResources;

namespace ImageClassifierApp.ViewModels
{
    public class DatasetsViewModel : PartialViewModelBase, INotifyPropertyChanged
    {
        public ObservableCollection<PendingDataSet> PendingDataSets { get; set; }
        public ObservableCollection<DataSetModel> DataSetModels => SharedDataService.Instance.LoadedDataSets;
        private readonly DataSetLoader _dataSetLoader;

        public DataSetModel SelectedDataSet { get; set; }
        public bool AnyPendingDataSets => PendingDataSets.Count != 0;
        public bool AnyLoadedDataSets => DataSetModels.Count != 0;

        public DatasetsViewModel()
        {
            _dataSetLoader = DataSetLoader.Instance;
            _dataSetLoader.ModelLoaded += DataSetLoaderOnModelLoaded;
            Title = "data sets";
            PendingDataSets = new ObservableCollection<PendingDataSet>();
        }

        private void DataSetLoaderOnModelLoaded(object sender, PendingDataSet pendingDataSet)
        {
            PendingDataSets.Remove(pendingDataSet);
            if (pendingDataSet.Cancel == false)
            {
                DataSetModels.Add(pendingDataSet.Model);
                if (SelectedDataSet == null)
                    SelectedDataSet = DataSetModels.FirstOrDefault();
            }
            else if (pendingDataSet.OutOfMemory)
            {
                NotifyUser.NotifyUserByNotification(new Notification()
                {
                    Title = "Ooops!",
                    Message = $"Your dataset '{pendingDataSet.DataSetName}' caused OutOfMemory exception. Please try to load smaller dataset of try load images in dataset in smaller scale.",
                    Exception = pendingDataSet.ExceptionDescription
                });
            }
            else if (pendingDataSet.Exception)
            {
                NotifyUser.NotifyUserByMessage($"Something happend during loading {pendingDataSet.DataSetName} dataset.", pendingDataSet.ExceptionDescription);
            }
            OnPropertyChanged(nameof(AnyPendingDataSets));
            OnPropertyChanged(nameof(AnyLoadedDataSets));
        }

        protected override void RegisterButtons()
        {
            base.RegisterButton(new CharCodeButtonModel()
            {
                Title = "load new",
                CharCode = char.ConvertFromUtf32(0xE8B7),
                Command = new OpenDataSetCommand(),
                CommandParam = this
            });
        }

        protected override UserControl GetUserControl()
        {
            return new DatasetsUserControl();
        }

        public void LoadDataSet(IEnumerable<Dir> paDirs, DataSetModel paModel)
        {
            var pending = new PendingDataSet(paDirs, paModel);
            PendingDataSets.Add(pending);
            _dataSetLoader.RegisterDataSet(pending);
            OnPropertyChanged(nameof(AnyPendingDataSets));
        }

        public void CancelDataSetLoades(PendingDataSet paDataSet)
        {
            paDataSet.CancelLoading();
            OnPropertyChanged(nameof(AnyPendingDataSets));
        }

        public void RemoveDataSet(DataSetModel paModel)
        {
            DataSetModels.Remove(paModel);
            GcHelper.ForceCollect();
            OnPropertyChanged(nameof(AnyLoadedDataSets));
        }
    }
}
