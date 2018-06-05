using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Models.Training;
using ImageClassifierApp.ViewModels;
using ImageClassifierApp.Views;

namespace ImageClassifierApp.Controls.Training
{
    /// <summary>
    /// Interaction logic for TrainingUserControl.xaml
    /// </summary>
    public partial class TrainingUserControl : UserControl
    {
        public TrainingNetViewModel ViewModel => (TrainingNetViewModel) this.DataContext;

        public TrainingUserControl()
        {
            InitializeComponent();
        }

        private async void RemoveTraining(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var window = new YesNoDialog
            {
                Message = $"Are you sure that you want to delete this training?"
            };
            var result = window.ShowDialog();
            if (result == true)
                await ViewModel.RemoveModel((TrainingModel)button.Tag);
        }
    }
}
