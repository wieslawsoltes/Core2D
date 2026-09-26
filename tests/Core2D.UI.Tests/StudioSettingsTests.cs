using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Docking.Tools.Options;
using Core2D.Views.Docking.Tools.Options;
using Xunit;
using CanvasView = Core2D.Controls.Editor.PageView;

namespace Core2D.UI.Tests;

public class StudioSettingsTests
{
    [AvaloniaFact]
    public void ViewportSettingsRetainNativeModelAndAllCommands()
    {
        using var state = new AppState();
        StudioScenarioTests.Populate(state);
        var canvas = new CanvasView { DataContext = state.Editor };
        var settings = new ZoomOptionsView { DataContext = new ZoomOptionsViewModel { Context = state.Editor } };
        var layout = new Grid { ColumnDefinitions = new ColumnDefinitions("*,320") };
        layout.Children.Add(canvas);
        layout.Children.Add(settings);
        Grid.SetColumn(settings, 1);
        var window = new Window { Width = 1000, Height = 720, Content = layout };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var sections = settings.GetVisualDescendants().OfType<StudioSection>().ToArray();
            Assert.Equal(3, sections.Length);
            var buttons = settings.GetVisualDescendants().OfType<Button>().Where(x => x.Command is not null).ToArray();
            Assert.Equal(5, buttons.Length);
            var speed = Assert.Single(settings.GetVisualDescendants().OfType<StudioNumericField>());
            speed.DraftText = "1.5";
            Assert.True(speed.TryCommit());
            var zoom = canvas.FindControl<Avalonia.Controls.PanAndZoom.ZoomBorder>("PageZoomBorder")!;
            Assert.Equal(1.5, zoom.ZoomSpeed);
            sections.Single(x => Equals(x.Header, "Constraints")).IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(8, settings.GetVisualDescendants().OfType<TextBox>().Count(x => x.GetVisualAncestors().OfType<StudioPropertyField>().Any()));
            StudioSecondaryViewTests.Capture(window, "viewport-settings.png");
        }
        finally { window.Close(); }
    }
}
