using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Core2D.ViewModels.Docking.Views;
using Core2D.Views.Docking.Views;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioDashboardTests
{
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void HomeRendersAndCreatesANewProjectFromKeyboard(bool dark)
    {
        using var state = new AppState();
        var view = new DashboardView { DataContext = new DashboardViewModel { Context = state.Editor } };
        var window = new Window
        {
            Width = 1200, Height = 760, Content = view,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.NotNull(view.FindControl<Button>("OpenProjectCard")!.Command);
            var button = view.FindControl<Button>("NewProjectCard")!;
            Assert.NotNull(button.Command);
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            var directory = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
                frame!.Save(Path.Combine(directory, $"home-{(dark ? "dark" : "light")}.png"));
            }
            var original = state.Editor!.Project;
            button.Focus();
            window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
            Assert.NotNull(state.Editor.Project);
            Assert.NotSame(original, state.Editor.Project);
        }
        finally
        {
            window.Close();
        }
    }
}
