using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Services.Notifications;

namespace ImageClassifierApp.Controls.Notifications
{
    /// <summary>
    /// Interaction logic for NotificationsUserControl.xaml
    /// </summary>
    public partial class NotificationsUserControl : UserControl
    {
        public NotificationManager ViewModel => (NotificationManager) this.DataContext;

        public NotificationsUserControl()
        {
            InitializeComponent();
        }

        private void CloseNotification(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentNotificationClosed();
        }
    }
}
