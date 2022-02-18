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

    /// <summary>
    /// Provides X, Y, Width and Height of the screen.
    /// </summary>
    private Win32Type.RECT _screenBounds;

    /// <summary>
    /// Provides access to the ScreenshotCanvasViewModel.
    /// </summary>
    private ScreenshotCanvasViewModel? _vm = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();

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

        _screenBounds = GraphicsCaptureHelper.GetMonitorRectFromWindow();
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
        if (_vm!.CustomRectangle == null)
        {
            FileLogger.LogError("CustomSelectedRectangle is null.");
            return;
        }

        if (_vm.CustomRectangleScreenCoordinates == null)
        {
            FileLogger.LogError("CustomSelectedRectangleScreenCoordinates is null.");
            return;
        }

        if (_parentCanvas == null)
        {
            FileLogger.LogError("_parentCanvas is null");
            return;
        }

        // Mouse position relative to the parent canvas.
        var currentCanvas = e.GetPosition(_parentCanvas);

        var canvasDelta = currentCanvas - _initialMouseCanvasPosition;
        var screenDelta = _parentCanvas.PointToScreen(new Point(canvasDelta.X, canvasDelta.Y));

        MoveRectangleIfCanBeMoved(canvasDelta.X, canvasDelta.Y, screenDelta.X, screenDelta.Y);
    }

    /// <summary>
    /// This method verifies whether a new possible rectangle position is valid.
    /// If so, the move is performed.  
    /// </summary>
    /// <param name="canvasDeltaX">New X position in canvas coordinates.</param>
    /// <param name="canvasDeltaY">New Y position in canvas coordinates.</param>
    /// <param name="screenDeltaX">New X position in screen coordinates.</param>
    /// <param name="screenDeltaY">New Y position in screen coordinates.</param>
    private void MoveRectangleIfCanBeMoved(
        double canvasDeltaX,
        double canvasDeltaY, 
        double screenDeltaX,
        double screenDeltaY
    )
    {
        //
        // Check before committing the drag operation if the rectangle will
        // be moved outside of canvas bounds.
        //

        bool canMoveToRight = true;
        bool canMoveToLeft = true;
        bool canMoveToTop = true;
        bool canMoveToBottom = true;

        // If true, a rectangle will be moved outside of screen bounds on the _left_ side
        if (screenDeltaX < 0)
        {
            // So we prohibit to move a rectangle to the west and definetely allow moving to the east.
            canMoveToRight = true;
            canMoveToLeft = false;
        }

        // If true, a rectangle will be moved outside of screen bounds on the _top_ side
        if (screenDeltaY < 0)
        {
            // So we prohibit to move a rectangle to the north and definetely allow moving to the south.
            canMoveToBottom = true;
            canMoveToTop = false;
        }

        // If true, a rectangle will be moved outside of screen bounds on the _right_ side
        if (screenDeltaX + _vm!.CustomRectangleScreenCoordinates!.Width > _screenBounds.Width)
        {
            // So we prohibit to move a rectangle to the east and definetely allow moving to the west.
            canMoveToRight = false;
            canMoveToLeft = true;
        }

        // If true, a rectangle will be moved outside of screen bounds on the _bottom_ side
        if (screenDeltaY + _vm.CustomRectangleScreenCoordinates.Height > _screenBounds.Height)
        {
            // So we prohibit to move a rectangle to the south and definetely allow moving to the north.
            canMoveToBottom = false;
            canMoveToTop = true;
        }

        // Moving a rectangle to the right
        if (_vm.CustomRectangleScreenCoordinates.X < screenDeltaX)
        {
            if (canMoveToRight)
            {
                _vm.CustomRectangle!.X = canvasDeltaX;
                _vm.CustomRectangleScreenCoordinates.X = screenDeltaX;
            }
        }
        // Moving a rectangle to the left
        else
        {
            if (canMoveToLeft)
            {
                _vm.CustomRectangle!.X = canvasDeltaX;
                _vm.CustomRectangleScreenCoordinates.X = screenDeltaX;
            }
        }

        // Moving a rectangle to the bottom
        if (_vm.CustomRectangleScreenCoordinates.Y < screenDeltaY)
        {
            if (canMoveToBottom)
            {
                _vm.CustomRectangle!.Y = canvasDeltaY;
                _vm.CustomRectangleScreenCoordinates.Y = screenDeltaY;
            }
        }
        // Moving a rectangle to the top
        else
        {
            if (canMoveToTop)
            {
                _vm.CustomRectangle!.Y = canvasDeltaY;
                _vm.CustomRectangleScreenCoordinates.Y = screenDeltaY;
            }
        }
    }
}
