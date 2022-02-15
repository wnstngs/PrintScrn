using PrintScrn.Models;

namespace PrintScrn.Services.Interfaces;

interface IGraphicsCapture
{
    Screenshot? CaptureFullscreen();

    Screenshot CaptureCustomRectangle(RectangleCaptureArea rectangle);
}
