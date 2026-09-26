using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioPresentationTests
{
    [AvaloniaFact]
    public void InspectorSectionCollapsesAndExpandsWithKeyboard()
    {
        var content = new TextBox { Text = "Editable content" };
        var section = new StudioSection { Header = "Geometry", Content = content };
        var window = new Window { Width = 280, Height = 200, Content = section };
        try
        {
            window.Show();
            var header = section.GetVisualDescendants().OfType<ToggleButton>().Single(x => x.Name == "ExpanderHeader");
            Assert.True(section.IsExpanded);
            Assert.True(content.IsEffectivelyVisible);
            Assert.True(header.Focus());
            window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();
            Assert.False(section.IsExpanded);
            Assert.False(content.IsEffectivelyVisible);
            window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();
            Assert.True(section.IsExpanded);
            Assert.True(content.IsEffectivelyVisible);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void SurfaceAndTextUpdateWhenTheThemeChangesAtRuntime()
    {
        var text = new TextBlock { Text = "Theme-aware content" };
        var surface = new StudioSurface { Content = text };
        var window = new Window { Width = 280, Height = 200, Content = surface, RequestedThemeVariant = ThemeVariant.Light };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Color.Parse("#FFFFFF"), Assert.IsAssignableFrom<ISolidColorBrush>(surface.Background).Color);
            Assert.Equal(Color.Parse("#242424"), Assert.IsAssignableFrom<ISolidColorBrush>(text.Foreground).Color);
            window.RequestedThemeVariant = ThemeVariant.Dark;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Color.Parse("#2C2C2C"), Assert.IsAssignableFrom<ISolidColorBrush>(surface.Background).Color);
            Assert.Equal(Color.Parse("#F5F5F5"), Assert.IsAssignableFrom<ISolidColorBrush>(text.Foreground).Color);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void HeaderShowsUnsavedDocumentNameAndKeepsEveryTopLevelMenu()
    {
        using var state = new AppState();
        var editor = state.Editor!;
        editor.OnNewProject();
        editor.Project!.Name = "Untitled prototype";
        var header = new WorkspaceHeader { DataContext = editor };
        var window = new Window { Width = 1024, Height = 180, Content = header };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Untitled prototype", header.FindControl<TextBlock>("DocumentTitle")!.Text);
            Assert.NotNull(header.FindControl<StudioIconButton>("UndoButton")!.Command);
            Assert.NotNull(header.FindControl<StudioIconButton>("RedoButton")!.Command);
            Assert.NotNull(header.FindControl<StudioIconButton>("SaveButton")!.Command);
            var export = header.FindControl<Button>("ExportButton")!;
            Assert.NotNull(export.Command);
            var menuButton = header.GetVisualDescendants().OfType<StudioIconButton>().Single(x => x.Flyout is Flyout);
            var flyout = (Flyout)menuButton.Flyout!;
            flyout.ShowAt(menuButton);
            Dispatcher.UIThread.RunJobs();
            var menu = ((Control)flyout.Content!).GetVisualDescendants().OfType<Menu>().Single();
            Assert.True(menu.Items.Count >= 5);
            Assert.All(menu.Items.OfType<MenuItem>(), item => Assert.NotNull(item.Header));
            editor.Project.Name = "Renamed prototype";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Renamed prototype", header.FindControl<TextBlock>("DocumentTitle")!.Text);
        }
        finally
        {
            window.Close();
        }
    }
}
