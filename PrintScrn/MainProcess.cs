using System;
using PrintScrn.Infrastructure;

namespace PrintScrn;

internal static class MainProcess
{
    private static string LogsPath = Environment.CurrentDirectory + "\\Log.txt";

    [STAThread]
    public static void Main()
    {
        App app = new();
        FileLogger.Init(LogsPath);
        App.LogsLocation = LogsPath;
        app.InitializeComponent();
        app.Run();
        FileLogger.Close();
    }
}
