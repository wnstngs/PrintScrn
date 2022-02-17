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

    #region CustomRectangle

    private RectangleCaptureArea? _customRectangle;

    public RectangleCaptureArea? CustomRectangle
    {
        get => _customRectangle;
        set => Set(ref _customRectangle, value);
    }

    #endregion

    #region CustomRectangleScreenCoordinates

    private RectangleCaptureArea? _customRectangleScreenCoordinates;

    public RectangleCaptureArea? CustomRectangleScreenCoordinates
    {
        get => _customRectangleScreenCoordinates;
        set => Set(ref _customRectangleScreenCoordinates, value);
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
        if (CustomRectangle == null)
        {
            FileLogger.LogError("CustomSelectedRectangle is null.");
            return;
        }

        if (CustomRectangle.Height < MinSelectedRectSize || 
            CustomRectangle.Width < MinSelectedRectSize)
        {
            return;
        }

        if (CustomRectangleScreenCoordinates != null)
        {
            var croppedBitmap = _fullscreenScreenshot?.Bitmap.Crop(CustomRectangleScreenCoordinates);
            var croppedBitmapSource = croppedBitmap.ToBitmapSource();
            if (croppedBitmapSource != null)
            {
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
