using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Editor;
using Core2D.Controls.Studio;
using Core2D.Views;
using Dock.Avalonia.Controls;
using Dock.Model.Core;
using Xunit;

namespace Core2D.UI.Tests;

public class WorkspaceTests
{
    [AvaloniaTheory]
    [InlineData(false, 1440, 900)]
    [InlineData(true, 1440, 900)]
    [InlineData(true, 1024, 768)]
    public void ActualWorkspaceRetainsDockingAndRenders(bool dark, int width, int height)
    {
        using var state = new AppState();
        var editor = state.Editor;
        Assert.NotNull(editor);
        editor!.OnNewProject();
        editor.Project!.Name = "Studio workspace";
        var view = new MainView { DataContext = editor };
        var window = new Window
        {
            Width = width,
            Height = height,
            Content = view,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.NotNull(editor.Project);
            var dock = Assert.Single(view.GetVisualDescendants().OfType<DockControl>().Where(x => ReferenceEquals(x.Layout, editor.RootDock)));
            Assert.True(dock.Bounds.Width > 0);
            Assert.NotEmpty(view.GetVisualDescendants().OfType<WorkspaceHeader>());
            Assert.NotEmpty(view.GetVisualDescendants().OfType<ToolsView>());
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            Assert.Equal(width, frame!.PixelSize.Width);
            var directory = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
                frame.Save(Path.Combine(directory, $"workspace-{(dark ? "dark" : "light")}-{width}.png"));
            }

            // GetDockable resolves registered locator aliases (Root/Pages/Home), not arbitrary IDs.
            // Inspect the actual layout tree to verify all sixteen existing panels are retained.
            var dockables = EnumerateDockables(Assert.IsAssignableFrom<IDockable>(editor.RootDock)).ToArray();
            foreach (var id in new[] { "ProjectExplorer", "PageProperties", "ShapeProperties", "StyleProperties", "DataProperties", "StateProperties", "StyleLibrary", "BlockLibrary", "DatabaseLibrary", "TemplateLibrary", "ScriptLibrary", "ProjectOptions", "RendererOptions", "ZoomOptions", "ImageOptions", "ObjectBrowser" })
            {
                Assert.Contains(dockables, dockable => dockable.Id == id);
            }
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void EveryDrawingToolAndPathSubtoolRemainsBound()
    {
        using var state = new AppState();
        var editor = state.Editor!;
        editor.OnNewProject();
        var view = new ToolsView { DataContext = editor };
        var window = new Window { Width = 1000, Height = 180, Content = view };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var tools = view.GetVisualDescendants().OfType<StudioToolButton>().ToArray();
            Assert.Equal(17, tools.Length);
            Assert.All(tools, button =>
            {
                Assert.NotNull(button.Command);
                Assert.NotNull(button.Icon);
                Assert.False(string.IsNullOrWhiteSpace(AutomationProperties.GetName(button)));
            });
            var rectangle = tools.Single(x => AutomationProperties.GetName(x) == "Rectangle tool");
            rectangle.Focus();
            window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
            Assert.Equal("Rectangle", editor.CurrentTool?.Title);
            Assert.True(rectangle.IsChecked);
            var path = tools.Single(x => AutomationProperties.GetName(x) == "Path tool");
            path.Command!.Execute(path.CommandParameter);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Path", editor.CurrentTool?.Title);
            var move = tools.Single(x => AutomationProperties.GetName(x) == "Path move");
            Assert.True(move.IsEffectivelyVisible);
            move.Command!.Execute(move.CommandParameter);
            Assert.Equal("Move", editor.CurrentPathTool?.Title);
            Assert.Equal("Path", editor.CurrentTool?.Title);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void ToolShelfScrollsInsteadOfClippingInNarrowPanes()
    {
        using var state = new AppState();
        var view = new ToolsView { DataContext = state.Editor };
        var window = new Window { Width = 240, Height = 100, Content = view };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var scroll = view.GetVisualDescendants().OfType<ScrollViewer>().Single(x => x.Name == "PART_ScrollViewer");
            Assert.True(scroll.Extent.Width > scroll.Viewport.Width);
            scroll.Offset = new Vector(scroll.Extent.Width, 0);
            Dispatcher.UIThread.RunJobs();
            Assert.True(scroll.Offset.X > 0);
            Assert.True(view.Bounds.Width <= window.ClientSize.Width);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void SplitDocumentToolShelvesKeepSelectionSynchronized()
    {
        using var state = new AppState();
        var editor = state.Editor!;
        var first = new ToolsView { DataContext = editor };
        var second = new ToolsView { DataContext = editor };
        var window = new Window { Width = 1000, Height = 240, Content = new StackPanel { Children = { first, second } } };
        try
        {
            window.Show();
            editor.OnToolRectangle();
            Dispatcher.UIThread.RunJobs();
            foreach (var view in new[] { first, second })
            {
                var rectangle = view.GetVisualDescendants().OfType<StudioToolButton>().Single(x => AutomationProperties.GetName(x) == "Rectangle tool");
                Assert.True(rectangle.IsChecked);
            }
        }
        finally
        {
            window.Close();
        }
    }

    private static IEnumerable<IDockable> EnumerateDockables(IDockable dockable)
    {
        yield return dockable;
        if (dockable is Dock.Model.Controls.IRootDock { HiddenDockables: { } hidden })
        {
            foreach (var item in hidden)
            {
                yield return item;
            }
        }
        if (dockable is IDock { VisibleDockables: { } children })
        {
            foreach (var child in children)
            {
                foreach (var descendant in EnumerateDockables(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}
