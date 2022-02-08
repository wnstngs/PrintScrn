using System;

namespace PrintScrn.Native;

public static partial class Win32Fn
{
    public static bool GetMonitorInfoSafe(IntPtr hmonitor, ref Win32Type.MONITORINFOEX lpmi)
    {
        return GetMonitorInfo(hmonitor, ref lpmi);
    }

    public static IntPtr MonitorFromWindowSafe(IntPtr hwnd, uint dwflags)
    {
        return MonitorFromWindow(hwnd, dwflags);
    }

    public static IntPtr GetDesktopWindowSafe()
    {
        return GetDesktopWindow();
    }

    public static IntPtr GetWindowDcSafe(IntPtr hWnd)
    {
        return GetWindowDC(hWnd);
    }

    public static IntPtr CreateCompatibleDcSafe(IntPtr hdc)
    {
        return CreateCompatibleDC(hdc);
    }

    public static IntPtr CreateCompatibleBitmapSafe(IntPtr hdc, int nWidth, int nHeight)
    {
        return CreateCompatibleBitmap(hdc, nWidth, nHeight);
    }

    public static IntPtr SelectObjectSafe(IntPtr hdc, IntPtr hgdiobj)
    {
        return SelectObject(hdc, hgdiobj);
    }

    public static bool BitBltSafe(
        IntPtr hdc,
        int nXDest,
        int nYDest,
        int nWidth,
        int nHeight,
        IntPtr hdcSrc,
        int nXSrc,
        int nYSrc,
        Win32Type.TernaryRasterOperations dwRop
    )
    {
        return BitBlt(hdc, nXDest, nYDest, nWidth, nHeight, hdcSrc, nXSrc, nYSrc, dwRop);
    }

    public static bool ReleaseDcSafe(IntPtr hWnd, IntPtr hdc)
    {
        return ReleaseDC(hWnd, hdc);
    }

    public static bool DeleteDcSafe(IntPtr hdc)
    {
        return DeleteDC(hdc);
    }

    public static bool DeleteObjectSafe(IntPtr hObject)
    {
        return DeleteObject(hObject);
    }

    public static bool GetClientRectSafe(IntPtr hWnd, out Win32Type.RECT lpRect)
    {
        return GetClientRect(hWnd, out lpRect);
    }

    public static bool ClipCursorSafe(ref Win32Type.RECT lpRect)
    {
        return ClipCursor(ref lpRect);
    }
}