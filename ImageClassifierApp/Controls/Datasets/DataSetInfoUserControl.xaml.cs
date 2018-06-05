using System;
using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Classification;
using Image = System.Windows.Controls.Image;

namespace ImageClassifierApp.Controls.Datasets
{
    /// <summary>
    /// Interaction logic for DataSetInfoUserControl.xaml
    /// </summary>
    public partial class DataSetInfoUserControl : UserControl
    {
        public DataSetModel ViewModel => (DataSetModel)this.DataContext;

        public DataSetInfoUserControl()
        {
            InitializeComponent();
            ListControlHelper.RegisterAutoSelect(this.CategoryComboBox, this.PhotosListBox);
        }

        private void Dispose(object obj)
        {
            if (obj is IDisposable disposable)
            {
                App.RunInUiThread(() =>
                {
                    disposable.Dispose();
                });
            }
        }

        private void FrameworkElement_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var img = (Image)sender;
            Dispose(img.Source);
        }
    }
}
