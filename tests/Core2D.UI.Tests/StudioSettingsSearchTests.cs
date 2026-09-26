using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.Views.Containers;
using Core2D.Views.Renderer;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioSettingsSearchTests
{
    [AvaloniaFact]
    public void KeywordFilteringKeepsEditorValuesAndExpansionState()
    {
        var input = new TextBox { Text = "25" };
        var first = new StudioSettingsSection { Header = "Geometry", Keywords = "points selection anchors", IsExpanded = true, Content = input };
        var second = new StudioSettingsSection { Header = "Appearance", Keywords = "color fill stroke", IsExpanded = false };
        var panel = new StudioSettingsPanel { Items = { first, second } };
        var window = new Window { Width = 300, Height = 360, Content = panel };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs(); Assert.Equal(2, panel.MatchCount);
            panel.Query = "FILL color"; Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, panel.MatchCount);
            Assert.False(first.IsVisible); Assert.True(second.IsVisible);
            Assert.True(first.IsExpanded); Assert.False(second.IsExpanded);
            Assert.Equal("25", input.Text);
            panel.Query = "nonexistent"; Dispatcher.UIThread.RunJobs(); Assert.Equal(0, panel.MatchCount);
            panel.Query = ""; Dispatcher.UIThread.RunJobs(); Assert.Equal(2, panel.MatchCount);
            first.Header = "Coordinates"; panel.Query = "coordinates"; Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, panel.MatchCount);
            window.Content = null; Assert.True(first.MatchesQuery); Assert.True(second.MatchesQuery);
            window.Content = panel; Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, panel.MatchCount);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false, false)] [InlineData(true, false)]
    [InlineData(false, true)] [InlineData(true, true)]
    public void RealSettingsViewsRenderAndFilterWithoutChangingTheModel(bool dark, bool renderer)
    {
        using var state = new AppState(); StudioScenarioTests.Populate(state);
        Control view = renderer ? new ShapeRendererStateView { DataContext = state.Editor!.PageState } : new OptionsView { DataContext = state.Editor!.Project!.Options };
        var window = new Window { Width = 360, Height = 640, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var panel = Assert.Single(view.GetVisualDescendants().OfType<StudioSettingsPanel>());
            Assert.True(panel.MatchCount >= 2);
            int original = panel.MatchCount;
            panel.Query = "___no_match___"; Dispatcher.UIThread.RunJobs(); Assert.Equal(0, panel.MatchCount);
            panel.Query = ""; Dispatcher.UIThread.RunJobs(); Assert.Equal(original, panel.MatchCount);
            StudioSecondaryViewTests.Capture(window, $"{(renderer ? "renderer-settings" : "drawing-settings")}-{(dark ? "dark" : "light")}.png");
        }
        finally { window.Close(); }
    }
}
