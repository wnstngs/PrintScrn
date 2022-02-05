using System.Drawing;
using PrintScrn.Native;

namespace PrintScrn.Capture
{
    public static class Snapshot
    {
        public static Bitmap? Fullscreen()
        {
            var monitor = CaptureHelper.GetMonitorRectFromWindow();
            return _Snapshot(monitor);
        }

        public static Bitmap? Rectangle(Win32Type.RECT rect)
        {
            return _Snapshot(rect);
        }

        private static Bitmap? _Snapshot(Win32Type.RECT rect)
        {
            var desktopWindow = Win32Fn.GetDesktopWindow();
            Bitmap? bmp = null;

            if (rect.Width == 0 || rect.Height == 0)
            {
                return bmp;
            }

            var hdcSrc = Win32Fn.GetWindowDC(desktopWindow);
            var hdcDest = Win32Fn.CreateCompatibleDC(hdcSrc);
            var hBitmap = Win32Fn.CreateCompatibleBitmap(hdcSrc, rect.Width, rect.Height);
            var hOld = Win32Fn.SelectObject(hdcDest, hBitmap);

            Win32Fn.BitBlt(
                hdcDest,
                0,
                0,
                rect.Width,
                rect.Height,
                hdcSrc,
                rect.X,
                rect.Y,
                Win32Type.TernaryRasterOperations.SRCCOPY | Win32Type.TernaryRasterOperations.CAPTUREBLT
            );

            Win32Fn.SelectObject(hdcDest, hOld);
            Win32Fn.DeleteDC(hdcDest);
            Win32Fn.ReleaseDC(desktopWindow, hdcSrc);

            bmp = System.Drawing.Image.FromHbitmap(hBitmap);

            Win32Fn.DeleteObject(hBitmap);

            return bmp;
        }
    }
}
