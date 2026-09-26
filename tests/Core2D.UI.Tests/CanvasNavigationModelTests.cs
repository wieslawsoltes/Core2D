using System;
using System.Reactive.Linq;
using Avalonia.Headless.XUnit;
using Core2D.ViewModels.Editor;
using Xunit;

namespace Core2D.UI.Tests;

public class CanvasNavigationModelTests
{
    [AvaloniaFact]
    public void MeasuredZoomDoesNotWriteBackIntoViewport()
    {
        int writes = 0;
        double zoom = 0;
        using var navigation = new CanvasNavigationViewModel(value => { zoom = value; writes++; }, () => { }, () => { });
        navigation.ZoomPercent = 250;
        Assert.Equal(2.5, zoom);
        Assert.Equal(1, writes);
        navigation.UpdateZoom(1.25);
        Assert.Equal(125m, navigation.ZoomPercent);
        Assert.Equal(1, writes);
        navigation.UpdateZoom(double.NaN);
        Assert.Equal(125m, navigation.ZoomPercent);
        navigation.ZoomPercent = 100000;
        Assert.Equal(256, zoom);
        navigation.ZoomPercent = -10;
        Assert.Equal(.01, zoom);
    }

    [AvaloniaFact]
    public void ZoomCommandsInvokeOnlyTheirOwnViewport()
    {
        double first = 1, second = 1;
        using var left = new CanvasNavigationViewModel(value => first = value, () => { }, () => { });
        using var right = new CanvasNavigationViewModel(value => second = value, () => { }, () => { });
        left.ZoomIn.Execute().Subscribe();
        Assert.Equal(1.25, first);
        Assert.Equal(1, second);
        right.ZoomOut.Execute().Subscribe();
        Assert.Equal(.8, second);
        Assert.Equal(1.25, first);
        left.ResetZoom.Execute().Subscribe();
        Assert.Equal(1, first);
        Assert.Equal(.8, second);
    }
}
