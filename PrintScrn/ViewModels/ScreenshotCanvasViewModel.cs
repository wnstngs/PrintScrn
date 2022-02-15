using System;
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

public class ScreenshotCanvasViewModel : BaseViewModel
{
    private const int MinSelectedRectSize = 10;

    private readonly IGraphicsCapture _graphicsCaptureService;

    private Screenshot? _fullscreenScreenshot;

    private Screenshot? _customRectangleScreenshot;

    public ScreenshotCanvasViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Add(this);

        _graphicsCaptureService = new GraphicsCaptureService();

        _customRectangleScreenshot = new Screenshot();

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

    #region SelectedRectImageSource

    private ImageSource? _selectedRectImageSource;

    public ImageSource? SelectedRectImageSource
    {
        get => _selectedRectImageSource;
        set
        {
            if (_customRectangleScreenshot == null || _fullscreenScreenshot == null)
            {
                return;
            }

            if (_selectedRectXPositionScreenCoords < 0 || _selectedRectYPositionScreenCoords < 0)
            {
                return;
            }

            _customRectangleScreenshot.Bitmap = _fullscreenScreenshot.Bitmap.Crop(
                new(
                    (int)Math.Round(_selectedRectXPositionScreenCoords),
                    (int)Math.Round(_selectedRectYPositionScreenCoords),
                    (int)Math.Round(_selectedRectWidthScreenCoords),
                    (int)Math.Round(_selectedRectHeightScreenCoords)
                )
            );

            if (_customRectangleScreenshot.Bitmap == null)
            {
                return;
            }

            Set(ref _selectedRectImageSource, _customRectangleScreenshot.Bitmap.ToBitmapImage());
        }
    }

    #endregion

    #region SelectedRectXPosition

    private double _selectedRectXPosition;

    public double SelectedRectXPosition
    {
        get => _selectedRectXPosition;
        set => Set(ref _selectedRectXPosition, value);
    }

    #endregion

    #region SelectedRectYPosition

    private double _selectedRectYPosition;

    public double SelectedRectYPosition
    {
        get => _selectedRectYPosition;
        set => Set(ref _selectedRectYPosition, value);
    }

    #endregion

    #region SelectedRectWidth

    private double _selectedRectWidth;

    public double SelectedRectWidth
    {
        get => _selectedRectWidth;
        set => Set(ref _selectedRectWidth, value);
    }

    #endregion

    #region SelectedRectHeight

    private double _selectedRectHeight;

    public double SelectedRectHeight
    {
        get => _selectedRectHeight;
        set => Set(ref _selectedRectHeight, value);
    }

    #endregion

    #region SelectedRectXPositionScreenCoords

    private double _selectedRectXPositionScreenCoords;

    public double SelectedRectXPositionScreenCoords
    {
        get => _selectedRectXPositionScreenCoords;
        set => Set(ref _selectedRectXPositionScreenCoords, value);
    }

    #endregion

    #region SelectedRectYPositionScreenCoords

    private double _selectedRectYPositionScreenCoords;

    public double SelectedRectYPositionScreenCoords
    {
        get => _selectedRectYPositionScreenCoords;
        set => Set(ref _selectedRectYPositionScreenCoords, value);
    }

    #endregion

    #region SelectedRectWidthScreenCoords

    private double _selectedRectWidthScreenCoords;

    public double SelectedRectWidthScreenCoords
    {
        get => _selectedRectWidthScreenCoords;
        set => Set(ref _selectedRectWidthScreenCoords, value);
    }

    #endregion

    #region SelectedRectHeightScreenCoords

    private double _selectedRectHeightScreenCoords;

    public double SelectedRectHeightScreenCoords
    {
        get => _selectedRectHeightScreenCoords;
        set => Set(ref _selectedRectHeightScreenCoords, value);
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
        if (SelectedRectHeight < MinSelectedRectSize || SelectedRectWidth < MinSelectedRectSize)
        {
            return;
        }

        if (_customRectangleScreenshot != null)
        {
            _customRectangleScreenshot.BitmapSource = _customRectangleScreenshot.Bitmap.ToBitmapSource();
            if (_customRectangleScreenshot?.BitmapSource == null)
            {
                FileLogger.LogError("BitmapSource is null.");
                return;
            }
            Clipboard.SetImage(_customRectangleScreenshot.BitmapSource);
        }
        else
        {
            FileLogger.LogError("_customRectangleScreenshot is null.");
        }

        Application.Current.Shutdown(0);
    }

    #endregion

    #endregion
}
