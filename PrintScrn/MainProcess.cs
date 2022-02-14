using System;
using PrintScrn.Infrastructure;

namespace PrintScrn;

internal static class MainProcess
{
    [STAThread]
    public static void Main()
    {
        App app = new();
        FileLogger.Init("Log.log");
        app.InitializeComponent();
        app.Run();
        FileLogger.Close();
    }
}
