using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ImageClassifierApp.Helpers
{
    /// <summary>
    /// Class adds DataContext handlers for auto selection of first item if it's available
    /// </summary>
    public static class ListControlHelper
    {
        public static void RegisterAutoSelect(params Selector[] paControls)
        {
            foreach (var itemsControl in paControls)
            {
                itemsControl.DataContextChanged += ItemsControlOnDataContextChanged;
            }
        }

        private static void ItemsControlOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var selector = (Selector)sender;
            OnSelectFirst(selector);
            if (selector.DataContext is INotifyPropertyChanged propChanged)
            {
                propChanged.PropertyChanged += (o, args) =>
                {
                    OnSelectFirst(selector);
                };
            }
        }

        private static void OnSelectFirst(Selector paSelector)
        {
            if (paSelector.SelectedIndex == -1 && paSelector.Items.Count > 0)
            {
                paSelector.SelectedIndex = 0;
            }
        }
    }
}
