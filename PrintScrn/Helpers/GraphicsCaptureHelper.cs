using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using PrintScrn.Native;

namespace PrintScrn.Helpers;

internal static class GraphicsCaptureHelper
{
    public static Win32Type.RECT GetMonitorRectFromWindow()
    {
        var hwnd = new WindowInteropHelper(Application.Current.MainWindow!).Handle;
        if (hwnd == IntPtr.Zero)
        {
            MessageBox.Show(
                "Window was not initialized at the moment of 'GetMonitorRectFromWindow()' call.",
                "Fatal",
                MessageBoxButton.OK, MessageBoxImage.Error
            );
            throw new Win32Exception();
        }

        var hMonitor = Win32Fn.MonitorFromWindowSafe(hwnd, Win32Constant.MONITOR_DEFAULTTOPRIMARY);
        if (hMonitor == IntPtr.Zero)
        {
            throw new Win32Exception();
        }

        Win32Type.MONITORINFOEX monitorInfo = new();
        monitorInfo.Size = Marshal.SizeOf(monitorInfo);
        Win32Fn.GetMonitorInfoSafe(hMonitor, ref monitorInfo);

        var rect = monitorInfo.Monitor;
        Win32Fn.DeleteObjectSafe(hMonitor);

        return rect;
    }
}