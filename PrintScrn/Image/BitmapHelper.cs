using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrintScrn.Image
{
    public static class BitmapHelper
    {
        public static BitmapImage? ToBitmapImage(this Bitmap? bmp)
        {
            MemoryStream memstream = new();
            bmp?.Save(memstream, System.Drawing.Imaging.ImageFormat.Bmp);

            BitmapImage? bitmapImage = new();
            bitmapImage.BeginInit();
            memstream.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = memstream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public static BitmapSource? ToBitmapSource(this Bitmap? bmp)
        {
            if (bmp == null)
            {
                return null;
            }

            var bitmapData = bmp.LockBits(
                new System.Drawing.Rectangle(
                    0,
                    0,
                    bmp.Width,
                    bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bmp.PixelFormat
            );

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width,
                bitmapData.Height,
                bmp.HorizontalResolution,
                bmp.VerticalResolution,
                PixelFormats.Bgr24,
                null,
                bitmapData.Scan0,
                bitmapData.Stride * bitmapData.Height,
                bitmapData.Stride
            );

            bmp.UnlockBits(bitmapData);

            return bitmapSource;
        }
    }
}
