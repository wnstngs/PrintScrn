using Microsoft.Xaml.Behaviors;
using PrintScrn.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using PrintScrn.Infrastructure;
using PrintScrn.Infrastructure.Extensions;
using PrintScrn.Models;

namespace PrintScrn.Behaviors;

public class RectangleSelectionBehavior : Behavior<UIElement>
{
    /// <summary>
    /// Mouse position when <see cref="UIElement.PreviewMouseDown"/> event occured.
    /// (i. e. coordinates where the user clicked first time).
    /// </summary>
    private Point _initialMousePos;

    private RectangleCaptureArea _rectangleCaptureArea;

    #region Properties

    #region InitialMouseXPos

    public static readonly DependencyProperty InitialMouseXPosProperty = DependencyProperty.Register(
        nameof(InitialMouseXPos),
        typeof(double),
        typeof(RectangleSelectionBehavior),
        new(default(double))
    );

    public double InitialMouseXPos
    {
        get => (double)GetValue(InitialMouseXPosProperty);
        set => SetValue(InitialMouseXPosProperty, value);
    }

    #endregion

    #region InitialMouseYPos

    public static readonly DependencyProperty InitialMouseYPosProperty = DependencyProperty.Register(
        nameof(InitialMouseYPos),
        typeof(double),
        typeof(RectangleSelectionBehavior),
        new(default(double))
    );

    public double InitialMouseYPos
    {
        get => (double)GetValue(InitialMouseYPosProperty);
        set => SetValue(InitialMouseYPosProperty, value);
    }

    #endregion

    #region SelectedRectWidth

    public static readonly DependencyProperty SelectedRectWidthProperty = DependencyProperty.Register(
        nameof(SelectedRectWidth),
        typeof(double),
        typeof(RectangleSelectionBehavior),
        new(default(double))
    );

    public double SelectedRectWidth
    {
        get => (double)GetValue(SelectedRectWidthProperty);
        set => SetValue(SelectedRectWidthProperty, value);
    }

    #endregion

    #region SelectedRectHeight

    public static readonly DependencyProperty SelectedRectHeightProperty = DependencyProperty.Register(
        nameof(SelectedRectHeight),
        typeof(double),
        typeof(RectangleSelectionBehavior),
        new(default(double))
    );

    public double SelectedRectHeight
    {
        get => (double)GetValue(SelectedRectHeightProperty);
        set => SetValue(SelectedRectHeightProperty, value);
    }

    #endregion

    #region InitialMouseXPosScreenCoords

    public static readonly DependencyProperty InitialMouseXPosPropertyScreenCoords = DependencyProperty.Register(
        nameof(InitialMouseXPosScreenCoords),
        typeof(double),
        typeof(RectangleSelectionBehavior),
        new(default(double))
    );

    public double InitialMouseXPosScreenCoords
    {
        get => (double)GetValue(InitialMouseXPosPropertyScreenCoords);
        set => SetValue(InitialMouseXPosPropertyScreenCoords, value);
    }

    #endregion

    #region InitialMouseYPosScreenCoords

    public static readonly DependencyProperty InitialMouseYPosPropertyScreenCoords = DependencyProperty.Register(
        nameof(InitialMouseYPosScreenCoords),
        typeof(double),
        typeof(RectangleSelectionBehavior),
        new(default(double))
    );

    public double InitialMouseYPosScreenCoords
    {
        get => (double)GetValue(InitialMouseYPosPropertyScreenCoords);
        set => SetValue(InitialMouseYPosPropertyScreenCoords, value);
    }

    #endregion

    #region SelectedRectWidthScreenCoords

    public static readonly DependencyProperty SelectedRectWidthPropertyScreenCoords = DependencyProperty.Register(
        nameof(SelectedRectWidthScreenCoords),
        typeof(double),
        typeof(RectangleSelectionBehavior),
        new(default(double))
    );

