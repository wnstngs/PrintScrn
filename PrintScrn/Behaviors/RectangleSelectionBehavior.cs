using Microsoft.Xaml.Behaviors;
using PrintScrn.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using PrintScrn.Infrastructure.Extensions;
using PrintScrn.Models;
using Point = System.Windows.Point;

namespace PrintScrn.Behaviors;

/// <summary>
/// The RectangleSelectionBehavior is responsible for selection of a rectangle on the ScreenshotCanvas.
/// </summary>
public class RectangleSelectionBehavior : Behavior<UIElement>
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

    #region Properties

    #region SelectedRectangleCanvasPosition

    public static readonly DependencyProperty SelectedRectangleCanvasPositionProperty = DependencyProperty.Register(
        nameof(SelectedRectangleCanvasPosition),
        typeof(RectangleCaptureArea),
        typeof(RectangleSelectionBehavior),
        new(default(RectangleCaptureArea))
    );

    public RectangleCaptureArea SelectedRectangleCanvasPosition
    {
        get => (RectangleCaptureArea) GetValue(SelectedRectangleCanvasPositionProperty);
        set => SetValue(SelectedRectangleCanvasPositionProperty, value);
    }

    #endregion

    #region SelectedRectangleScreenPosition

    public static readonly DependencyProperty SelectedRectangleScreenPositionProperty = DependencyProperty.Register(
        nameof(SelectedRectangleScreenPosition),
        typeof(RectangleCaptureArea),
        typeof(RectangleSelectionBehavior),
        new(default(RectangleCaptureArea))
    );

    public RectangleCaptureArea SelectedRectangleScreenPosition
    {
        get => (RectangleCaptureArea) GetValue(SelectedRectangleScreenPositionProperty);
        set => SetValue(SelectedRectangleScreenPositionProperty, value);
    }

    #endregion

    #endregion

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// Assign PreviewMouseDown event handler.
    /// </summary>
    protected override void OnAttached()
    {
        SelectedRectangleCanvasPosition = new();
        SelectedRectangleScreenPosition = new();
        AssociatedObject.PreviewMouseDown += OnMouseDown;
    }

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
    /// Unregisters all event handlers.
    /// </summary>
    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseDown -= OnMouseDown;
        AssociatedObject.PreviewMouseMove -= OnMouseMove;
        AssociatedObject.PreviewMouseUp -= OnMouseUp;
    }

    /// <summary>
    /// <see cref="UIElement.PreviewMouseDown"/> event handler.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var toolbarViewModel = ViewModelsExtension.FindViewModel<ToolbarViewModel>();
        if (toolbarViewModel != null)
        {
            // Hide a toolbar
            toolbarViewModel.ToolbarVisibility = Visibility.Collapsed;
        }

        ResetSelectedRectangle();

        _initialMouseCanvasPosition = e.GetPosition(AssociatedObject);
        _initialMouseScreenPosition = AssociatedObject.PointToScreen(_initialMouseCanvasPosition);

        SelectedRectangleCanvasPosition.X = _initialMouseCanvasPosition.X;
        SelectedRectangleCanvasPosition.Y = _initialMouseCanvasPosition.Y;

        SelectedRectangleScreenPosition.X = _initialMouseScreenPosition.X;
        SelectedRectangleScreenPosition.Y = _initialMouseScreenPosition.Y;

        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseUp += OnMouseUp;
    }

    /// <summary>
    /// <see cref="UIElement.PreviewMouseUp"/> event handler.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseUp -= OnMouseUp;

        var toolbarViewModel = ViewModelsExtension.FindViewModel<ToolbarViewModel>();
        if (toolbarViewModel != null)
        {
            // Show a toolbar
            toolbarViewModel.ToolbarVisibility = Visibility.Visible;
        }

        AssociatedObject.PreviewMouseDown += OnMouseDown;
    }

    /// <summary>
    /// <see cref="UIElement.PreviewMouseMove"/> event handler.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        AssociatedObject.PreviewMouseDown -= OnMouseDown;

        Point currentPos = e.GetPosition(AssociatedObject);
        Point currentPosScreenCoordinates = AssociatedObject.PointToScreen(currentPos);

        // Update position of the selected rectangle.
        SelectedRectangleCanvasPosition.X = Math.Min(currentPos.X, _initialMouseCanvasPosition.X);
        SelectedRectangleCanvasPosition.Y = Math.Min(currentPos.Y, _initialMouseCanvasPosition.Y);
        SelectedRectangleCanvasPosition.Width =
            Math.Max(currentPos.X, _initialMouseCanvasPosition.X) - Math.Min(currentPos.X, _initialMouseCanvasPosition.X) + 1;
        SelectedRectangleCanvasPosition.Height =
            Math.Max(currentPos.Y, _initialMouseCanvasPosition.Y) - Math.Min(currentPos.Y, _initialMouseCanvasPosition.Y) + 1;

        // Update position of the selected rectangle in screen coordinates.
        SelectedRectangleScreenPosition.X =
            Math.Min(currentPosScreenCoordinates.X, _initialMouseScreenPosition.X);
        SelectedRectangleScreenPosition.Y =
            Math.Min(currentPosScreenCoordinates.Y, _initialMouseScreenPosition.Y);
        SelectedRectangleScreenPosition.Width =
            Math.Max(currentPosScreenCoordinates.X, _initialMouseScreenPosition.X) -
            Math.Min(currentPosScreenCoordinates.X, _initialMouseScreenPosition.X) + 1;
        SelectedRectangleScreenPosition.Height =
            Math.Max(currentPosScreenCoordinates.Y, _initialMouseScreenPosition.Y) -
            Math.Min(currentPosScreenCoordinates.Y, _initialMouseScreenPosition.Y) + 1;

        // Force redraw.
        AssociatedObject.InvalidateVisual();
    }

    /// <summary>
    /// Zeroes all properties related to the behavior.
    /// </summary>
    private void ResetSelectedRectangle()
    {
        SelectedRectangleCanvasPosition.X = 0;
        SelectedRectangleCanvasPosition.Y = 0;
        SelectedRectangleCanvasPosition.Width = 0;
        SelectedRectangleCanvasPosition.Height = 0;
    }
}
