using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Editor;
using Core2D.Views;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioActionsTests
{
    [AvaloniaFact]
    public void BrowsingDoesNotExecuteAndDisabledActionsAreRechecked()
    {
        int executions = 0;
        bool enabled = false;
        object? received = null;
        object parameter = new();
        var command = new RelayCommand<object>(value => { executions++; received = value; }, _ => enabled);
        var item = new MenuItem { Header = "_Export drawing", Command = command, CommandParameter = parameter };
        var palette = new StudioCommandPalette { Items = { new MenuItem { Header = "File", Items = { item } } } };
        var focus = new Button { Content = "Previous focus" };
        var window = new Window { Width = 700, Height = 600, Content = new Grid { Children = { focus, palette } } };
        try
        {
            window.Show(); focus.Focus(); palette.IsOpen = true; Dispatcher.UIThread.RunJobs();
            var grid = Assert.Single(palette.GetVisualDescendants().OfType<DataGrid>());
            Assert.Equal(1, palette.ResultCount);
            Assert.False(Assert.Single(grid.ItemsSource!.Cast<StudioActionViewModel>()).IsAvailable);
            Assert.Equal(0, executions);
            Press(window, PhysicalKey.Enter);
            Assert.Equal(0, executions); Assert.True(palette.IsOpen);
            enabled = true; command.NotifyCanExecuteChanged(); Dispatcher.UIThread.RunJobs();
            Assert.True(Assert.Single(grid.ItemsSource!.Cast<StudioActionViewModel>()).IsAvailable);
            palette.Query = "file drawing"; Dispatcher.UIThread.RunJobs(); Assert.Equal(1, palette.ResultCount);
            palette.Query = "missing"; Dispatcher.UIThread.RunJobs(); Assert.Equal(0, palette.ResultCount);
            palette.Query = "export"; Dispatcher.UIThread.RunJobs();
            Press(window, PhysicalKey.Enter);
            Assert.Equal(1, executions); Assert.Same(parameter, received); Assert.False(palette.IsOpen);
            Assert.True(focus.IsFocused);
            palette.IsOpen = true; Dispatcher.UIThread.RunJobs();
            enabled = false; // No notification: execution still must recheck the actual command.
            Press(window, PhysicalKey.Enter);
            Assert.Equal(1, executions); Assert.True(palette.IsOpen);
            Press(window, PhysicalKey.Escape);
            Assert.False(palette.IsOpen); Assert.True(focus.IsFocused);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false, 1100)] [InlineData(true, 1100)] [InlineData(true, 360)]
    public void RealWorkspaceActionsUseCompiledCommandsAndRemainWithinViewport(bool dark, int width)
    {
        using var state = new AppState(); StudioScenarioTests.Populate(state);
        var view = new MainView { DataContext = state.Editor };
        var window = new Window { Width = width, Height = 720, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var palette = view.FindControl<StudioActionsView>("QuickActionsPalette")!;
            palette.IsOpen = true; Dispatcher.UIThread.RunJobs();
            Assert.True(palette.ResultCount > 40, $"Only {palette.ResultCount} actions were bound.");
            var grid = Assert.Single(palette.GetVisualDescendants().OfType<DataGrid>());
            Assert.Contains(grid.ItemsSource!.Cast<StudioActionViewModel>(), action => action.IsAvailable);
            var surface = palette.GetVisualDescendants().OfType<Border>().Single(x => x.Name == "PART_Surface");
            Assert.True(surface.Bounds.Width <= width - 30);
            palette.Query = "align"; Dispatcher.UIThread.RunJobs();
            Assert.True(palette.ResultCount >= 4);
            StudioSecondaryViewTests.Capture(window, $"actions-{(dark ? "dark" : "light")}-{width}.png");
            palette.Query = "tool rectangle"; Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, palette.ResultCount);
            var action = Assert.Single(grid.ItemsSource!.Cast<StudioActionViewModel>());
            Assert.True(action.IsAvailable);
            // Capture can move focus indirectly; explicitly return to the native search input.
            palette.GetVisualDescendants().OfType<TextBox>().Single(x => x.Name == "PART_Search").Focus();
            Press(window, PhysicalKey.Enter);
            Assert.False(palette.IsOpen);
            Assert.Equal("Rectangle", state.Editor!.CurrentTool?.Title);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void WorkspaceShortcutAndLauncherRestoreFocusAndRespectDialogs()
    {
        using var state = new AppState(); StudioScenarioTests.Populate(state);
        var view = new MainView { DataContext = state.Editor };
        var window = new Window { Width = 1100, Height = 720, Content = view };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var palette = view.FindControl<StudioActionsView>("QuickActionsPalette")!;
            var canvas = view.GetVisualDescendants().OfType<ZoomBorder>().First();
            Assert.True(canvas.Focus());
            var modifiers = OperatingSystem.IsMacOS() ? RawInputModifiers.Meta : RawInputModifiers.Control;
            window.KeyPressQwerty(PhysicalKey.K, modifiers);
            window.KeyReleaseQwerty(PhysicalKey.K, modifiers);
            Dispatcher.UIThread.RunJobs(); Assert.True(palette.IsOpen);
            Press(window, PhysicalKey.Escape); Assert.True(canvas.IsFocused);
            var launch = view.GetVisualDescendants().OfType<StudioIconButton>().Single(x => x.Name == "QuickActionsButton");
            var point = launch.TranslatePoint(new Point(launch.Bounds.Width / 2, launch.Bounds.Height / 2), window)!.Value;
            window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs(); Assert.True(palette.IsOpen);
            palette.CanOpen = false; Assert.False(palette.IsOpen);
            palette.IsOpen = true; Assert.False(palette.IsOpen);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void ReattachmentAndCommandReplacementDoNotRetainOldActions()
    {
        int first = 0, second = 0;
        var item = new MenuItem { Header = "Run", Command = new RelayCommand(() => first++) };
        var palette = new StudioCommandPalette { Items = { item } };
        var window = new Window { Width = 640, Height = 480, Content = palette };
        try
        {
            window.Show(); palette.IsOpen = true; Dispatcher.UIThread.RunJobs();
            item.Command = new RelayCommand(() => second++); Dispatcher.UIThread.RunJobs();
            Press(window, PhysicalKey.Enter);
            Assert.Equal(0, first); Assert.Equal(1, second);
            window.Content = null; window.Content = palette;
            palette.IsOpen = true; Dispatcher.UIThread.RunJobs();
            Press(window, PhysicalKey.Enter);
            Assert.Equal(2, second);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void NativePreeditOwnsConfirmationKeys()
    {
        int count = 0;
        var palette = new StudioCommandPalette { Items = { new MenuItem { Header = "Run", Command = new RelayCommand(() => count++) } } };
        var window = new Window { Width = 640, Height = 480, Content = palette };
        try
        {
            window.Show(); palette.IsOpen = true; Dispatcher.UIThread.RunJobs();
            var search = palette.GetVisualDescendants().OfType<TextBox>().Single(x => x.Name == "PART_Search");
            var presenter = Assert.Single(search.GetVisualDescendants().OfType<TextPresenter>());
            presenter.PreeditText = "composition";
            Press(window, PhysicalKey.Enter);
            Assert.Equal(0, count); Assert.True(palette.IsOpen);
            presenter.PreeditText = null;
            Press(window, PhysicalKey.Escape); Assert.False(palette.IsOpen);
        }
        finally { window.Close(); }
    }

    private static void Press(Window window, PhysicalKey key)
    {
        window.KeyPressQwerty(key, RawInputModifiers.None);
        window.KeyReleaseQwerty(key, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
    }
}
