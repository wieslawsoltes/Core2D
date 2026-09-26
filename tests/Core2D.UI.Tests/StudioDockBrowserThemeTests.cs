using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.ViewModels.Docking;
using Core2D.Views;
using Dock.Avalonia.Controls;
using Dock.Controls.ProportionalStackPanel;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Settings;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioDockBrowserThemeTests
{
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void NativeDockResourcesResolveToBrowserBrushes(bool dark)
    {
        var window = new Window { Width = 200, Height = 100, RequestedThemeVariant = Variant(dark) };
        try
        {
            window.Show(); Jobs();
            foreach (var (dock, browser) in new[]
            {
                ("DockThemeBackgroundBrush", "BrowserSurfaceBrush"),
                ("DockThemeControlBackgroundBrush", "BrowserInputBrush"),
                ("DockThemeBorderLowBrush", "BrowserBorderBrush"),
                ("DockThemeForegroundBrush", "BrowserTextBrush"),
                ("DockThemeAccentBrush", "BrowserSurfaceBrush"),
                ("DockApplicationAccentBrushLow", "BrowserSurfaceBrush"),
                ("DockApplicationAccentBrushMed", "BrowserHoverBrush"),
                ("DockApplicationAccentBrushHigh", "BrowserHoverBrush"),
                ("DockApplicationAccentForegroundBrush", "BrowserTextBrush"),
                ("DockApplicationAccentBrushIndicator", "BrowserAccentBrush"),
                ("DockToolChromeIconBrush", "BrowserMutedBrush"),
                ("DockWindowChromeBackgroundBrush", "BrowserSurfaceBrush"),
                ("DockWindowChromeBorderBrush", "BrowserBorderBrush"),
                ("DockWindowChromeTitleBarBackgroundBrush", "BrowserCanvasBrush"),
                ("DockWindowChromeForegroundBrush", "BrowserTextBrush")
            })
            {
                Assert.Same(window.FindResource(window.ActualThemeVariant, browser), window.FindResource(window.ActualThemeVariant, dock));
            }
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void OppositeThemeWindowsAndLiveSwitchesKeepNativeTemplates()
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var view = new MainView { DataContext = state.Editor };
        var first = Host(view, false);
        var title = new HostWindowTitleBar();
        var second = Host(title, true);
        try
        {
            first.Show(); second.Show(); Jobs();
            var tab = view.GetVisualDescendants().OfType<DocumentTabStripItem>().First();
            var template = tab.Template;
            var dockControl = view.GetVisualDescendants().OfType<DocumentControl>().First();
            var nativeTheme = Assert.IsType<ControlTheme>(first.FindResource(typeof(DocumentControl)));
            var nativeTemplate = nativeTheme.Setters.OfType<Setter>().Single(x => x.Property == TemplatedControl.TemplateProperty).Value;
            Assert.Same(nativeTemplate, dockControl.Template);
            Assert.NotEqual(Brush(first, "DockThemeBackgroundBrush"), Brush(second, "DockThemeBackgroundBrush"));
            Assert.Equal(Brush(second, "BrowserCanvasBrush"), Solid(title.Background));
            first.RequestedThemeVariant = ThemeVariant.Dark; Jobs();
            Assert.Same(template, tab.Template);
            Assert.Equal(Brush(second, "DockThemeBackgroundBrush"), Brush(first, "DockThemeBackgroundBrush"));
            second.RequestedThemeVariant = ThemeVariant.Light; Jobs();
            Assert.Equal(Brush(second, "BrowserCanvasBrush"), Solid(title.Background));
            Assert.NotEqual(Brush(first, "DockThemeForegroundBrush"), Brush(second, "DockThemeForegroundBrush"));
        }
        finally { second.Close(); first.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(DocumentTabLayout.Top, false)]
    [InlineData(DocumentTabLayout.Top, true)]
    [InlineData(DocumentTabLayout.Left, true)]
    [InlineData(DocumentTabLayout.Right, false)]
    public void DocumentTabsRetainNativeDragAndCloseContracts(DocumentTabLayout placement, bool dark)
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var factory = Assert.IsType<DockFactory>(state.Editor!.DockFactory);
        var pages = factory.PagesDock!;
        pages.TabsLayout = placement;
        var view = new MainView { DataContext = state.Editor };
        var window = Host(view, dark);
        try
        {
            window.Show(); Jobs();
            var document = view.GetVisualDescendants().OfType<DocumentControl>().Single(x => ReferenceEquals(x.DataContext, pages));
            var tab = document.GetVisualDescendants().OfType<DocumentTabStripItem>().First();
            var strip = document.GetVisualDescendants().OfType<DocumentTabStrip>().Single();
            var template = tab.Template;
            var expectedThickness = placement == DocumentTabLayout.Left ? new Thickness(0, 0, 2, 0)
                : placement == DocumentTabLayout.Right ? new Thickness(2, 0, 0, 0) : new Thickness(0, 0, 0, 2);
            Assert.Equal(expectedThickness, tab.BorderThickness);
            Assert.Equal(32d, tab.MinHeight);
            Assert.NotNull(tab.DocumentContextMenu);
            Assert.Same(pages.VisibleDockables, strip.ItemsSource);
            var close = tab.GetVisualDescendants().OfType<Button>().Single();
            Assert.Same(tab.DataContext, close.CommandParameter);
            Assert.NotNull(close.Command);
            Assert.Equal(24d, close.Width);
            Assert.Equal(24d, close.Height);
            Assert.Equal(0d, close.MinHeight);
            var tracking = tab.GetVisualDescendants().OfType<DockableControl>().Single();
            Assert.NotNull(tracking);
            var border = tab.GetVisualDescendants().OfType<Border>().First();
            Assert.Equal(new CornerRadius(6, 6, 0, 0), border.CornerRadius);
            Assert.True(DockProperties.GetIsDragArea(border));
            Assert.True(DockProperties.GetIsDropArea(border));
            Assert.True(DockProperties.GetIsDockTarget(border));
            tab.IsActive = true;
            tab.IsSelected = true; Jobs();
            Assert.Equal(Brush(tab, "DockTabActiveIndicatorBrush"), Solid(tab.BorderBrush));
            Assert.Equal(Brush(tab, "DockTabActiveBackgroundBrush"), Solid(tab.Background));
            var bounds = tab.Bounds.Size;
            Move(window, tab); Jobs();
            Assert.Equal(bounds, tab.Bounds.Size);
            Assert.Same(template, tab.Template);
            Assert.Equal(Brush(tab, "DockTabActiveForegroundBrush"), Solid(tab.Foreground));
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void ToolChromeAndTabSelectionWorkAcrossReattachment(bool dark)
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        editor.OnToggleDockableVisibility("ProjectExplorer");
        editor.OnToggleDockableVisibility("ObjectBrowser");
        var factory = Assert.IsType<DockFactory>(editor.DockFactory);
        var left = StudioDockGraph.Enumerate((IDockable)editor.RootDock!).OfType<IToolDock>().Single(x => x.Id == "StudioLeftDock");
        left.GripMode = GripMode.Visible;
        var layout = editor.RootDock;
        var children = left.VisibleDockables;
        var proportion = left.Proportion;
        var view = new MainView { DataContext = editor };
        var window = Host(view, dark);
        try
        {
            window.Show(); Jobs();
            for (int cycle = 0; cycle < 2; cycle++)
            {
                var chrome = view.GetVisualDescendants().OfType<ToolChromeControl>().Single(x => ReferenceEquals(x.DataContext, left));
                var grip = chrome.GetVisualDescendants().OfType<Grid>().Single(x => x.Name == "PART_Grip");
                Assert.True(grip.Bounds.Height >= 32);
                var close = chrome.GetVisualDescendants().OfType<Button>().Single(x => x.Name == "PART_CloseButton");
                var pin = chrome.GetVisualDescendants().OfType<Button>().Single(x => x.Name == "PART_PinButton");
                Assert.Equal(24d, pin.Width);
                Assert.NotNull(pin.Command);
                Assert.NotNull(close.Command);
                chrome.IsActive = true; Jobs();
                Assert.Equal(Brush(chrome, "DockSurfaceHeaderActiveBrush"), Solid(grip.Background));
                var tab = view.GetVisualDescendants().OfType<ToolTabStripItem>().Single(x => x.DataContext is IDockable d && d.Id == "ProjectExplorer");
                Click(window, tab); Jobs();
                Assert.Same(tab.DataContext, left.ActiveDockable);
                Assert.Equal(Brush(tab, "DockTabSelectedForegroundBrush"), Solid(tab.Foreground));
                Assert.NotNull(tab.TabContextMenu);
                Assert.Equal(new Thickness(10, 0), tab.Padding);
                window.Content = null;
                window.Content = view; Jobs();
                Assert.Same(layout, editor.RootDock);
                Assert.Same(children, left.VisibleDockables);
                Assert.Equal(proportion, left.Proportion);
            }
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void LocalTokenOverridesReachSplittersAndNativeDockTargets()
    {
        var splitter = new ProportionalStackPanelSplitter { Height = 80, Width = 8 };
        var target = new DockTarget { Width = 180, Height = 120 };
        var panel = new StackPanel { Children = { splitter, target } };
        var window = Host(panel, true);
        try
        {
            window.Show(); Jobs();
            Assert.Equal(Brush(splitter, "DockSplitterIdleBrush"), Solid(splitter.Background));
            Assert.Equal(4d, splitter.Thickness);
            var template = splitter.Template;
            var custom = new SolidColorBrush(Colors.OrangeRed);
            window.Resources["DockSplitterHoverBrush"] = custom;
            Move(window, splitter); Jobs();
            Assert.Same(custom, splitter.Background);
            Assert.Same(template, splitter.Template);
            foreach (var indicator in target.GetVisualDescendants().OfType<Panel>().Where(x => x.Name?.EndsWith("Indicator", StringComparison.Ordinal) == true))
                Assert.Equal(Brush(target, "BrowserAccentBrush"), Solid(indicator.Background));
            Assert.Equal(5, target.GetVisualDescendants().OfType<Panel>().Count(x => x.Name?.EndsWith("Indicator", StringComparison.Ordinal) == true));
            window.Resources["DockApplicationAccentBrushIndicator"] = custom; Jobs();
            Assert.All(target.GetVisualDescendants().OfType<Panel>().Where(x => x.Name?.EndsWith("Indicator", StringComparison.Ordinal) == true), p => Assert.Same(custom, p.Background));
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void PopulatedDockWorkspaceRendersWithBrowserChrome(bool dark)
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        editor.OnToggleDockableVisibility("ProjectExplorer");
        editor.OnToggleDockableVisibility("ObjectBrowser");
        editor.OnToggleDockableVisibility("ShapeProperties");
        var factory = Assert.IsType<DockFactory>(editor.DockFactory);
        foreach (var dock in StudioDockGraph.Enumerate((IDockable)editor.RootDock!).OfType<IToolDock>().Where(x => x.Id is "StudioLeftDock" or "StudioRightDock"))
            dock.GripMode = GripMode.Visible;
        factory.SetActiveDockable(factory.GetDockable<IDockable>("StudioNavigator")!);
        factory.SetActiveDockable(factory.GetDockable<IDockable>("StudioInspector")!);
        var view = new MainView { DataContext = editor };
        var window = Host(view, dark);
        try
        {
            window.Show(); Jobs();
            window.MouseMove(new Point(700, 890));
            Assert.True(view.GetVisualDescendants().OfType<ToolTabStripItem>().Count() >= 5);
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            string? directory = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
                frame!.Save(Path.Combine(directory, $"dock-browser-{(dark ? "dark" : "light")}.png"));
            }
        }
        finally { window.Close(); }
    }

    private static ThemeVariant Variant(bool dark) => dark ? ThemeVariant.Dark : ThemeVariant.Light;
    private static Window Host(Control view, bool dark) => new() { Width = 1440, Height = 900, Content = view, RequestedThemeVariant = Variant(dark) };
    private static Color Solid(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;
    private static Color Brush(Control control, string key) => Solid(Assert.IsAssignableFrom<IBrush>(control.FindResource(control.ActualThemeVariant, key)));
    private static void Jobs() => Dispatcher.UIThread.RunJobs();
    private static Point Center(Window window, Control control) => control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
    private static void Move(Window window, Control control) => window.MouseMove(Center(window, control));
    private static void Click(Window window, Control control)
    {
        Point point = Center(window, control);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
    }
}
