﻿using System.Windows;
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
    /// Parent of the associated rectangle.
    /// </summary>
    private Canvas? _parentCanvas;

    protected override void OnAttached()
    {
        AssociatedObject.MouseLeftButtonDown += OnButtonDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnButtonDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseUp;
    }

    private void OnButtonDown(object sender, MouseButtonEventArgs e)
    {
        if ((_parentCanvas ??= VisualTreeHelper.GetParent(AssociatedObject) as Canvas) is null)
        {
            return;
        }

        _initialMouseCanvasPosition = e.GetPosition(AssociatedObject);

        AssociatedObject.CaptureMouse();

        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp += OnMouseUp;
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseUp -= OnMouseUp;

        AssociatedObject.ReleaseMouseCapture();
    }

    // TODO: https://github.com/wnstngs/PrintScrn/commit/89b460eca2055f5e0bdbd099dc4cbd054ea8e8f5#commitcomment-66839838
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var vm = ViewModelsExtension.FindViewModel<ScreenshotCanvasViewModel>();
        if (vm == null)
        {
            FileLogger.LogError("screenshotCanvasViewModel is null.");
            return;
        }

        var currentCanvas = e.GetPosition(_parentCanvas);
        var canvasDelta = currentCanvas - _initialMouseCanvasPosition;
        if (vm.CustomRectangle != null)
        {
            vm.CustomRectangle.X = canvasDelta.X;
            vm.CustomRectangle.Y = canvasDelta.Y;
        }
        else
        {
            FileLogger.LogError("CustomSelectedRectangle is null.");
            return;
        }

        if (vm.CustomRectangleScreenCoordinates != null)
        {
            vm.CustomRectangleScreenCoordinates.X = _parentCanvas.PointToScreen(
                new Point(canvasDelta.X, canvasDelta.Y)
            ).X;
            vm.CustomRectangleScreenCoordinates.Y = _parentCanvas.PointToScreen(
                new Point(canvasDelta.X, canvasDelta.Y)
            ).Y;
        }
        else
        {
            FileLogger.LogError("CustomSelectedRectangleScreenCoordinates is null.");
        }
    }
}
