using System.Drawing;
using PrintScrn.Native;

namespace PrintScrn.Capture
{
    public static class Screenshot
    {
        public static Bitmap? Fullscreen()
        {
            var monitor = CaptureHelper.GetMonitorRectFromWindow();
            return TakeScreenshotInternal(monitor);
        }

        public static Bitmap? Rectangle(Win32Type.RECT rect)
        {
            return TakeScreenshotInternal(rect);
        }

        private static Bitmap? TakeScreenshotInternal(Win32Type.RECT rect)
        {
            var desktopWindow = Win32Fn.GetDesktopWindowSafe();
            Bitmap? bmp = null;

            if (rect.Width == 0 || rect.Height == 0)
            {
                return bmp;
            }

            var hdcSrc = Win32Fn.GetWindowDcSafe(desktopWindow);
            var hdcDest = Win32Fn.CreateCompatibleDcSafe(hdcSrc);
            var hBitmap = Win32Fn.CreateCompatibleBitmapSafe(hdcSrc, rect.Width, rect.Height);
            var hOld = Win32Fn.SelectObjectSafe(hdcDest, hBitmap);

            Win32Fn.BitBltSafe(
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

            Win32Fn.SelectObjectSafe(hdcDest, hOld);
            Win32Fn.DeleteDcSafe(hdcDest);
            Win32Fn.ReleaseDcSafe(desktopWindow, hdcSrc);

            bmp = System.Drawing.Image.FromHbitmap(hBitmap);

            Win32Fn.DeleteObjectSafe(hBitmap);

            return bmp;
        }
    }
}
