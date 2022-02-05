using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using PrintScrn.Extensions;
using PrintScrn.ViewModels;

namespace PrintScrn.Behaviors;

public class CaptureRectangleSelectionBehavior : Behavior<UIElement>
{
    private Canvas? _canvas;
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
        InitialXPosition = 0.0;
        InitialYPosition = 0.0;
        SelectedRectWidth = 0.0;
        SelectedRectHeight = 0.0;

        _startPoint = e.GetPosition(AssociatedObject);

        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseUp += OnMouseUp;

        InitialXPosition = _startPoint.X;
        InitialYPosition = _startPoint.Y;

        if (((AssociatedObject.FindVisualRoot() as Window)?.DataContext) is MainViewModel vm)
        {
            vm.SelectedRectImageSource = vm.ScreenImageSource;
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseUp -= OnMouseUp;

        if (((AssociatedObject.FindVisualRoot() as Window)?.DataContext) is MainViewModel vm)
        {
            vm.SelectedRectImageSource = vm.ScreenImageSource;
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var currentPos = e.GetPosition(AssociatedObject);

        var delta = currentPos - _startPoint;

        SelectedRectWidth = delta.X;
        SelectedRectHeight = delta.Y;

        if (((AssociatedObject.FindVisualRoot() as Window)?.DataContext) is MainViewModel vm)
        {
            vm.SelectedRectImageSource = vm.ScreenImageSource;
        }
    }
}
