using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PrintScrn.Models;

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

    public static Bitmap? Crop(this Bitmap? bmp, RectangleCaptureArea rectangle)
    {
        if (
            bmp != null &&
            rectangle.X >= 0 &&
            rectangle.Y >= 0 &&
            rectangle.Width > 0 &&
            rectangle.Height > 0
        )
        {
            return bmp.Clone(
                new Rectangle((int) rectangle.X, (int) rectangle.Y, (int) rectangle.Width, (int) rectangle.Height),
                bmp.PixelFormat
            );
        }

        return null;
    }
}
