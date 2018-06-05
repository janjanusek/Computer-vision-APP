using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Controls.DragAndDrop;
using ImageClassifierApp.ViewModels;

namespace ImageClassifierApp.Controls.Builder
{
    /// <summary>
    /// Interaction logic for BuilderUserControl.xaml
    /// </summary>
    public partial class BuilderUserControl : UserControl
    {
        public NetBuilderViewModel ViewModel => (NetBuilderViewModel)this.DataContext;

        public BuilderUserControl()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
            DragAndDropControl.RegisteredTypes = new FileTypeModel(NetBuilderViewModel.CleanNetModel, NetBuilderViewModel.DirtyNetModel) { Description = "Neural net model" };
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if(ViewModel != null)
                DragAndDropControl.DataRecievedFromDropOperation += ViewModel.LoadNeuralNet;
        }
    }
}
