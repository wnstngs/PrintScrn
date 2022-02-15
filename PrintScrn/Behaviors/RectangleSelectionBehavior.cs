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
    /// (i. e. coordinates where the user clicked first time).
    /// </summary>
    private Point _initialMousePos;

    #region Properties

    #region SelectedRectangleCaptureArea

    public static readonly DependencyProperty SelectedRectangleProperty = DependencyProperty.Register(
        nameof(SelectedRectangle),
        typeof(RectangleCaptureArea),
        typeof(RectangleSelectionBehavior),
        new(default(RectangleCaptureArea))
    );

    public RectangleCaptureArea SelectedRectangle
    {
        get => (RectangleCaptureArea) GetValue(SelectedRectangleProperty);
        set => SetValue(SelectedRectangleProperty, value);
    }

    #endregion

    #endregion

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// Assign PreviewMouseDown event handler.
    /// </summary>
    protected override void OnAttached()
    {
        SelectedRectangle = new();
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

        _initialMousePos = e.GetPosition(AssociatedObject);

        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseUp += OnMouseUp;

        SelectedRectangle.X = _initialMousePos.X;
        SelectedRectangle.Y = _initialMousePos.Y;
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

        var currentPos = e.GetPosition(AssociatedObject);
        var delta = currentPos - _initialMousePos;

        // TODO: Looks like with '+ 1' we get correct values, but need to double-check.
        SelectedRectangle.Width = Math.Round(delta.X + 1);
        SelectedRectangle.Height = Math.Round(delta.Y + 1);

        // Force redraw
        AssociatedObject.InvalidateVisual();
    }

    /// <summary>
    /// Zeroes all properties related to the behavior.
    /// </summary>
    private void ResetSelectedRectangle()
    {
        SelectedRectangle.X = 0;
        SelectedRectangle.Y = 0;
        SelectedRectangle.Width = 0;
        SelectedRectangle.Height = 0;
    }
}
