using System.Windows;
using System.Windows.Controls;
using PropertyChanged;

namespace ImageClassifierApp.Views
{
    /// <summary>
    /// Interaction logic for UserControlWindow.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class UserControlWindow : Window
    {
        public UserControl UserControl { get; set; }

        public UserControlWindow()
        {
            InitializeComponent();
        }

        public static UserControlWindow ShowUserControlWindow(UserControl paControl, string paTitle = "")
        {
            var window = new UserControlWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Title = paTitle,
                UserControl = paControl
            };
            return window;
        }
    }
}
