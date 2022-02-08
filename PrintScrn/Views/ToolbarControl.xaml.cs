using System.Windows;
using System.Windows.Controls;

namespace PrintScrn.Views;

public partial class ToolbarControl : UserControl
{
    public ToolbarControl()
    {
        InitializeComponent();
    }

    private void OnClickShutdownApp(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown(0);
    }
}
