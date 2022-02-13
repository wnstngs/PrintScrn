using Microsoft.Xaml.Behaviors;
using PrintScrn.Extensions;
using PrintScrn.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace PrintScrn.Behaviors;

public class RectangleSelectionBehavior : Behavior<UIElement>
{
    private Point _startPoint;

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

    protected override void OnAttached()
    {
        AssociatedObject.MouseDown += OnMouseDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseDown -= OnMouseDown;
        AssociatedObject.PreviewMouseMove -= OnMouseMove;
        AssociatedObject.PreviewMouseUp -= OnMouseUp;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var toolbarViewModel = ViewModelsExtension.FindViewModel<ToolbarViewModel>();
        if (toolbarViewModel != null) toolbarViewModel.ToolbarVisibility = Visibility.Collapsed;

        ResetProperties();

        _startPoint = e.GetPosition(AssociatedObject);

        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseUp += OnMouseUp;

        InitialMouseXPos = _startPoint.X;
        InitialMouseYPos = _startPoint.Y;
        InitialMouseXPosScreenCoords = AssociatedObject.PointToScreen(_startPoint).X;
        InitialMouseYPosScreenCoords = AssociatedObject.PointToScreen(_startPoint).Y;

        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        if (screenshotCanvasViewModel != null) screenshotCanvasViewModel.SelectedRectImageSource = null;
    }

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
        screenshotCanvasViewModel?.SnapshotCustomRectangle.Execute(null);

        ResetProperties();
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var currentPos = e.GetPosition(AssociatedObject);
        var currentPosScreenCoords = AssociatedObject.PointToScreen(e.GetPosition(AssociatedObject));

        var delta = currentPos - _startPoint;
        var deltaScreenCoords = currentPosScreenCoords - AssociatedObject.PointToScreen(_startPoint);

        SelectedRectWidth = Math.Floor(delta.X);
        SelectedRectHeight = Math.Floor(delta.Y);
        SelectedRectWidthScreenCoords = Math.Floor(deltaScreenCoords.X);
        SelectedRectHeightScreenCoords = Math.Floor(deltaScreenCoords.Y);

        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        if (screenshotCanvasViewModel != null) screenshotCanvasViewModel.SelectedRectImageSource = null;

        AssociatedObject.InvalidateVisual();
    }

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
