using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.ViewModels;

namespace ImageClassifierApp.Controls.Datasets
{
    public class DataSetConfirmationEventArgs : EventArgs
    {
        public DataSetModel DataSetModel { get; set; }
        public List<Dir> Dirs { get; set; }
        public DatasetsViewModel DatasetsViewModel { get; set; }
    }

    /// <summary>
    /// Interaction logic for DataSetConfigurationUserControl.xaml
    /// </summary>
    public partial class DataSetConfigurationUserControl : UserControl
    {
        public DataSetConfirmationEventArgs DataSetConfirmationEventArgs { get; set; }
        public new Window Parent { get; set; }
        public event EventHandler<DataSetConfirmationEventArgs> SettingsApproved;

        public DataSetConfigurationUserControl()
        {
            InitializeComponent();
        }

        private void ConfirmSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsApproved?.Invoke(this, DataSetConfirmationEventArgs);
        }
    }
}
