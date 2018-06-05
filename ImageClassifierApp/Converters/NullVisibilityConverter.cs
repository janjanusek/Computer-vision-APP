using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ImageClassifierApp.Converters
{
    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(parameter?.ToString()))
                return value != null ? Visibility.Visible : Visibility.Collapsed;
            return value != null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
