using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls;
using Core2D.Controls.Studio;
using Core2D.Model.Editor;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.History;
using Xunit;
using PageView = Core2D.Controls.Editor.PageView;

namespace Core2D.UI.Tests;

public class StudioCanvasSafetyTests
{
    [Theory]
    [InlineData(1e9, 1)]
    [InlineData(1e12, 10)]
    [InlineData(1e15, 100)]
    public void ScientificRulerLabelsRetainVisibleTickPrecision(double first, double step)
    {
        var scale = new RulerScale(first, step, 10, 10);
        Assert.NotEqual(scale.Format(first, CultureInfo.InvariantCulture), scale.Format(first + step, CultureInfo.InvariantCulture));
    }

    [AvaloniaFact]
    public void GuideVisibilityLockAndExternalChangesInvalidateOnlyPreviews()
    {
        var history = new StackHistory();
        var guides = new CanvasGuidesViewModel(history);
        var overlay = new StudioCanvasOverlay();
        using var navigation = new CanvasNavigationViewModel(_ => { }, () => { }, () => { });
        overlay.Bind(navigation, guides);
        Guid id = guides.Add(true, 44)!.Value;
        foreach (var action in new Action[] { () => guides.IsLocked = true, () => guides.IsVisible = false, () => overlay.ShowGuides = false })
        {
            guides.IsLocked = false; guides.IsVisible = true; overlay.ShowGuides = true;
            overlay.Preview = new CanvasGuide(id, true, 88);
            overlay.SelectedGuide = id;
            action();
            Assert.Null(overlay.Preview);
            Assert.Null(overlay.SelectedGuide);
            Assert.Equal(44, Assert.Single(guides.Items).Position);
        }
        guides.IsVisible = true; overlay.ShowGuides = true;
        overlay.Preview = new CanvasGuide(id, true, 88);
        Assert.True(history.Undo());
        Assert.Null(overlay.Preview);
        Assert.Empty(guides.Items);
        overlay.Bind(null, null);
    }

    [AvaloniaFact]
    public void HidingGuidesDuringRulerDragCannotCommitOnRelease()
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var canvas = new PageView { DataContext = state.Editor };
        var window = new Window { Width = 900, Height = 700, Content = canvas };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var overlay = canvas.FindControl<StudioCanvasOverlay>("CanvasOverlay")!;
            var ruler = canvas.FindControl<Ruler>("HorizontalRuler")!;
            Point start = ruler.TranslatePoint(new Point(120, 10), window)!.Value;
            Point target = overlay.TranslatePoint(new Point(180, 180), window)!.Value;
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(target);
            Assert.NotNull(overlay.Preview);
            overlay.Guides!.IsVisible = false;
            Assert.Null(overlay.Preview);
            window.MouseUp(target, MouseButton.Left);
            Assert.Empty(overlay.Guides.Items);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void SharedPageGuidesDoNotShareViewportZoomCommands()
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var first = new PageView { DataContext = state.Editor };
        var second = new PageView { DataContext = state.Editor };
        var panel = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
        panel.Children.Add(first); panel.Children.Add(second); Grid.SetColumn(second, 1);
        var window = new Window { Width = 1200, Height = 700, Content = panel };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var left = first.FindControl<StudioCanvasOverlay>("CanvasOverlay")!;
            var right = second.FindControl<StudioCanvasOverlay>("CanvasOverlay")!;
            var firstZoom = first.FindControl<ZoomBorder>("PageZoomBorder")!;
            var secondZoom = second.FindControl<ZoomBorder>("PageZoomBorder")!;
            Assert.Same(left.Guides, right.Guides);
            Assert.NotSame(left.Navigation, right.Navigation);
            double secondScale = secondZoom.ZoomX;
            left.Navigation!.ZoomPercent = 200;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(2, firstZoom.ZoomX);
            Assert.Equal(secondScale, secondZoom.ZoomX);
            left.Guides!.Add(false, 80);
            Assert.Single(right.Guides!.Items);
        }
        finally { window.Close(); }
    }
}
