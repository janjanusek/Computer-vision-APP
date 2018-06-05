using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using AiSdk.Configuration.LayerModels;
using ImageClassifierApp.Models.NetModel;

namespace ImageClassifierApp.Converters
{
    public class NetModelLayerToBrush : IValueConverter
    {
        private static readonly SolidColorBrush ConvReluBrush = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.1 * 0.2), 24, 58, 86));
        private static readonly SolidColorBrush MaxPoolingBrush = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.3 * 0.2), 24, 58, 86));
        private static readonly SolidColorBrush HiddenFcnBrush = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.5 * 0.2), 24, 58, 86));
        private static readonly SolidColorBrush DropoutBrush = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.7 * 0.2), 24, 58, 86));
        private static readonly SolidColorBrush OutputFcnBrush = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.9 * 0.2), 24, 58, 86));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            var layer = (value as LayerInfo)?.Layer ?? (value as NeuralNetModelLayer);
            var conv = layer as NeuralNetConvReluLayerModel;
            var pooling = layer as NeuralNetPoolingLayerModel;
            if (conv != null)
                return ConvReluBrush;
            else if (pooling != null)
                return MaxPoolingBrush;
            else if (layer is FullyConectedLayerModel)
            {
                return (layer as NeuralNetOutputLayerModel) != null ? OutputFcnBrush : HiddenFcnBrush;
            }
            else if (layer is NeuralNetDropoutLayerModel)
            {
                return DropoutBrush;
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
