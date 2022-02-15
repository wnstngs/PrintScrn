using System.Windows;
using System.Windows.Input;
using PrintScrn.Infrastructure.Command;
using PrintScrn.Infrastructure.Extensions;

namespace PrintScrn.ViewModels;

public class ToolbarViewModel : Bindable
{
    public ToolbarViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Add(this);

        DelegateCaptureFullscreen = new RelayCommand(OnDelegateCaptureFullscreen);
    }

    ~ToolbarViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Remove(this);
    }

    #region Properties

    #region ToolbarVisibility

    private Visibility _toolbarVisibility = Visibility.Visible;

    public Visibility ToolbarVisibility
    {
        get => _toolbarVisibility;
        set => Set(ref _toolbarVisibility, value);
    }

    #endregion

    #region IsCustomRectangleCaptureMode

    private bool _isCustomRectangleCaptureMode = true;

    public bool IsCustomRectangleCaptureMode
    {
        get => _isCustomRectangleCaptureMode;
        set => Set(ref _isCustomRectangleCaptureMode, value);
    }

    #endregion

    #region IsWindowRectangleCaptureMode

    private bool _isIsWindowRectangleCaptureMode = false;

    public bool IsWindowRectangleCaptureMode
    {
        get => _isIsWindowRectangleCaptureMode;
        set => Set(ref _isIsWindowRectangleCaptureMode, value);
    }

    #endregion

    #region CaptureOrRecordButtonLabel

    private string _captureOrRecordButtonLabel = "Capture";

    public string CaptureOrRecordButtonLabel
    {
        get => _captureOrRecordButtonLabel;
        set => Set(ref _captureOrRecordButtonLabel, value);
    }

    #endregion

    #endregion

    #region Commands

    #region DelegateCaptureFullscreen

    public ICommand DelegateCaptureFullscreen { get; }

    private void OnDelegateCaptureFullscreen()
    {
        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        screenshotCanvasViewModel?.CaptureFullscreen.Execute(null);
    }

    #endregion

    #endregion
}
