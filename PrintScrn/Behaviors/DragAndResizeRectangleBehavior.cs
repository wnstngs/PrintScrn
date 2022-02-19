using System.Drawing;
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
public class DragAndResizeRectangleBehavior : Behavior<UIElement>
{
    //
    // Constants
    //

    private const int WidthOfResizableArea = 16;

    private const int MinSizeOfDraggingArea = 50;

    //
    // Private fields
    //

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
    private readonly ScreenshotCanvasViewModel? _vm = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();

    /// <summary>
    /// If true, _isDragging can be set to true.
    /// Supposed to be true when the mouse pointer is inside of a draggable area.
    /// </summary>
    private bool _canDrag;

    /// <summary>
    /// Determines whether a user is currently dragging the rectangle.
    /// </summary>
    private bool _isDragging;

    /// <summary>
    /// If true, _isResizing can be set to true.
    /// Supposed to be true when the mouse pointer is inside of a resizable area.
    /// </summary>
    private bool _canResize;

    /// <summary>
    /// Determines whether a user is currently resizing the rectangle.
    /// </summary>
    private bool _isResizing;

    //
    // Methods
    //

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// Assign PreviewMouseDown event handler.
    /// </summary>
    protected override void OnAttached()
    {
        _parentCanvas = VisualTreeHelper.GetParent(AssociatedObject) as Canvas;
        if (_parentCanvas == null)
        {
            FileLogger.LogError("_parentCanvas is null. DragAndResizeRectangleBehavior won't be attached.");
            return;
        }

        AssociatedObject.PreviewMouseMove += OnMouseMove;
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
        // TODO: Make bounds values of the display monitor available on the application level.
        _screenBounds = GraphicsCaptureHelper.GetMonitorRectFromWindow();
        _initialMouseCanvasPosition = e.GetPosition(AssociatedObject);

        if (_canDrag) _isDragging = true;
        if (_canResize) _isResizing = true;

        AssociatedObject.MouseLeftButtonUp += OnMouseUp;
    }

