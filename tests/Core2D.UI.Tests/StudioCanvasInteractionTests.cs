// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls;
using Core2D.Controls.Studio;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.Views;
using Xunit;
using PageView = Core2D.Controls.Editor.PageView;

namespace Core2D.UI.Tests;

public class StudioCanvasInteractionTests
{
    [AvaloniaTheory]
    [InlineData(false, 1440, 900)]
    [InlineData(true, 1440, 900)]
    [InlineData(true, 1024, 768)]
    public void RulersGuidesAndLayersRenderInTheRealWorkspace(bool dark, int width, int height)
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var view = new MainView { DataContext = state.Editor };
        var window = new Window { Width = width, Height = height, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var canvas = Assert.Single(view.GetVisualDescendants().OfType<PageView>());
            var overlay = canvas.FindControl<StudioCanvasOverlay>("CanvasOverlay")!;
            Assert.NotNull(overlay.Navigation);
            Assert.True(overlay.Navigation!.HasSelection);
            overlay.Navigation.FitPage.Execute().Subscribe();
            overlay.Guides!.Add(true, 44);
            overlay.Guides.Add(false, 180);
            overlay.Guides.Add(true, 596);
            var ruler = canvas.FindControl<Ruler>("HorizontalRuler")!;
            Assert.Equal(20, ruler.Bounds.Height);
            Assert.Equal(44, ruler.SelectionStart);
            Assert.Equal(266, ruler.SelectionLength);
            Assert.Equal("Auto", ruler.LabelFormat);
            Assert.NotEmpty(view.GetVisualDescendants().OfType<StudioLayerRow>());
            var container = canvas.FindControl<Control>("ContainerPanel")!;
            var zoom = canvas.FindControl<ZoomBorder>("PageZoomBorder")!;
            Point cursor = container.TranslatePoint(new Point(210, 140), window)!.Value;
            window.MouseMove(cursor);
            Assert.InRange(Math.Abs(ruler.Marker!.Value - 210), 0, .01);
            Point origin = container.TranslatePoint(default, overlay)!.Value;
            Assert.InRange(Math.Abs(origin.X - ruler.Offset), 0, .01);
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            string? directory = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
            if (directory is not null)
            {
                Directory.CreateDirectory(directory);
                frame!.Save(Path.Combine(directory, $"canvas-guides-{(dark ? "dark" : "light")}-{width}.png"));
            }
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void RulerDragCreatesMovesDuplicatesAndDeletesGuidesWithoutDrawing()
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        var canvas = new PageView { DataContext = editor };
        var window = new Window { Width = 900, Height = 700, Content = canvas };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var overlay = canvas.FindControl<StudioCanvasOverlay>("CanvasOverlay")!;
            var ruler = canvas.FindControl<Ruler>("HorizontalRuler")!;
            var container = canvas.FindControl<Control>("ContainerPanel")!;
            var zoom = canvas.FindControl<ZoomBorder>("PageZoomBorder")!;
            int count = editor.Project!.CurrentContainer!.CurrentLayer!.Shapes.Length;
            editor.Project.History.Reset();
            Point start = ruler.TranslatePoint(new Point(130, 10), window)!.Value;
            Point target = container.TranslatePoint(new Point(130, 120), window)!.Value;
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(target);
            Assert.Empty(overlay.Guides!.Items);
            window.MouseUp(target, MouseButton.Left);
            var guide = Assert.Single(overlay.Guides.Items);
            Assert.False(guide.IsVertical);
            Assert.InRange(Math.Abs(guide.Position - 120), 0, .01);
            Assert.Equal(count, editor.Project.CurrentContainer.CurrentLayer.Shapes.Length);
            Assert.True(editor.Project.History.Undo());
            Assert.Empty(overlay.Guides.Items);
            Assert.True(editor.Project.History.Redo());
            Assert.Single(overlay.Guides.Items);
            Point moved = target + new Vector(0, 25);
            window.MouseDown(target, MouseButton.Left, RawInputModifiers.Alt);
            window.MouseMove(moved, RawInputModifiers.Alt);
            window.MouseUp(moved, MouseButton.Left, RawInputModifiers.Alt);
            Assert.Equal(2, overlay.Guides.Items.Count);
            Assert.Equal(guide.Id, overlay.Guides.Items[0].Id);
            zoom.Focus();
            window.KeyPressQwerty(PhysicalKey.Delete, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Delete, RawInputModifiers.None);
            Assert.Single(overlay.Guides.Items);
            Assert.Equal(count, editor.Project.CurrentContainer.CurrentLayer.Shapes.Length);
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(target);
            window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            window.MouseUp(target, MouseButton.Left);
            Assert.Single(overlay.Guides.Items);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void SpacePanAndWheelZoomDoNotModifyTheDocument()
    {
        using var state = new AppState();
        var shape = StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        var canvas = new PageView { DataContext = editor };
        var window = new Window { Width = 900, Height = 700, Content = canvas };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var zoom = canvas.FindControl<ZoomBorder>("PageZoomBorder")!;
            var overlay = canvas.FindControl<StudioCanvasOverlay>("CanvasOverlay")!;
            var container = canvas.FindControl<Control>("ContainerPanel")!;
            Point point = zoom.TranslatePoint(new Point(250, 200), window)!.Value;
            zoom.Focus();
            double oldX = zoom.OffsetX;
            window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
            window.MouseDown(point, MouseButton.Left);
            window.MouseMove(point + new Vector(60, 25));
            window.MouseUp(point + new Vector(60, 25), MouseButton.Left);
            window.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
            Assert.InRange(Math.Abs(zoom.OffsetX - oldX - 60), 0, .01);
            Assert.Equal(44, shape.TopLeft!.X);
            double oldZoom = zoom.ZoomX;
            window.MouseWheel(point, new Vector(0, 1));
            Assert.Equal(oldZoom, zoom.ZoomX);
            Point local = window.TranslatePoint(point, overlay)!.Value;
            Point world = overlay.ToWorld(local)!.Value;
            window.MouseWheel(point, new Vector(0, 2), RawInputModifiers.Control);
            Point worldAfter = overlay.ToWorld(local)!.Value;
            Assert.True(zoom.ZoomX > oldZoom);
            Assert.InRange(Math.Abs(worldAfter.X - world.X), 0, .01);
            Assert.InRange(Math.Abs(worldAfter.Y - world.Y), 0, .01);
            overlay.Navigation!.ShowRulers = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(canvas.FindControl<Ruler>("HorizontalRuler")!.IsVisible);
            Assert.False(overlay.ShowGuides);
            Assert.Equal(44, shape.TopLeft.X);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void ReattachmentRestoresMeasurementsAndPageGuideSession()
    {
        using var state = new AppState();
        var shape = StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        var canvas = new PageView { DataContext = editor };
        var panel = new Panel { Children = { canvas } };
        var window = new Window { Width = 900, Height = 700, Content = panel };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var overlay = canvas.FindControl<StudioCanvasOverlay>("CanvasOverlay")!;
            var guides = overlay.Guides!;
            guides.Add(true, 44);
            panel.Children.Remove(canvas);
            Assert.Null(overlay.Navigation);
            panel.Children.Add(canvas);
            Dispatcher.UIThread.RunJobs();
            Assert.Same(guides, overlay.Guides);
            Assert.Single(overlay.Guides!.Items);
            shape.BottomRight!.X += 10;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(276, canvas.FindControl<Ruler>("HorizontalRuler")!.SelectionLength);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void LayerRowInlineRenameAndStateActionsShareDocumentHistory()
    {
        using var state = new AppState();
        var shape = StudioScenarioTests.Populate(state);
        var row = new StudioLayerRow { DataContext = shape };
        StudioEditContext.SetHistory(row, state.Editor!.Project!.History);
        var window = new Window { Width = 300, Height = 80, Content = row };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            Assert.NotNull(row.Editor);
            row.IsRenaming = true;
            Dispatcher.UIThread.RunJobs();
            var input = Assert.Single(row.GetVisualDescendants().OfType<TextBox>());
            Assert.True(input.Focus());
            input.Text = "Renamed card";
            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Assert.Equal("Renamed card", shape.Name);
            Assert.False(row.IsRenaming);
            Assert.True(state.Editor.Project.History.Undo());
            Assert.Equal("Feature card", shape.Name);
            row.Editor!.IsLocked = true;
            row.Editor.IsVisible = false;
            Assert.True(state.Editor.Project.History.Undo());
            Assert.True(row.Editor.IsVisible);
            Assert.True(row.Editor.IsLocked);
        }
        finally { window.Close(); }
    }
}
