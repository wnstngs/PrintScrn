using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PrintScrn.Infrastructure;
using PrintScrn.Infrastructure.Command;
using PrintScrn.Infrastructure.Extensions;
using PrintScrn.Models;
using PrintScrn.Services;
using PrintScrn.Services.Interfaces;

namespace PrintScrn.ViewModels;

public class ScreenshotCanvasViewModel : Bindable
{
    private const int MinSelectedRectSize = 10;

    private readonly IGraphicsCapture _graphicsCaptureService;

    private Screenshot? _fullscreenScreenshot;

    public ScreenshotCanvasViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Add(this);

        _graphicsCaptureService = new GraphicsCaptureService();

        CanvasInitialize = new RelayCommand(OnCanvasInitialize);
        CaptureCustomRectangle = new RelayCommand(OnCaptureCustomRectangle);
        CaptureFullscreen = new RelayCommand(OnCaptureFullscreen);
    }

    ~ScreenshotCanvasViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Remove(this);
    }

    #region Properties

    #region ScreenImageSource

    private ImageSource? _screenshotCanvasImageSource;

    public ImageSource? ScreenshotCanvasImageSource
    {
        get => _screenshotCanvasImageSource;
        set => Set(ref _screenshotCanvasImageSource, value);
    }

    #endregion

    #region CustomSelectedRectangle

    private RectangleCaptureArea? _customSelectedRectangle;

    public RectangleCaptureArea? CustomSelectedRectangle
    {
        get => _customSelectedRectangle;
        set => Set(ref _customSelectedRectangle, value);
    }

    #endregion

    #region CustomSelectedRectangleScreenCoordinates

    private RectangleCaptureArea? _customSelectedRectangleScreenCoordinates;

    public RectangleCaptureArea? CustomSelectedRectangleScreenCoordinates
    {
        get => _customSelectedRectangleScreenCoordinates;
        set => Set(ref _customSelectedRectangleScreenCoordinates, value);
    }

    #endregion

    #endregion

    #region Commands

    #region CanvasInitialize

    public ICommand CanvasInitialize { get; }

    private void OnCanvasInitialize()
    {
        var windowViewModel = ViewModelsExtension.FindViewModel<PrintScrnWindowViewModel>();
        if (windowViewModel != null)
        {
            windowViewModel.WindowOpacity = 0.0;
        }
        else
        {
            FileLogger.LogWarning("windowViewModel is null.");
        }

        _fullscreenScreenshot = _graphicsCaptureService.CaptureFullscreen();

        if (_fullscreenScreenshot?.BitmapImage != null)
        {
            ScreenshotCanvasImageSource = _fullscreenScreenshot.BitmapImage;
        }
        else
        {
            FileLogger.LogError("BitmapImage is null.");
        }

        if (windowViewModel != null)
        {
            windowViewModel.WindowOpacity = 1.0;
            windowViewModel.ShowInTaskbar = true;
        }
    }

    #endregion

    #region CaptureFullscreen

    public ICommand CaptureFullscreen { get; }

    private void OnCaptureFullscreen()
    {
        if (_fullscreenScreenshot?.BitmapSource == null)
        {
            FileLogger.LogError("BitmapSource is null.");
            return;
        }
        Clipboard.SetImage(_fullscreenScreenshot.BitmapSource);
        Application.Current.Shutdown(0);
    }

    #endregion

    #region CaptureCustomRectangle

    public ICommand CaptureCustomRectangle { get; }

    private void OnCaptureCustomRectangle()
    {
        if (CustomSelectedRectangle == null)
        {
            FileLogger.LogError("CustomSelectedRectangle is null.");
            return;
        }

        if (CustomSelectedRectangle.Height < MinSelectedRectSize || 
            CustomSelectedRectangle.Width < MinSelectedRectSize)
        {
            return;
        }

        if (CustomSelectedRectangleScreenCoordinates != null)
        {
            var croppedBitmap = _fullscreenScreenshot?.Bitmap.Crop(CustomSelectedRectangleScreenCoordinates);
            var croppedBitmapSource = croppedBitmap.ToBitmapSource();
            if (croppedBitmapSource != null)
            {
                croppedBitmap?.Save("screenshot.png", ImageFormat.Png);
                Clipboard.SetImage(croppedBitmapSource);
                Application.Current.Shutdown(0);
            }
            else
            {
                FileLogger.LogError("croppedBitmapSource is null.");
            }
        }
        else
        {
            FileLogger.LogError("CustomSelectedRectangleScreenCoordinates is null.");
        }
    }

    #endregion

    #endregion
}
