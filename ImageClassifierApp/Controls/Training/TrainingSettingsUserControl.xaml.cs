using System;
using System.Windows;
using System.Windows.Controls;
using AiSdk.NeuralNet.Gpu;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Models.NetModel;

namespace ImageClassifierApp.Controls.Training
{
    public class TrainingSetupEventArgs : EventArgs
    {
        public DataSetModel TrainingDataSet { get; set; }
        public GpuCard GpuCard { get; set; }
        public NetModelBase NetModel { get; set; }

        public bool IsValid()
        {
            return TrainingDataSet != null && (GpuCard != null || GpuCard is NullGpu) && NetModel != null;
        }
    }
    /// <summary>
    /// Interaction logic for TrainingSettingsUserControl.xaml
    /// </summary>
    public partial class TrainingSettingsUserControl : UserControl
    {
        public event EventHandler<TrainingSetupEventArgs> TrainingApproved;

        public TrainingSettingsUserControl()
        {
            InitializeComponent();
            ListControlHelper.RegisterAutoSelect(this.Gpus, this.NetModels, this.TrainingDataSets);
        }

        private void OnTrainingApproved(object sender, RoutedEventArgs e)
        {
            TrainingApproved?.Invoke(this, new TrainingSetupEventArgs()
            {
                TrainingDataSet = (DataSetModel) TrainingDataSets.SelectedItem,
                GpuCard = (GpuCard) Gpus.SelectedItem,
                NetModel = (NetModelBase) NetModels.SelectedItem
            });
        }
    }
}
