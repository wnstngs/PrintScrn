using PrintScrn.Models;

namespace PrintScrn.Services.Interfaces;

interface IGraphicsCapture
{
    Screenshot SnapshotFullscreen();

    Screenshot SnapshotCustomRectangle(RectangleCaptureArea rectangle);
}
