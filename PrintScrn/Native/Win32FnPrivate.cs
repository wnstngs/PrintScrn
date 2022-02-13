using System;
using System.Runtime.InteropServices;

namespace PrintScrn.Native;

public static partial class Win32Fn
{

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hmonitor, ref Win32Type.MONITORINFOEX lpmi);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwflags);

    [DllImport("user32.dll", SetLastError = false)]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
    private static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
    private static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
    private static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

    [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool BitBlt(
        [In] IntPtr hdc,
        int nXDest,
        int nYDest,
        int nWidth,
        int nHeight,
        [In] IntPtr hdcSrc,
        int nXSrc,
        int nYSrc,
        Win32Type.TernaryRasterOperations dwRop
    );

    [DllImport("user32.dll")]
    private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

    [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
    private static extern bool DeleteDC([In] IntPtr hdc);

    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeleteObject([In] IntPtr hObject);

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hWnd, out Win32Type.RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool ClipCursor(ref Win32Type.RECT lpRect);
}
