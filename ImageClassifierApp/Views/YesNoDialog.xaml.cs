using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ImageClassifierApp.Properties;

namespace ImageClassifierApp.Views
{
    /// <summary>
    /// Interaction logic for YesNoDialog.xaml
    /// </summary>
    public partial class YesNoDialog : Window, INotifyPropertyChanged
    {
        public string Message { get; set; }

        public YesNoDialog()
        {
            Message = "This is very long message for design purporses. Do you agree with thi imagenary question?";
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public static bool? ShowDialog(string paMessage)
        {
            var window = new YesNoDialog()
            {
                Message = paMessage,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            return window.ShowDialog();
        }

        private void OnNoClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OnYesClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
