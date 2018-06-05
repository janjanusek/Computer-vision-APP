using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AiSdk.Configuration.LayerModels;
using ImageClassifierApp.Models.NetModel;
using ImageClassifierApp.Objects.Extensions;

namespace ImageClassifierApp.Controls.LayerModelFormular
{
    /// <summary>
    /// Interaction logic for LayerModelFormularUserControl.xaml
    /// </summary>
    public partial class LayerModelFormularUserControl : UserControl
    {
        public ModelLayerBase ViewModel => ((LayerInfo)this.DataContext)?.Layer;

        public LayerModelFormularUserControl()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        public bool IsReadOnly { get; set; }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            FormGrid.Children.Clear();
            if (ViewModel == null)
                return;
            IsReadOnly = (dependencyPropertyChangedEventArgs.NewValue as LayerInfo)?.IsReadOnly ?? false;
            FormGrid.Children.Clear();
            FormGrid.RowDefinitions.Clear();
            RenderProps(ViewModel.GetType().GetProperties());
        }

        private void RenderProps(IEnumerable<PropertyInfo> paProps)
        {
            var row = 0;
            foreach (var propertyInfo in paProps)
            {
                if (propertyInfo.CanWrite == false)
                    continue;
                AddNewLineToGrid();
                var type = propertyInfo.PropertyType;
                var label = GetLabel(propertyInfo);
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, row);
                FormGrid.Children.Add(label);
                UIElement element =
                    type.IsEnum ? (UIElement)GetComboBox(propertyInfo, ViewModel) : GetInput(propertyInfo, ViewModel);
                Grid.SetColumn(element, 1);
                Grid.SetRow(element, row);
                FormGrid.Children.Add(element);
                row++;
            }
        }

        private void AddNewLineToGrid()
        {
            var line = new RowDefinition
            {
                Height = GridLength.Auto
            };
            FormGrid.RowDefinitions.Add(line);
        }

        private ComboBox GetComboBox(PropertyInfo paPropertyInfo, object paViewModel)
        {
            var valuesList = new List<object>();
            var enumerator = Enum.GetValues(paPropertyInfo.PropertyType).GetEnumerator();
            while (enumerator.MoveNext())
            {
                valuesList.Add(enumerator.Current);
            }
            var selectedValue = paPropertyInfo.GetValue(paViewModel) ?? valuesList.FirstOrDefault();
            var selectedIndex = valuesList.IndexOf(selectedValue);
            var values = Enum.GetNames(paPropertyInfo.PropertyType).ToArray();
            var names = values.Select(n => ((Enum)Enum.Parse(paPropertyInfo.PropertyType, n)).ToString().InsertSpaceBeforeUpperCase().ToLower()).ToArray();
            var combo = new ComboBox
            {
                ItemsSource = names,
                Margin = new Thickness(5),
                SelectedIndex = selectedIndex
            };

            if (IsReadOnly)
            {
                combo.SelectionChanged += (sender, args) =>
                {
                    if (combo.SelectedIndex != selectedIndex)
                        combo.SelectedIndex = selectedIndex;
                };
            }
            else
            {
                combo.SelectionChanged += (sender, args) =>
                {
                    paPropertyInfo.SetValue(paViewModel, valuesList[combo.SelectedIndex]);
                };
            }
            return combo;
        }

        private TextBlock GetLabel(PropertyInfo paPropertyInfo)
        {
            return new TextBlock
            {
                Text = paPropertyInfo.Name.InsertSpaceBeforeUpperCase().ToLower(),
                Margin = new Thickness(0, 5, 0, 5)
            };
        }

        private TextBox GetInput(PropertyInfo paPropertyInfo, object paViewModel)
        {
            var textBox = new TextBox
            {
                IsReadOnly = IsReadOnly || paPropertyInfo.CanWrite == false,
                Margin = new Thickness(5),
            };
            var binding = new Binding()
            {
                Mode = paPropertyInfo.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                Path = new PropertyPath(paPropertyInfo.Name),
                Source = paViewModel
            };
            textBox.SetBinding(TextBox.TextProperty, binding);
            return textBox;
        }
    }
}
