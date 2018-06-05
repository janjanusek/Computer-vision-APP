using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ImageClassifierApp.Helpers;
using ImageClassifierApp.Objects.Extensions;
using ImageClassifierApp.ViewModels.Testing;

namespace ImageClassifierApp.Controls.Testing
{
    /// <summary>
    /// Interaction logic for BatchTesterControl.xaml
    /// </summary>
    public partial class BatchTesterControl : UserControl
    {
        public TestingViewModel ViewModel => (TestingViewModel)this.DataContext;

        public BatchTesterControl()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
            ListControlHelper.RegisterAutoSelect(this.SupportedDataSetsComboBox);
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {

            if (args.OldValue != null)
                ((TestingViewModel) args.OldValue).PropertyChanged -= ViewModelOnPropertyChanged;
            if (ViewModel != null)
                ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(TestingViewModel.StateHandler))
            {
                if (ViewModel?.StateHandler?.DataSetMetaData?.ClassLabels != null)
                {
                    GenerateConfusionMatrix();
                }
            }
        }

        public void GenerateConfusionMatrix()
        {
            ConfusionMatrix.RowDefinitions.Add(new RowDefinition());
            ConfusionMatrix.ColumnDefinitions.Add(new ColumnDefinition());
            var labels = ViewModel.StateHandler.DataSetMetaData.ClassLabels.ToArray();
            InitConfMatrix(labels.Length + 1);
            this.SetTextBlock(0,0, "classes");
            for (int i = 0; i < labels.Length; i++)
            {
                this.SetTextBlock(0, i + 1, labels[i]);
                this.SetTextBlock(i + 1, 0, labels[i]);
            }
            var data = new int[labels.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new int[labels.Length];
                for (int j = 0; j < data.Length; j++)
                {
                    data[i][j] = 0;
                }
            }
            SetDataFromMatrix(data);
        }

        public void SetDataFromMatrix(int[][] paData)
        {
            for (int i = 0; i < paData.Length; i++)
            {
                for (int j = 0; j < paData[i].Length; j++)
                {
                    this.SetTextBlock(i + 1, j + 1, paData[i][j].ToString());
                }
            }
            this.UnifyRowsAndColumns();
        }

        public void InitConfMatrix(int paSize)
        {
            ConfusionMatrix.Children.Clear();
            ConfusionMatrix.ColumnDefinitions.Clear();
            ConfusionMatrix.RowDefinitions.Clear();
            var offsetMargin = new Thickness(5);
            for (int i = 0; i < paSize; i++)
            {
                for (int j = 0; j < paSize; j++)
                {
                    ConfusionMatrix.ColumnDefinitions.Add(new ColumnDefinition());
                    ConfusionMatrix.RowDefinitions.Add(new RowDefinition());

                    var grid = new Grid();
                    grid.Children.Add(new Border() { BorderThickness = new Thickness(0.5), BorderBrush = Brushes.DarkGray });
                    var tbx = new TextBlock()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = offsetMargin
                    };
                    grid.Children.Add(tbx);

                    if (i == 0 || j == 0)
                        tbx.Style = (Style)Application.Current.Resources["Headline"];
                    if (i == j && i != 0)
                    {
                        grid.Background = Brushes.DarkGray;
                        tbx.Foreground = Brushes.White;
                    }
                    grid.SetValue(Grid.RowProperty, i);
                    grid.SetValue(Grid.ColumnProperty, j);
                    ConfusionMatrix.Children.Add(grid);
                }
            }
        }

        public void UnifyRowsAndColumns()
        {
            var heigth = ConfusionMatrix.Children
                .Cast<Grid>().Max(r => r.ActualHeight);
            var width = ConfusionMatrix.Children
                .Cast<Grid>().Max(c => c.ActualWidth);
            ConfusionMatrix.RowDefinitions.ForEach(r=> r.MinHeight = heigth).ExecuteQuery();
            ConfusionMatrix.ColumnDefinitions.ForEach(c => c.MinWidth = width).ExecuteQuery();
        }

        public void SetTextBlock(int paI, int paJ, string paValue)
        {
            var tbx = this.GetTextBlock(paI, paJ);
            if (tbx == null)
                return;
            tbx.Text = paValue;
        }

        public TextBlock GetTextBlock(int paI, int paJ)
        {
            return ConfusionMatrix.Children
                .Cast<Grid>()
                .FirstOrDefault(e => Grid.GetRow(e) == paI && Grid.GetColumn(e) == paJ)?.Children.OfType<TextBlock>().LastOrDefault();
        }

        private async void OnTestNetwork(object sender, RoutedEventArgs e)
        {
            var matrix = await ViewModel.GenerateConfusionMatrix();
            if(matrix != null)
                this.SetDataFromMatrix(matrix);
        }
    }
}
