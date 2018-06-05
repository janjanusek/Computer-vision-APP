using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageClassifierApp.Models.Classification;
using ImageClassifierApp.Objects.Stream;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Size = System.Drawing.Size;

namespace ImageClassifierApp.Services.Image
{
    /// <summary>
    /// Thread safe class for loading raw pixels from image
    /// </summary>
    public class ImageProcessor
    {
        private readonly bool[] _channels;
        private readonly byte[] _bytePixels;
        private readonly byte[] _bytePixelsClean;
        private readonly bool _grayScale;
        private readonly Size _size;
        private static readonly object Sync = new object();


        public ImageProcessor(DataSetModel paModel)
        {
            var metaData = paModel.MetaData;
            _bytePixels = new byte[metaData.Deep * metaData.Height * metaData.Width];
            _bytePixelsClean = new byte[_bytePixels.Length];
            _grayScale = metaData.GrayScale;
            _size = new Size(metaData.Width, metaData.Height);
            _channels = new[] { metaData.ChannelA, metaData.ChannelR, metaData.ChannelG, metaData.ChannelB };
        }

        public byte[] ToPixels(string paPath)
        {
            byte[] pixels;
            using (var stream = File.OpenRead(paPath))
            {
                using (var wrapper = new WrappingStream(stream))
                {
                    using (var bmp = (Bitmap)System.Drawing.Image.FromStream(wrapper))
                    {
                        Bitmap resized = null;
                        if (bmp.Width != _size.Width || bmp.Height != _size.Height)
                        {
                            resized = (Bitmap)FixedSize(bmp, _size.Width, _size.Height);
                        }
                        else
                            resized = bmp;

                        using (resized)
                        {
                            lock (Sync)
                            {
                                pixels = ReadBytePixels(resized);
                            }
                        }
                    }
                }
            }
            Array.Copy(_bytePixelsClean, _bytePixels, _bytePixels.Length);
            var index = 0;
            if (!_grayScale)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    if (_channels[i % 4])
                    {
                        _bytePixels[index++] = pixels[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < pixels.Length; i += 4)
                {
                    _bytePixels[index++] = (byte)((pixels[i] + pixels[i + 1] + pixels[i + 2] + pixels[i + 3]) / 4.0);
                }
            }
            return _bytePixels;
        }

        public System.Drawing.Image FixedSize(System.Drawing.Image imgPhoto, int paWidth, int paHeight)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)paWidth / (float)sourceWidth);
            nPercentH = ((float)paHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((paWidth -
                                                (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((paHeight -
                                                (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            
            var wbm = new WriteableBitmap(paWidth, paHeight, 100, 100, PixelFormats.Bgra32, null);
            Bitmap bmPhoto = null; 
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)wbm));
                enc.Save(outStream);
                bmPhoto = new System.Drawing.Bitmap(outStream);
            }

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.InterpolationMode =
                InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private static byte[] ReadBytePixels(Bitmap paBitmap)
        {
            IntPtr hBitmap = paBitmap.GetHbitmap();
            try
            {
                var source = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                var wbmap = new WriteableBitmap(source);
                return wbmap.ToByteArray();
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }
    }
}
