using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Services.Image;

namespace ImageClassifierApp.Converters
{
    public class ImageResizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(v => v == null))
                return null;
            var viewModel = (DataSetModel)values[0];
            var path = values[1].ToString();

            var width = viewModel.MetaData.Width;
            var height = viewModel.MetaData.Height;
            if (File.Exists(path) == false)
            {
                return null;
            }
            var bitmap = System.Drawing.Image.FromFile(path);

            var bitmap2 = new ImageProcessor(new DataSetModel() { MetaData = new DataSetMetaData() { } }).FixedSize(bitmap, width, height);
            bitmap.Dispose();
            return CreateBitmapSourceFromBitmap((Bitmap)bitmap2);
        }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
    {
        if (bitmap == null)
            throw new ArgumentNullException("bitmap");

        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            bitmap.GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
    }
}
}
