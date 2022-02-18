using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using PrintScrn.Infrastructure;
using PrintScrn.Infrastructure.Extensions;
using PrintScrn.Infrastructure.Helpers;
using PrintScrn.Infrastructure.Native;
using PrintScrn.ViewModels;
using Point = System.Windows.Point;

namespace PrintScrn.Behaviors;

/// <summary>
/// The RectangleSelectionBehavior is responsible for dragging and resizing of a rectangle
/// on the ScreenshotCanvas.
/// </summary>
public class MoveAndResizeRectangleBehavior : Behavior<UIElement>
{
    /// <summary>
    /// Mouse position when <see cref="UIElement.PreviewMouseDown"/> event occured.
    /// (i. e. coordinates where the user clicked first time). Relative to the AssociatedObject.
    /// </summary>
    private Point _initialMouseCanvasPosition;

    /// <summary>
    /// Parent of the associated rectangle.
    /// </summary>
    private Canvas? _parentCanvas;

    private Win32Type.RECT screenBounds;

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// Assign PreviewMouseDown event handler.
    /// </summary>
    protected override void OnAttached()
    {
        AssociatedObject.MouseLeftButtonDown += OnButtonDown;
    }

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
    /// Unregisters all event handlers.
    /// </summary>
    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnButtonDown;
        AssociatedObject.PreviewMouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseUp;
    }

    /// <summary>
    /// <see cref="UIElement.MouseLeftButtonDown"/> event handler.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Find parent (ScreenshotCanvas) of the AssociatedObject.
        if ((_parentCanvas ??= VisualTreeHelper.GetParent(AssociatedObject) as Canvas) is null)
        {
            return;
        }

        screenBounds = GraphicsCaptureHelper.GetMonitorRectFromWindow();
        _initialMouseCanvasPosition = e.GetPosition(AssociatedObject);

        AssociatedObject.CaptureMouse();

        AssociatedObject.PreviewMouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp += OnMouseUp;
    }

    /// <summary>
    /// <see cref="UIElement.MouseLeftButtonUp"/> event handler.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        AssociatedObject.PreviewMouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseUp;

        AssociatedObject.ReleaseMouseCapture();
    }

    /// <summary>
    /// <see cref="UIElement.PreviewMouseMove"/> event handler.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var vm = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        if (vm == null)
        {
            FileLogger.LogError("screenshotCanvasViewModel is null.");
            return;
        }

        if (vm.CustomRectangle == null)
        {
            FileLogger.LogError("CustomSelectedRectangle is null.");
            return;
        }

        if (vm.CustomRectangleScreenCoordinates == null)
        {
            FileLogger.LogError("CustomSelectedRectangleScreenCoordinates is null.");
            return;
        }

        if (_parentCanvas == null)
        {
            FileLogger.LogError("_parentCanvas is null");
            return;
        }

        var currentCanvas = e.GetPosition(_parentCanvas);
        var canvasDelta = currentCanvas - _initialMouseCanvasPosition;
        var screenDelta = _parentCanvas.PointToScreen(
            new Point(canvasDelta.X, canvasDelta.Y)
        );

        //
        // Check before committing the drag operation if the rectangle will
        // be moved outside of canvas bounds.
        //

        bool canMoveToRight = true;
        bool canMoveToLeft = true;
        bool canMoveToTop = true;
        bool canMoveToBottom = true;

        if (screenDelta.X < 0)
        {
            canMoveToRight = true;
            canMoveToLeft = false;
        }

        if (screenDelta.Y < 0)
        {
            canMoveToBottom = true;
            canMoveToTop = false;
        }

        if (screenDelta.X + vm.CustomRectangleScreenCoordinates.Width > screenBounds.Width)
        {
            canMoveToRight = false;
            canMoveToLeft = true;
        }

        if (screenDelta.Y + vm.CustomRectangleScreenCoordinates.Height > screenBounds.Height)
        {
            canMoveToBottom = false;
            canMoveToTop = true;
        }

        if (vm.CustomRectangleScreenCoordinates.X < screenDelta.X)
        {
            if (canMoveToRight)
            {
                vm.CustomRectangle.X = canvasDelta.X;
                vm.CustomRectangleScreenCoordinates.X = screenDelta.X;
            }
        }
        else
        {
            if (canMoveToLeft)
            {
                vm.CustomRectangle.X = canvasDelta.X;
                vm.CustomRectangleScreenCoordinates.X = screenDelta.X;
            }
        }

        if (vm.CustomRectangleScreenCoordinates.Y < screenDelta.Y)
        {
            if (canMoveToBottom)
            {
                vm.CustomRectangle.Y = canvasDelta.Y;
                vm.CustomRectangleScreenCoordinates.Y = screenDelta.Y;
            }
        }
        else
        {
            if (canMoveToTop)
            {
                vm.CustomRectangle.Y = canvasDelta.Y;
                vm.CustomRectangleScreenCoordinates.Y = screenDelta.Y;
            }
        }
    }
}
