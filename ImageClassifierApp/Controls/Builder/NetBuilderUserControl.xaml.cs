using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Controls.ReorderListView;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.NetModel;

namespace ImageClassifierApp.Controls.Builder
{
    /// <summary>
    /// Interaction logic for NetBuilderUserControl.xaml
    /// </summary>
    public partial class NetBuilderUserControl : UserControl
    {
        public NetModelBase ViewModel => (NetModelBase)this.DataContext;

        public NetBuilderUserControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            ListControlHelper.RegisterAutoSelect(LayersListView, SupportedComboBox);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (ViewModel != null && ViewModel.CanBeChanged)
                new ListViewDragDropManager<LayerInfo>(LayersListView);
        }


        private void AddLayer(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.Tag != null && ViewModel is NetModelCreator creatorViewModel)
                creatorViewModel.AddLayer((LayerInfo)button.Tag);
        }

        private void RemoveLayerClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.Tag != null && ViewModel is NetModelCreator creatorViewModel)
                creatorViewModel.RemoveLayer((LayerInfo)button.Tag);
        }
    }
}
