using PrintScrn.Commands;
using PrintScrn.Extensions;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PrintScrn.Models;
using PrintScrn.Services;
using PrintScrn.Services.Interfaces;

namespace PrintScrn.ViewModels;

public class ScreenshotCanvasViewModel : BaseViewModel
{
    private const int MinSelectedRectSize = 15;

    private readonly IGraphicsCapture _graphicsCaptureService;

    private Screenshot? _fullscreenScreenshot;

    private Screenshot? _customRectangleScreenshot;

    public ScreenshotCanvasViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Add(this);

        _graphicsCaptureService = new GraphicsCaptureService();

        OnInitCmd = new RelayCommand(
            OnExecuted_OnInitCmd,
            CanExecute_OnInitCmd
        );
        SnapshotCustomRectangle = new RelayCommand(
            OnSnapshotCustomRectangle,
            CanSnapshotCustomRectangle
        );
        SnapshotFullscreen = new RelayCommand(
            OnSnapshotFullscreen,
            CanSnapshotFullscreen
        );
    }

    ~ScreenshotCanvasViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Remove(this);
    }

    #region Properties

    #region MaskRectBackground

    private System.Windows.Media.Brush _maskRectBackground = new SolidColorBrush(Colors.Black);

    public System.Windows.Media.Brush MaskRectBackground
    {
        get => _maskRectBackground;
        set => Set(ref _maskRectBackground, value);
    }

    #endregion

    #region MaskRectOpacity

    private double _maskRectOpacity = 0.4;

    public double MaskRectOpacity
    {
        get => _maskRectOpacity;
        set => Set(ref _maskRectOpacity, value);
    }

    #endregion

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
            if (_selectedRectWidthScreenCoords <= MinSelectedRectSize ||
                _selectedRectHeightScreenCoords <= MinSelectedRectSize)
            {
                return;
            }

            if (_selectedRectXPositionScreenCoords < 0 || _selectedRectYPositionScreenCoords < 0)
            {
                return;
            }

            if (_customRectangleScreenshot == null || _fullscreenScreenshot == null)
            {
                return;
            }

            _customRectangleScreenshot.Bitmap = _fullscreenScreenshot.Bitmap.Crop(
                new(
                    (int)Math.Floor(_selectedRectXPositionScreenCoords),
                    (int)Math.Floor(_selectedRectYPositionScreenCoords),
                    (int)Math.Floor(_selectedRectWidthScreenCoords),
                    (int)Math.Floor(_selectedRectHeightScreenCoords)
                )
            );

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

    #region OnInitCmd

    public ICommand OnInitCmd { get; }

    private static bool CanExecute_OnInitCmd(object p)
    {
        return true;
    }

    private void OnExecuted_OnInitCmd(object p)
    {
        var windowViewModel = ViewModelsExtension.FindViewModel<PrintScrnWindowViewModel>();
        if (windowViewModel != null) windowViewModel.WindowOpacity = 0.0;

        _fullscreenScreenshot = _graphicsCaptureService.SnapshotFullscreen();

        if (_fullscreenScreenshot?.BitmapImage != null)
        {
            ScreenshotCanvasImageSource = _fullscreenScreenshot.BitmapImage;
        }

        if (windowViewModel != null)
        {
            windowViewModel.WindowOpacity = 1.0;
            windowViewModel.ShowInTaskbar = true;
        }
    }

    #endregion

    #region SnapshotFullscreen

    public ICommand SnapshotFullscreen { get; }

    private static bool CanSnapshotFullscreen(object p)
    {
        return true;
    }

    private void OnSnapshotFullscreen(object p)
    {
        if (_fullscreenScreenshot?.BitmapSource == null)
        {
            return;
        }
        Clipboard.SetImage(_fullscreenScreenshot.BitmapSource);
        Application.Current.Shutdown(0);
    }

    #endregion

    #region SnapshotCustomRectangle

    public ICommand SnapshotCustomRectangle { get; }

    private static bool CanSnapshotCustomRectangle(object p)
    {
        return true;
    }

    private void OnSnapshotCustomRectangle(object p)
    {
        if (_customRectangleScreenshot?.BitmapSource == null)
        {
            return;
        }
        Clipboard.SetImage(_customRectangleScreenshot.BitmapSource);
        Application.Current.Shutdown(0);
    }

    #endregion

    #endregion
}
