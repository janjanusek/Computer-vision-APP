using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.ViewModels;
using ImageClassifierApp.Views;

namespace ImageClassifierApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel => (MainViewModel) this.DataContext;
        public static MainViewModel CurrentInstance { get; private set; }


        public MainWindow()
        {
            InitializeComponent();
            CurrentInstance = ViewModel;
            this.Loaded += OnLoaded;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var dialog = new YesNoDialog {Message = "Do you really want to close this application?"};
            e.Cancel = dialog.ShowDialog() == false;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (var viewModel in ViewModel.ViewModels)
            {
                await viewModel.Init();
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectViewModel((sender as ListView)?.SelectedItem as PartialViewModelBase);
        }
    }
}
