using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.ViewModels;

namespace ImageClassifierApp.Services.SharedResources
{
    /// <summary>
    /// Class provides access to shared resources such as loaded datasets and loaded NetModels across whole app
    /// </summary>
    public class SharedDataService
    {
        private static SharedDataService _instance;
        public static SharedDataService Instance => _instance ?? (_instance = new SharedDataService());

        private SharedDataService()
        {
            LoadedDataSets = new ObservableCollection<DataSetModel>();
            LoadedModels = new ObservableCollection<NetModelBase>();
        }

        public ObservableCollection<DataSetModel> LoadedDataSets { get; set; }
        public ObservableCollection<NetModelBase> LoadedModels { get; set; }


        public static async Task<bool> OpenModuleAndSetData<T>(object paData) where T : PartialViewModelBase
        {
            var viewModel = MainWindow.CurrentInstance?.ViewModels.FirstOrDefault(model => model.GetType() == typeof(T));
            if (viewModel == null)
                return false;
            var currentViewModel = MainWindow.CurrentInstance.CurrentPartialViewModel;
            MainWindow.CurrentInstance.SelectViewModel(viewModel);
            var result = await viewModel.SetData(paData);
            if (result == false)
                MainWindow.CurrentInstance.SelectViewModel(currentViewModel);
            return result;
        }
    }
}
