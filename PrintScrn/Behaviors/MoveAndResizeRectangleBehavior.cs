using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using PrintScrn.Infrastructure;
using PrintScrn.Infrastructure.Extensions;
using PrintScrn.Models;
using PrintScrn.ViewModels;

namespace PrintScrn.Behaviors;

public class MoveAndResizeRectangleBehavior : Behavior<UIElement>
{
    /// <summary>
    /// Mouse position when <see cref="UIElement.PreviewMouseDown"/> event occured.
    /// (i. e. coordinates where the user clicked first time). Relative to the AssociatedObject.
    /// </summary>
    private Point _initialMouseCanvasPosition;

    /// <summary>
    /// Mouse position when <see cref="UIElement.PreviewMouseDown"/> event occured.
    /// (i. e. coordinates where the user clicked first time). Screen coordinates.
    /// </summary>
    private Point _initialMouseScreenPosition;

    /// <summary>
    /// Parent of the associated rectangle.
    /// </summary>
    private Canvas? _parentCanvas;

    #region Properties

    #region RectangleCanvasPosition

    public static readonly DependencyProperty RectangleCanvasPositionProperty = DependencyProperty.Register(
        nameof(RectangleCanvasPosition),
        typeof(RectangleCaptureArea),
        typeof(MoveAndResizeRectangleBehavior),
        new(default(RectangleCaptureArea))
    );

    public RectangleCaptureArea RectangleCanvasPosition
    {
        get => (RectangleCaptureArea)GetValue(RectangleCanvasPositionProperty);
        set => SetValue(RectangleCanvasPositionProperty, value);
    }

    #endregion

    #region RectangleScreenPosition

    public static readonly DependencyProperty RectangleScreenPositionProperty = DependencyProperty.Register(
        nameof(RectangleScreenPosition),
        typeof(RectangleCaptureArea),
        typeof(MoveAndResizeRectangleBehavior),
        new(default(RectangleCaptureArea))
    );

    public RectangleCaptureArea RectangleScreenPosition
    {
        get => (RectangleCaptureArea)GetValue(RectangleScreenPositionProperty);
        set => SetValue(RectangleScreenPositionProperty, value);
    }

    #endregion

    #endregion

    protected override void OnAttached()
    {
        RectangleCanvasPosition = new();
        RectangleScreenPosition = new();
        AssociatedObject.MouseLeftButtonDown += OnButtonDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnButtonDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseUp -= OnMouseUp;
    }

    private void OnButtonDown(object sender, MouseButtonEventArgs e)
    {
        if ((_parentCanvas ??= VisualTreeHelper.GetParent(AssociatedObject) as Canvas) is null)
        {
            return;
        }

        _initialMouseCanvasPosition = e.GetPosition(AssociatedObject);
        _initialMouseScreenPosition = AssociatedObject.PointToScreen(_initialMouseCanvasPosition);

        AssociatedObject.CaptureMouse();

        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseUp += OnMouseUp;
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseUp -= OnMouseUp;

        AssociatedObject.ReleaseMouseCapture();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        if (screenshotCanvasViewModel == null)
        {
            FileLogger.LogError("screenshotCanvasViewModel is null.");
            return;
        }

        var currentMouseCanvasPosition = e.GetPosition(_parentCanvas);
        var canvasPositionDelta = currentMouseCanvasPosition - _initialMouseCanvasPosition;
        if (screenshotCanvasViewModel.CustomSelectedRectangle != null)
        {
            screenshotCanvasViewModel.CustomSelectedRectangle.X = canvasPositionDelta.X;
            screenshotCanvasViewModel.CustomSelectedRectangle.Y = canvasPositionDelta.Y;
        }
        else
        {
            FileLogger.LogError("CustomSelectedRectangle is null.");
            return;
        }

        var currentMouseScreenPosition = e.GetPosition(_parentCanvas);
        var screenPositionDelta = currentMouseScreenPosition - _initialMouseScreenPosition;
        if (screenshotCanvasViewModel.CustomSelectedRectangleScreenCoordinates != null)
        {
            screenshotCanvasViewModel.CustomSelectedRectangleScreenCoordinates.X = screenPositionDelta.X;
            screenshotCanvasViewModel.CustomSelectedRectangleScreenCoordinates.Y = screenPositionDelta.Y;
        }
        else
        {
            FileLogger.LogError("CustomSelectedRectangleScreenCoordinates is null.");
            return;
        }
    }
}
