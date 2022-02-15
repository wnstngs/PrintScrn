using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrintScrn.Infrastructure.Extensions;

public static class BitmapExtension
{
    public static BitmapImage? ToBitmapImage(this Bitmap? bmp)
    {
        if (bmp == null)
        {
            FileLogger.LogWarning("'bmp' is null.");
            return null;
        }

        MemoryStream memstream = new();
        bmp.Save(memstream, System.Drawing.Imaging.ImageFormat.Bmp);

        BitmapImage bitmapImage = new();
        bitmapImage.BeginInit();
        memstream.Seek(0, SeekOrigin.Begin);
        bitmapImage.StreamSource = memstream;
        try
        {
            bitmapImage.EndInit();
        }
        catch (Exception e)
        {
            FileLogger.LogError(e.Message);
            return null;
        }

        return bitmapImage;
    }

    public static BitmapSource? ToBitmapSource(this Bitmap? bmp)
    {
        if (bmp == null)
        {
            FileLogger.LogWarning("'bmp' is null.");
            return null;
        }

        var bitmapData = bmp.LockBits(
            new(
                0,
                0,
                bmp.Width,
                bmp.Height
            ),
            System.Drawing.Imaging.ImageLockMode.ReadOnly,
            bmp.PixelFormat
        );

        BitmapSource? bitmapSource = null;

        try
        {
            bitmapSource = BitmapSource.Create(
                bitmapData.Width,
                bitmapData.Height,
                bmp.HorizontalResolution,
                bmp.VerticalResolution,
                PixelFormats.Bgr32,
                null,
                bitmapData.Scan0,
                bitmapData.Stride * bitmapData.Height,
                bitmapData.Stride
            );
        }
        catch (Exception e)
        {
            FileLogger.LogError(e.Message);
            return bitmapSource;
        }
        finally
        {
            bmp.UnlockBits(bitmapData);
        }

        return bitmapSource;
    }

    public static Bitmap? Crop(this Bitmap? bmp, Rectangle rect)
    {
        if (
            bmp != null &&
            rect.X >= 0 &&
            rect.Y >= 0 &&
            rect.Width > 0 &&
            rect.Height > 0 &&
            new Rectangle(0, 0, bmp.Width, bmp.Height).Contains(rect)
        )
        {
            return bmp.Clone(rect, bmp.PixelFormat);
        }

        return null;
    }
}
