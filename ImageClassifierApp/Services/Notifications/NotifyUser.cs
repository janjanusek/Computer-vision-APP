using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ImageClassifierApp.Properties;

namespace ImageClassifierApp.Services.Notifications
{
    /// <summary>
    /// Class provides notifications for user instead of default ugly Dialog class without support of styling
    /// </summary>
    public static class NotifyUser
    {
        public static void NotifyUserByNotification(Notification paNotification)
        {
            App.RunInUiThread(() =>
            {
                NotificationManager.NotificationManagerResources.RegisterNotification(paNotification);
            });
        }

        public static void NotifyUserByMessage(string paMessage, Exception paException = null)
        {
            App.RunInUiThread(() =>
            {
                NotificationManager.NotificationManagerResources.RegisterNotification(new Notification()
                {
                    Title = "Warning",
                    Message = paMessage,
                    Exception = paException
                });
            });
        }
    }

    public class NotificationManager : INotifyPropertyChanged
    {
        public static readonly string ResourcesName = nameof(NotificationManager);

        public static NotificationManager NotificationManagerResources =>
            (NotificationManager)Application.Current.Resources[NotificationManager.ResourcesName];

        private volatile Stack<Notification> _notifications;
        public bool Any => _notifications.Count != 0;

        public Notification CurrentNotification { get; set; }

        public NotificationManager()
        {
            _notifications = new Stack<Notification>();
        }

        public void CurrentNotificationClosed()
        {
            CurrentNotification = Any ? _notifications.Pop() : null;
        }

        public void RegisterNotification(Notification paNotification)
        {
            App.RunInUiThread(() =>
            {
                if (CurrentNotification == null)
                {
                    CurrentNotification = paNotification;
                }
                else
                {
                    _notifications.Push(paNotification);
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Notification
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public string ExceptionInfo { get; set; }

        public Exception Exception
        {
            set => ExceptionInfo = value == null ? null : $"Message: {value.Message}\nStackTrace: \n{value.StackTrace}";
        }
    }
}
