using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Services.Async;
using ImageClassifierApp.ViewModels;

namespace ImageClassifierApp.Controls.Datasets
{
    /// <summary>
    /// Interaction logic for DatasetLoaderUserControl.xaml
    /// </summary>
    public partial class DatasetLoaderUserControl : UserControl
    {
        public DatasetsViewModel ViewModel => (DatasetsViewModel)this.DataContext;

        public DatasetLoaderUserControl()
        {
            InitializeComponent();
            ListControlHelper.RegisterAutoSelect(this.ListView);
        }

        private void StopLoading(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            ViewModel.CancelDataSetLoades((PendingDataSet)button.Tag);
        }
    }
}
