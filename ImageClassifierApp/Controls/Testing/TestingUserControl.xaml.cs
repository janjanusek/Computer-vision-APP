using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Controls.DragAndDrop;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.ViewModels;
using ImageClassifierApp.ViewModels.Testing;

namespace ImageClassifierApp.Controls.Testing
{
    /// <summary>
    /// Interaction logic for TestingUserControl.xaml
    /// </summary>
    public partial class TestingUserControl : UserControl
    {
        public TestingViewModel ViewModel => (TestingViewModel)this.DataContext;

        public TestingUserControl()
        {
            InitializeComponent();
            DragAndDropControl.RegisteredTypes = new FileTypeModel(NetBuilderViewModel.DirtyNetModel);
            ListControlHelper.RegisterAutoSelect(this.ComboBox);
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var newValue = dependencyPropertyChangedEventArgs.NewValue;
            var oldValue = dependencyPropertyChangedEventArgs.OldValue;
            if (oldValue != null)
                DragAndDropControl.DataRecievedFromDropOperation -= ViewModel.OnTrainetNetModelsRecieved;
            if (newValue != null)
                DragAndDropControl.DataRecievedFromDropOperation += ViewModel.OnTrainetNetModelsRecieved;
        }

        private async void SelectTrainedModel(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadedNetModelSelected(((sender as Button)?.Tag as TrainedNetModel));
        }
    }
}
