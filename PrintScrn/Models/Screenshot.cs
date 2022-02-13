using System.Drawing;
using System.Windows.Media.Imaging;

namespace PrintScrn.Models;

public class Screenshot
{
    public Bitmap? Bitmap { get; set; }

    public BitmapSource? BitmapSource { get; set; }

    public BitmapImage? BitmapImage { get; set; }
}
