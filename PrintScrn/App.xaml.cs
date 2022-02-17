using System.Windows;

namespace PrintScrn;

public partial class App : Application
{
    public static string LogsLocation { get; set; } = new(string.Empty);
}
