﻿using System.Drawing;
using System.Drawing.Drawing2D;
using PrintScrn.Infrastructure.Extensions;
using PrintScrn.Infrastructure.Helpers;
using PrintScrn.Models;
using PrintScrn.Services.Interfaces;

namespace PrintScrn.Services;

public class GraphicsCaptureService : IGraphicsCapture
{
    public Screenshot? CaptureFullscreen()
    {
        var monitorRect = GraphicsCaptureHelper.GetMonitorRectFromWindow();
        var bitmap = CaptureBitmapFromScreen(
            new RectangleCaptureArea
            {
                X = monitorRect.X,
                Y = monitorRect.Y,
                Width = monitorRect.Width,
                Height = monitorRect.Height
            }
        );
        return new Screenshot
        {
            Bitmap = bitmap,
            BitmapSource = bitmap.ToBitmapSource(),
            BitmapImage = bitmap.ToBitmapImage()
        };
    }

    public Screenshot CaptureCustomRectangle(RectangleCaptureArea rectangle)
    {
        var bitmap = CaptureBitmapFromScreen(rectangle);
        return new Screenshot
        {
            Bitmap = bitmap,
            BitmapSource = bitmap.ToBitmapSource(),
            BitmapImage = bitmap.ToBitmapImage()
        };
    }

    private Bitmap? CaptureBitmapFromScreen(RectangleCaptureArea rectangle)
    {
        Bitmap? bitmap = new((int) rectangle.Width, (int) rectangle.Height);
        var gfx = Graphics.FromImage(bitmap);
        gfx.CompositingQuality = CompositingQuality.HighQuality;
        gfx.SmoothingMode = SmoothingMode.HighQuality;
        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
        gfx.CopyFromScreen((int) rectangle.X, (int) rectangle.Y, 0, 0, bitmap.Size);
        return bitmap;
    }
}
