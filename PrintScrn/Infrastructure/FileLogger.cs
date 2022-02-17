using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace PrintScrn.Infrastructure;

public static class FileLogger
{
    private static string _path;

    private static StreamWriter? _writer;

    /// <summary>
    /// Initializes an instance of the file logger.
    /// After use must be closed via <see cref="Close"/>.
    /// </summary>
    /// <param name="path">The complete file path to write to. path can be a file name.</param>
    public static void Init(string path)
    {
        _path = path;

        try
        {
            File.Delete(_path);
        }
        catch (Exception e)
        {
            Trace.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name}: {e.Message}");
        }

        try
        {
            _writer = new(_path, append: true);
        }
        catch (Exception e)
        {
            Trace.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name}: {e.Message}");
        }
    }

    /// <summary>
    /// Closes stream of the file logger.
    /// </summary>
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

    public static void Reopen()
    {
        try
        {
            _writer = new(_path, append: true);
        }
        catch (Exception e)
        {
            Trace.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name}: {e.Message}");
        }
    }

    /// <summary>
    /// Logs informational message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="line">-</param>
    /// <param name="caller">-</param>
    public static void LogInfo(
        string message,
        [CallerLineNumber] int line = -1,
        [CallerMemberName] string caller = "---"
    )
    {
        const string level = "INFO";
        WriteLog($"[{level}] [{caller}:{line}] {message}");
    }

    /// <summary>
    /// Logs warning message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="line">-</param>
    /// <param name="caller">-</param>
    public static void LogWarning(
        string message,
        [CallerLineNumber] int line = -1,
        [CallerMemberName] string caller = "---"
    )
    {
        const string level = "WARN";
        WriteLog($"[{level}] [{caller}:{line}] {message}");
    }

    /// <summary>
    /// Logs error message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="line">-</param>
    /// <param name="caller">-</param>
    public static void LogError(
        string message,
        [CallerLineNumber] int line = -1,
        [CallerMemberName] string caller = "---"
    )
    {
        const string level = "ERROR";
        WriteLog($"[{level}] [{caller}:{line}] {message}");
    }

    /// <summary>
    /// Logs critical error message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="line">-</param>
    /// <param name="caller">-</param>
    public static void LogFatal(
        string message,
        [CallerLineNumber] int line = -1,
        [CallerMemberName] string caller = "---"
    )
    {
        const string level = "FATAL";
        WriteLog($"[{level}] [{caller}:{line}] {message}");
    }

    /// <summary>
    /// Writes a string to the log stream.
    /// </summary>
    /// <param name="message">Message to log.</param>
    private static async void WriteLog(string message)
    {
        if (_writer == null)
        {
            return;
        }

        try
        {
            await _writer.WriteLineAsync(message);
        }
        catch (Exception e)
        {
            Trace.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name}: {e.Message}");
        }
    }
}
