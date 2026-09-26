using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Views;
using Dock.Avalonia.Controls;
using Dock.Model.Core;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioDockTabMetricsTests
{
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void SelectingAndHoveringToolTabsDoesNotMoveTheirLabels(bool dark)
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        editor.OnToggleDockableVisibility("ProjectExplorer");
        editor.OnToggleDockableVisibility("ObjectBrowser");
        var view = new MainView { DataContext = editor };
        var window = new Window { Width = 1440, Height = 900, Content = view,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var tab = view.GetVisualDescendants().OfType<ToolTabStripItem>().Single(x =>
                x.DataContext is IDockable item && item.Id == "ProjectExplorer");
            var model = (IDockable)tab.DataContext!;
            var title = tab.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Text == model.Title);
            tab.IsSelected = false;
            Dispatcher.UIThread.RunJobs();
            var before = title.TranslatePoint(default, tab);
            var size = tab.Bounds.Size;
            Assert.Equal(new Thickness(0, 0, 0, 2), tab.BorderThickness);
            tab.IsSelected = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(new Thickness(0, 0, 0, 2), tab.BorderThickness);
            Assert.Equal(new Thickness(10, 0), tab.Padding);
            Assert.Equal(before, title.TranslatePoint(default, tab));
            Assert.Equal(size, tab.Bounds.Size);
            Point center = tab.TranslatePoint(new Point(tab.Bounds.Width / 2, tab.Bounds.Height / 2), window)!.Value;
            window.MouseMove(center);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(before, title.TranslatePoint(default, tab));
            Assert.Equal(size, tab.Bounds.Size);
        }
        finally { window.Close(); }
    }
}
