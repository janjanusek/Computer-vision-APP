using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ImageClassifierApp.Objects.Extensions;
using Microsoft.Win32;
using PropertyChanged;

namespace ImageClassifierApp.Controls.DragAndDrop
{
    /// <summary>
    /// Interaction logic for DragAndDropControl.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class DragAndDropControl : UserControl
    {
        public event EventHandler<DragAndDropEventArgs> DataRecievedFromDropOperation;
        public FileTypeModel RegisteredTypes { get; set; }

        public DragAndDropControl()
        {
            InitializeComponent();
            TitleBrush = Brushes.Black;
            ProcessingFiles(false);
        }

        // Dependency Property
        public static readonly DependencyProperty TitleBrushProperty =
            DependencyProperty.Register(nameof(TitleBrush), typeof(SolidColorBrush),
                typeof(DragAndDropControl), null);

        // .NET Property wrapper
        public SolidColorBrush TitleBrush
        {
            get { return (SolidColorBrush)GetValue(TitleBrushProperty); }
            set { SetValue(TitleBrushProperty, value); }
        }

        protected virtual void OnDataRecievedFromDropOperation(DragAndDropEventArgs e)
        {
            DataRecievedFromDropOperation?.Invoke(this, e);
        }

        private void DragAndDropControl_OnDrop(object sender, DragEventArgs e)
        {
            var eventArgs = new DragAndDropEventArgs(this);
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                this.ProcessingFiles(true);
                SortData((string[])e.Data.GetData(DataFormats.FileDrop), eventArgs);
                OnDataRecievedFromDropOperation(eventArgs);
            }
            this.ProcessingFiles(eventArgs.WaitForAsync);
        }

        private void LoadFiles(object sender, MouseButtonEventArgs e)
        {
            if (RegisteredTypes?.Any != true)
                throw new Exception("You must specify at least one supported file type.");

            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = RegisteredTypes.ToString()
            };
            dialog.ShowDialog();
            if (!dialog.FileNames.Any())
                return;

            this.ProcessingFiles(true);
            var eventArgs = new DragAndDropEventArgs(this);
            SortData(dialog.FileNames, eventArgs);
            OnDataRecievedFromDropOperation(eventArgs);
            this.ProcessingFiles(eventArgs.WaitForAsync);
        }

        public void ProcessingFiles(bool paProcessing)
        {
            App.RunInUiThread(() =>
            {
                IsProcessingFiles.Visibility = paProcessing ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void SortData(string[] paPaths, DragAndDropEventArgs paSortedPapaEventArgs)
        {
            var supportedTypes = RegisteredTypes.FileTypes;
            paSortedPapaEventArgs.FilesPaths = new Dictionary<string, List<string>>();
            foreach (var supportedType in supportedTypes)
            {
                paSortedPapaEventArgs.FilesPaths.Add(supportedType, new List<string>());
            }

            foreach (var path in paPaths)
            {
                var type = $"{path.Split('.').Last().ToLower()}";
                var typeIndex = supportedTypes.IndexOf(type);
                if (typeIndex > -1)
                {
                    paSortedPapaEventArgs.FilesPaths[supportedTypes[typeIndex]].Add(path);
                }
            }
        }
    }

    public class DragAndDropEventArgs : EventArgs
    {
        public Dictionary<string, List<string>> FilesPaths { get; set; }

        private DragAndDropControl _dragAndDropControl;
        public bool WaitForAsync { get; set; }

        public void AsyncProcessingDone()
        {
            _dragAndDropControl.ProcessingFiles(false);
        }

        public DragAndDropEventArgs(DragAndDropControl paDragAndDropControl)
        {
            _dragAndDropControl = paDragAndDropControl;
            WaitForAsync = false;
        }
    }
}
