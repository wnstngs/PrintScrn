using PrintScrn.Commands;
using PrintScrn.Extensions;
using System.Windows;
using System.Windows.Input;

namespace PrintScrn.ViewModels;

public class ToolbarViewModel : BaseViewModel
{
    public ToolbarViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Add(this);

        FullscreenCaptureCmd = new RelayCommand(
            OnExecuted_FullscreenCaptureCmd,
            CanExecute_FullscreenCaptureCmd
        );
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

    #region CustomRectangleCaptureMode

    private bool _customRectangleCaptureMode = true;

    public bool CustomRectangleCaptureMode
    {
        get => _customRectangleCaptureMode;
        set => Set(ref _customRectangleCaptureMode, value);
    }

    #endregion

    #region WindowRectangleCaptureMode

    private bool _windowRectangleCaptureMode = false;

    public bool WindowRectangleCaptureMode
    {
        get => _windowRectangleCaptureMode;
        set => Set(ref _windowRectangleCaptureMode, value);
    }

    #endregion

    #endregion

    #region Commands

    #region FullscreenCaptureCmd

    public ICommand FullscreenCaptureCmd { get; }

    private static bool CanExecute_FullscreenCaptureCmd(object p)
    {
        return true;
    }

    private void OnExecuted_FullscreenCaptureCmd(object p)
    {
        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        screenshotCanvasViewModel?.ScreenshotFullscreenCmd.Execute(null);
    }

    #endregion

    #endregion
}
