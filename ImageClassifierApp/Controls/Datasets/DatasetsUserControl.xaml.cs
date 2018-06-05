using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.ViewModels;

namespace ImageClassifierApp.Controls.Datasets
{
    /// <summary>
    /// Interaction logic for DatasetsUserControl.xaml
    /// </summary>
    public partial class DatasetsUserControl : UserControl
    {
        public DatasetsViewModel ViewModel => (DatasetsViewModel) this.DataContext;

        public DatasetsUserControl()
        {
            InitializeComponent();
            ListControlHelper.RegisterAutoSelect(this.LoadedDataSetsListView);
        }

        private void RemoveDataset(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            ViewModel.RemoveDataSet((DataSetModel) button.Tag);
        }
    }
}