    /// <summary>
    /// <see cref="UIElement.MouseLeftButtonUp"/> event handler.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        _isDragging = false;
        _isResizing = false;
        AssociatedObject.MouseLeftButtonUp -= OnMouseUp;
    }

    /// <summary>
    /// <see cref="UIElement.PreviewMouseMove"/> event handler.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_parentCanvas == null)
        {
            // _parentCanvas is found in the MouseLeftButtonDown handler, so for some time it can be null.
            // That's why behavior is not working properly before you you click inside a rectangle.
            // Mark it as TODO.
            return;
        }

        // Mouse position relative to the parent canvas.
        var currentCanvas = e.GetPosition(_parentCanvas);
        var canvasDelta = currentCanvas - _initialMouseCanvasPosition;
        var screenDelta = _parentCanvas.PointToScreen(new Point(canvasDelta.X, canvasDelta.Y));

        if (_isDragging)
        {
            PerformRectangleDragIfPossible(canvasDelta.X, canvasDelta.Y, screenDelta.X, screenDelta.Y);
        }
        else if (_isResizing)
        {
            PerformRectangleResizeIfPossible(canvasDelta.X, canvasDelta.Y, screenDelta.X, screenDelta.Y);
        }
        else
        {
            CheckIfCanDragOrResize(currentCanvas);
        }
    }

    /// <summary>
    /// Depending on the position of the mouse pointer it determines if the mouse pointer
    /// is inside of the rectangle (and it can be dragged), over the borders (so selected rectangle can be resized),
    /// or outside of the rectangle (remains regular cursor, no dragging or resizing).
    /// </summary>
    /// <param name="mousePositionCanvas">Current position of the mouse pointer (canvas coordinates).</param>
    private void CheckIfCanDragOrResize(
        Point mousePositionCanvas
    )
    {
        if (_vm == null)
        {
            FileLogger.LogWarning("_vm is null.");
            return;
        }

        if (_vm.CustomRectangle == null)
        {
            FileLogger.LogWarning("_vm.CustomRectangle is null.");
            return;
        }

        var assocRect = AssociatedObject as System.Windows.Shapes.Rectangle;
        if (assocRect == null)
        {
            FileLogger.LogWarning("assocRect is null.");
            return;
        }

        //
        // First of all we have to check if size of the rectangle is < MinSizeOfDraggingArea x MinSizeOfDraggingArea.
        // In that case a rectangle can't be dragged.
        //
        if (_vm.CustomRectangle.Width < MinSizeOfDraggingArea || _vm.CustomRectangle.Height < MinSizeOfDraggingArea)
        {
            // So we check only whether the mouse pointer is inside of any resizing area.
            goto ResizeCheck;
        }

        //
        // Calculate draggable area of the rectangle.
        //
        var draggableAreaX = (int) (_vm.CustomRectangle.X + WidthOfResizableArea);
        var draggableAreaY = (int) (_vm.CustomRectangle.Y + WidthOfResizableArea);
        var draggableAreaWidth = (int) (_vm.CustomRectangle.Width - WidthOfResizableArea * 2);
        var draggableAreaHeight = (int) (_vm.CustomRectangle.Height - WidthOfResizableArea * 2);

        //
        // If mouse pointer is inside of the draggable area allow to drag.
        //
        if (
            new Rectangle(draggableAreaX, draggableAreaY, draggableAreaWidth, draggableAreaHeight).Contains(
                new System.Drawing.Point((int) mousePositionCanvas.X, (int) mousePositionCanvas.Y)
            )
        )
        {
            _canDrag = true;
            assocRect.Cursor = Cursors.SizeAll;
            return;
        }

        // TODO: Check here whether the mouse pointer is inside of a resizing area.
        //
        // If mouse pointer is inside of the resizing area allow user to resize.
        // Resizing area is "rectangle borders area +- WidthOfResizableArea".
        //
        ResizeCheck:
        //
        // Corners:
        //
        if (
            // Top-Left
            new Rectangle(
                (int) (_vm.CustomRectangle.X - WidthOfResizableArea),
                (int) (_vm.CustomRectangle.Y - WidthOfResizableArea),
                WidthOfResizableArea * 2,
                WidthOfResizableArea * 2
            ).Contains(new System.Drawing.Point((int) mousePositionCanvas.X, (int) mousePositionCanvas.Y))
            ||
            // Bottom-Right
            new Rectangle(
                (int) (_vm.CustomRectangle.Width - WidthOfResizableArea + _vm.CustomRectangle.X),
                (int) (_vm.CustomRectangle.Height - WidthOfResizableArea + _vm.CustomRectangle.Y),
                WidthOfResizableArea * 2,
                WidthOfResizableArea * 2
            ).Contains(new System.Drawing.Point((int) mousePositionCanvas.X, (int) mousePositionCanvas.Y))
        )
        {
            assocRect.Cursor = Cursors.SizeNWSE;
            _canResize = true;
            return;
        }

        if (
            // Top-Right
            new Rectangle(
                (int) (_vm.CustomRectangle.X - WidthOfResizableArea),
                (int) (_vm.CustomRectangle.Height - WidthOfResizableArea + _vm.CustomRectangle.Y),
                WidthOfResizableArea * 2,
                WidthOfResizableArea * 2
            ).Contains(new System.Drawing.Point((int) mousePositionCanvas.X, (int) mousePositionCanvas.Y))
            ||
            // Bottom-Left
            new Rectangle(
                (int) (_vm.CustomRectangle.Width - WidthOfResizableArea + _vm.CustomRectangle.X),
                (int) (_vm.CustomRectangle.Y - WidthOfResizableArea),
                WidthOfResizableArea * 2,
                WidthOfResizableArea * 2
            ).Contains(new System.Drawing.Point((int) mousePositionCanvas.X, (int) mousePositionCanvas.Y))
        )
        {
            assocRect.Cursor = Cursors.SizeNESW;
            _canResize = true;
            return;
        }

        //
        // Edges
        //
        if (
            // Top edge
            new Rectangle(
                (int) (_vm.CustomRectangle.X + WidthOfResizableArea),
                (int) (_vm.CustomRectangle.Y - WidthOfResizableArea),
                (int) (_vm.CustomRectangle.Width - WidthOfResizableArea * 2),
                WidthOfResizableArea * 2
            ).Contains(new System.Drawing.Point((int) mousePositionCanvas.X, (int) mousePositionCanvas.Y))
            ||
            // Bottom edge
            new Rectangle(
                (int) (_vm.CustomRectangle.X + WidthOfResizableArea),
                (int) (_vm.CustomRectangle.Height - WidthOfResizableArea + _vm.CustomRectangle.Y),
                (int) (_vm.CustomRectangle.Width - WidthOfResizableArea * 2),
                WidthOfResizableArea * 2
            ).Contains(new System.Drawing.Point((int) mousePositionCanvas.X, (int) mousePositionCanvas.Y))
        )
        {
            assocRect.Cursor = Cursors.SizeNS;
            _canResize = true;
            return;
        }

        if (
            // Left edge
            new Rectangle(
                (int) (_vm.CustomRectangle.X - WidthOfResizableArea),
                (int) (_vm.CustomRectangle.Y + WidthOfResizableArea),
                WidthOfResizableArea * 2,
                (int) (_vm.CustomRectangle.Height - WidthOfResizableArea * 2)
            ).Contains(new System.Drawing.Point((int) mousePositionCanvas.X, (int) mousePositionCanvas.Y))
            ||
            // Right edge
            new Rectangle(
                (int) (_vm.CustomRectangle.Width - WidthOfResizableArea + _vm.CustomRectangle.X),
                (int) (_vm.CustomRectangle.Y + WidthOfResizableArea),
                WidthOfResizableArea * 2,
                (int) (_vm.CustomRectangle.Height - WidthOfResizableArea * 2)
            ).Contains(new System.Drawing.Point((int) mousePositionCanvas.X, (int) mousePositionCanvas.Y))
        )
        {
            assocRect.Cursor = Cursors.SizeWE;
            _canResize = true;
            return;
        }

        //
        // None of the dragging or resizing conditions were true, so disallow dragging/resizing operations.
        //
        assocRect.Cursor = Cursors.Arrow;
        _canResize = false;
        _canDrag = false;
    }

    /// <summary>
    /// This method verifies whether a new possible rectangle position is valid.
    /// If so, the move is performed.  
    /// </summary>
    /// <param name="canvasDeltaX">New X position in canvas coordinates.</param>
    /// <param name="canvasDeltaY">New Y position in canvas coordinates.</param>
    /// <param name="screenDeltaX">New X position in screen coordinates.</param>
    /// <param name="screenDeltaY">New Y position in screen coordinates.</param>
    private void PerformRectangleDragIfPossible(
        double canvasDeltaX,
        double canvasDeltaY,
        double screenDeltaX,
        double screenDeltaY
    )
    {
        if (_vm == null)
        {
            FileLogger.LogWarning("_vm is null.");
            return;
        }

        if (_vm.CustomRectangleScreenCoordinates == null || _vm.CustomRectangle == null)
        {
            FileLogger.LogWarning("_vm.CustomRectangleScreenCoordinates || _vm.CustomRectangle is null.");
            return;
        }

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
        if (screenDeltaX + _vm.CustomRectangleScreenCoordinates.Width > _screenBounds.Width)
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
                _vm.CustomRectangle.X = canvasDeltaX;
                _vm.CustomRectangleScreenCoordinates.X = screenDeltaX;
            }
        }
        // Moving a rectangle to the left
        else
        {
            if (canMoveToLeft)
            {
                _vm.CustomRectangle.X = canvasDeltaX;
                _vm.CustomRectangleScreenCoordinates.X = screenDeltaX;
            }
        }

        // Moving a rectangle to the bottom
        if (_vm.CustomRectangleScreenCoordinates.Y < screenDeltaY)
        {
            if (canMoveToBottom)
            {
                _vm.CustomRectangle.Y = canvasDeltaY;
                _vm.CustomRectangleScreenCoordinates.Y = screenDeltaY;
            }
        }
        // Moving a rectangle to the top
        else
        {
            if (canMoveToTop)
            {
                _vm.CustomRectangle.Y = canvasDeltaY;
                _vm.CustomRectangleScreenCoordinates.Y = screenDeltaY;
            }
        }
    }

    /// <summary>
    /// This method verifies whether a new size rectangle position is valid.
    /// If so, the resize is performed.  
    /// </summary>
    /// <param name="canvasDeltaX">New X position in canvas coordinates.</param>
    /// <param name="canvasDeltaY">New Y position in canvas coordinates.</param>
    /// <param name="screenDeltaX">New X position in screen coordinates.</param>
    /// <param name="screenDeltaY">New Y position in screen coordinates.</param>
    private void PerformRectangleResizeIfPossible(
        double canvasDeltaX,
        double canvasDeltaY,
        double screenDeltaX,
        double screenDeltaY
    )
    {
    }
}
