using PrintScrn.ViewModels;

namespace PrintScrn.Models;

public class RectangleCaptureArea : Bindable
{
    private double _x;

    public double X
    {
        get => _x;
        set => Set(ref _x, value);
    }

    private double _y;

    public double Y
    {
        get => _y;
        set => Set(ref _y, value);
    }

    private double _width;

    public double Width
    {
        get => _width;
        set => Set(ref _width, value);
    }

    private double _height;

    public double Height
    {
        get => _height;
        set => Set(ref _height, value);
    }
}
