using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Views;
using Dock.Avalonia.Controls;
using Dock.Model.Core;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioDockTests
{
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void SelectedAndInactiveToolTabsUseReadableThemeColors(bool dark)
    {
        using var state = new AppState();
        var editor = state.Editor!;
        editor.OnNewProject();
        editor.OnToggleDockableVisibility("ProjectExplorer");
        editor.OnToggleDockableVisibility("ObjectBrowser");
        editor.OnToggleDockableVisibility("ShapeProperties");
        var view = new MainView { DataContext = editor };
        var window = new Window
        {
            Width = 1440,
            Height = 900,
            Content = view,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var tabs = view.GetVisualDescendants().OfType<ToolTabStripItem>().ToArray();
            Assert.True(tabs.Length >= 5, "The real workspace must expose its docked tool tabs.");
            Assert.Contains(tabs, tab => tab.IsSelected);
            Assert.Contains(tabs, tab => !tab.IsSelected);
            foreach (var tab in tabs)
            {
                var expected = Assert.IsAssignableFrom<ISolidColorBrush>(tab.FindResource(
                    tab.IsSelected ? "DockTabSelectedForegroundBrush" : "DockTabForegroundBrush")).Color;
                Assert.Equal(expected, Assert.IsAssignableFrom<ISolidColorBrush>(tab.Foreground).Color);
                var dockable = Assert.IsAssignableFrom<IDockable>(tab.DataContext);
                var title = Assert.Single(tab.GetVisualDescendants().OfType<TextBlock>().Where(text => text.Text == dockable.Title));
                Assert.Equal(expected, Assert.IsAssignableFrom<ISolidColorBrush>(title.Foreground).Color);
            }
        }
        finally
        {
            window.Close();
        }
    }
}
