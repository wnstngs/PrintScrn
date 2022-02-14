using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace PrintScrn.Infrastructure;

public static class FileLogger
{
    private static StreamWriter? _writer;

    public static void Init(string path)
    {
        try
        {
            _writer = new(path, append: true);
        }
        catch (Exception e)
        {
            Trace.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name}: {e.Message}");
        }
    }

    public static void Close()
    {
        try
        {
            _writer?.Close();
        }
        catch (Exception e)
        {
            Trace.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name}: {e.Message}");
        }
    }

    public static async void Log(
        string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string caller = ""
    )
    {
        if (_writer == null)
        {
            return;
        }

        var logMessage = $"[{caller}:{line}] {message}";
        try
        {
            await _writer.WriteLineAsync(logMessage);
        }
        catch (Exception e)
        {
            Trace.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name}: {e.Message}");
        }
    }
}
