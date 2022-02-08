using System.Windows.Media.Imaging;

namespace PrintScrn.Models
{
    public class RectangleCaptureArea
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public BitmapImage? Image { get; set; }
    }
}
