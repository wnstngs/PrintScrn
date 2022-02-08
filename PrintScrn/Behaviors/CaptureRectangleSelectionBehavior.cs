using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using PrintScrn.Extensions;
using PrintScrn.ViewModels;

namespace PrintScrn.Behaviors;

public class CaptureRectangleSelectionBehavior : Behavior<UIElement>
{
    private Point _startPoint;

    #region Properties

    #region InitialXPosition

    public static readonly DependencyProperty InitialXPositionProperty = DependencyProperty.Register(
        nameof(InitialXPosition),
        typeof(double),
        typeof(CaptureRectangleSelectionBehavior),
        new PropertyMetadata(default(double))
    );

    public double InitialXPosition
    {
        get => (double)GetValue(InitialXPositionProperty);
        set => SetValue(InitialXPositionProperty, value);
    }

    #endregion

    #region InitialYPosition

    public static readonly DependencyProperty InitialYPositionProperty = DependencyProperty.Register(
        nameof(InitialYPosition),
        typeof(double),
        typeof(CaptureRectangleSelectionBehavior),
        new PropertyMetadata(default(double))
    );

    public double InitialYPosition
    {
        get => (double)GetValue(InitialYPositionProperty);
        set => SetValue(InitialYPositionProperty, value);
    }

    #endregion

    #region SelectedRectWidth

    public static readonly DependencyProperty SelectedRectWidthProperty = DependencyProperty.Register(
        nameof(SelectedRectWidth),
        typeof(double),
        typeof(CaptureRectangleSelectionBehavior),
        new PropertyMetadata(default(double))
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
        typeof(CaptureRectangleSelectionBehavior),
        new PropertyMetadata(default(double))
    );

    public double SelectedRectHeight
    {
        get => (double)GetValue(SelectedRectHeightProperty);
        set => SetValue(SelectedRectHeightProperty, value);
    }

    #endregion

    #endregion

    protected override void OnAttached()
    {
        AssociatedObject.MouseLeftButtonDown += OnMouseDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnMouseDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseUp;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        foreach (var vm in ViewModels.ViewModels.Instance.ViewModelsStore)
        {
            if (vm.GetType() != typeof(ToolbarViewModel)) continue;
            var toolbarViewModel = (ToolbarViewModel) vm;
            toolbarViewModel.ToolbarVisibility = Visibility.Collapsed;
        }

        InitialXPosition = 0.0;
        InitialYPosition = 0.0;
        SelectedRectWidth = 0.0;
        SelectedRectHeight = 0.0;

        _startPoint = e.GetPosition(AssociatedObject);

        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseUp += OnMouseUp;

        InitialXPosition = _startPoint.X;
        InitialYPosition = _startPoint.Y;

        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        screenshotCanvasViewModel?.UpdateSelectedRectCmd.Execute(null);
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseUp -= OnMouseUp;

        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        screenshotCanvasViewModel?.UpdateSelectedRectCmd.Execute(null);

        var toolbarViewModel = ViewModelsExtension.FindViewModel<ToolbarViewModel>();
        if (toolbarViewModel != null)
        {
            toolbarViewModel.ToolbarVisibility = Visibility.Visible;
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var currentPos = e.GetPosition(AssociatedObject);

        var delta = currentPos - _startPoint;

        SelectedRectWidth = Math.Round(delta.X);
        SelectedRectHeight = Math.Round(delta.Y);

        var screenshotCanvasViewModel = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        screenshotCanvasViewModel?.UpdateSelectedRectCmd.Execute(null);

        AssociatedObject.InvalidateVisual();
    }
}
