using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using PrintScrn.Infrastructure;

namespace PrintScrn.Views;

public partial class PrintScrnWindow : Window
{
    public PrintScrnWindow()
    {
        InitializeComponent();
    }

    private void OpenLogs(object sender, MouseButtonEventArgs e)
    {
        if (App.LogsLocation == string.Empty)
        {
            return;
        }

        try
        {
            FileLogger.Close();
            Process.Start(new ProcessStartInfo(App.LogsLocation) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            FileLogger.LogError(ex.Message);
            MessageBox.Show(ex.Message);
        }
        finally
        {
            FileLogger.Reopen();
        }
    }
}