    public double SelectedRectWidthScreenCoords
    {
        get => (double)GetValue(SelectedRectWidthPropertyScreenCoords);
        set => SetValue(SelectedRectWidthPropertyScreenCoords, value);
    }

    #endregion

    #region SelectedRectHeightScreenCoords

    public static readonly DependencyProperty SelectedRectHeightPropertyScreenCoords = DependencyProperty.Register(
        nameof(SelectedRectHeightScreenCoords),
        typeof(double),
        typeof(RectangleSelectionBehavior),
        new(default(double))
    );

    public double SelectedRectHeightScreenCoords
    {
        get => (double)GetValue(SelectedRectHeightPropertyScreenCoords);
        set => SetValue(SelectedRectHeightPropertyScreenCoords, value);
    }

    #endregion

    #endregion

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// Assign PreviewMouseDown event handler.
    /// </summary>
    protected override void OnAttached()
    {
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
        if (toolbarViewModel != null) toolbarViewModel.ToolbarVisibility = Visibility.Collapsed;

        ResetProperties();

        _initialMousePos = e.GetPosition(AssociatedObject);

        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseUp += OnMouseUp;

        InitialMouseXPos = _initialMousePos.X;
        InitialMouseYPos = _initialMousePos.Y;
        InitialMouseXPosScreenCoords = AssociatedObject.PointToScreen(_initialMousePos).X;
        InitialMouseYPosScreenCoords = AssociatedObject.PointToScreen(_initialMousePos).Y;

        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        if (screenshotCanvasViewModel != null)
        {
            // null because we only need to trigger setter.
            screenshotCanvasViewModel.SelectedRectImageSource = null;
        }
        else
        {
            FileLogger.LogError("'SelectedRectImageSource' is not set: 'screenshotCanvasViewModel' is null.");
        }
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
            toolbarViewModel.ToolbarVisibility = Visibility.Visible;
        }

        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        screenshotCanvasViewModel?.CaptureCustomRectangle.Execute(null);

        ResetProperties();
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
        var currentPosScreenCoords = AssociatedObject.PointToScreen(e.GetPosition(AssociatedObject));

        var delta = currentPos - _initialMousePos;
        var deltaScreenCoords = currentPosScreenCoords - AssociatedObject.PointToScreen(_initialMousePos);

        // TODO: Looks like with '+ 1' we get correct values, but need to double-check.
        // Without it, selecting whole screen I have X = 0, Y = 0, W = 2559, H = 1439 for 2560x1440 monitor.
        // Width and height values are incorrect (they are equal to end mouse position which actually is correct),
        // but we need to think how to correctly find width and height of the selected rectangle.
        SelectedRectWidth = Math.Round(delta.X + 1);
        SelectedRectHeight = Math.Round(delta.Y + 1);
        SelectedRectWidthScreenCoords = Math.Round(deltaScreenCoords.X + 1);
        SelectedRectHeightScreenCoords = Math.Round(deltaScreenCoords.Y + 1);

        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        if (screenshotCanvasViewModel != null)
        {
            // null because we only need to trigger setter.
            screenshotCanvasViewModel.SelectedRectImageSource = null;
        }
        else
        {
            FileLogger.LogError("'SelectedRectImageSource' is not set: 'screenshotCanvasViewModel' is null.");
        }

        AssociatedObject.InvalidateVisual();
    }

    /// <summary>
    /// Zeroes all properties related to the behavior.
    /// </summary>
    private void ResetProperties()
    {
        InitialMouseXPos = 0.0;
        InitialMouseYPos = 0.0;
        SelectedRectWidth = 0.0;
        SelectedRectHeight = 0.0;
        InitialMouseXPosScreenCoords = 0.0;
        InitialMouseYPosScreenCoords = 0.0;
        SelectedRectWidthScreenCoords = 0.0;
        SelectedRectHeightScreenCoords = 0.0;
    }
}
