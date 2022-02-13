using System.Drawing;
using PrintScrn.Helpers;
using PrintScrn.Models;
using PrintScrn.Services.Interfaces;

namespace PrintScrn.Services;

public class GraphicsCaptureService : IGraphicsCapture
{
    public Screenshot SnapshotFullscreen()
    {
        var monitorRect = GraphicsCaptureHelper.GetMonitorRectFromWindow();
        return new Screenshot
        {
            Bitmap = CaptureBitmapFromScreen(
                new RectangleCaptureArea
                {
                    X = monitorRect.X,
                    Y = monitorRect.Y,
                    Width = monitorRect.Width,
                    Height = monitorRect.Height
                }
            )
        };
    }

    public Screenshot SnapshotCustomRectangle(RectangleCaptureArea rectangle)
    {
        return new Screenshot
        {
            Bitmap = CaptureBitmapFromScreen(rectangle)
        }; ;
    }

    private Bitmap? CaptureBitmapFromScreen(RectangleCaptureArea rectangle)
    {
        Bitmap? bitmap = new(rectangle.Width, rectangle.Height);
        var gfx = Graphics.FromImage(bitmap);
        gfx.CopyFromScreen(rectangle.X, rectangle.Y, 0, 0, bitmap.Size);
        return bitmap;
    }
}
