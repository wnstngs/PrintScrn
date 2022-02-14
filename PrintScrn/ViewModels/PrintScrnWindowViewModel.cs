using System.Windows;
using System.Windows.Input;
using PrintScrn.Command;

namespace PrintScrn.ViewModels;

internal class PrintScrnWindowViewModel : BaseViewModel
{
    public PrintScrnWindowViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Add(this);

        MonitorWidth = 0;
        MonitorHeight = 0;

        InitializeWindow = new RelayCommand<Window>(OnInitializeWindow);
    }

    ~PrintScrnWindowViewModel()
    {
        ViewModels.Instance.ViewModelsStore.Remove(this);
    }

    #region Properties

    #region MonitorWidth

    private int _monitorWidth;

    public int MonitorWidth
    {
        get => _monitorWidth;
        private set => Set(ref _monitorWidth, value);
    }

    #endregion

    #region MonitorHeight

    private int _monitorHeight;

    public int MonitorHeight
    {
        get => _monitorHeight;
        private set => Set(ref _monitorHeight, value);
    }

    #endregion

    #region WindowOpacity

    private double _windowOpacity = 1.0;

    public double WindowOpacity
    {
        get => _windowOpacity;
        set => Set(ref _windowOpacity, value);
    }

    #endregion

    #region ShowInTaskbar

    private bool _showInTaskbar = false;

    public bool ShowInTaskbar
    {
        get => _showInTaskbar;
        set => Set(ref _showInTaskbar, value);
    }

    #endregion

    #endregion

    #region Commands

    #region InitializeWindow

    public ICommand InitializeWindow { get; }
    
    private void OnInitializeWindow(Window parameter)
    {
        var presentationSrc = PresentationSource.FromVisual(parameter);
        if (presentationSrc?.CompositionTarget != null)
        {
            var m = presentationSrc.CompositionTarget.TransformToDevice;
            MonitorWidth = (int)(SystemParameters.PrimaryScreenWidth * m.M11);
            MonitorHeight = (int)(SystemParameters.PrimaryScreenHeight * m.M22);
        }
    }

    #endregion

    #endregion
}
