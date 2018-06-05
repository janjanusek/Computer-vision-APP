using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.ViewModels;

namespace ImageClassifierApp.Controls.Builder
{
    /// <summary>
    /// Interaction logic for LoadedNeuralNetsUserControl.xaml
    /// </summary>
    public partial class LoadedNeuralNetsUserControl : UserControl
    {
        public NetBuilderViewModel ViewModel => (NetBuilderViewModel) this.DataContext;

        public LoadedNeuralNetsUserControl()
        {
            InitializeComponent();
            ListControlHelper.RegisterAutoSelect(this.LoadedNetsListView);
        }

        private void RemoveNetModel(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            ViewModel.LoadedNetModels.Remove((NetModelBase) button.Tag);
        }

        private void OnModelSelected(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView) sender;
            ViewModel.SelectedNetModel = (NetModelBase)listView.SelectedItem;
        }
    }
}
