using System;

namespace PrintScrn
{
    internal static class MainProcess
    {
        [STAThread]
        public static void Main()
        {
            App app = new();
            app.InitializeComponent();
            app.Run();
        }
    }
}
