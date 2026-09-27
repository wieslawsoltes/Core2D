using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Core2D.Controls.Studio;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioControlTests
{
    [AvaloniaFact]
    public void IconButtonExecutesCommandFromKeyboardAndHonoursCanExecute()
    {
        var executions = 0;
        var enabled = true;
        var command = new RelayCommand(() => executions++, () => enabled);
        var button = new StudioIconButton { Command = command, Content = "Save" };
        var window = new Window { Width = 400, Height = 200, Content = button };
        try
        {
            window.Show();
            Assert.True(button.Focus());
            window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
            Assert.Equal(1, executions);
            enabled = false;
            command.NotifyCanExecuteChanged();
            Dispatcher.UIThread.RunJobs();
            Assert.False(button.IsEffectivelyEnabled);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void ToolButtonsRetainRadioSelectionSemantics()
    {
        var first = new StudioToolButton { GroupName = "Tools", IsChecked = true };
        var second = new StudioToolButton { GroupName = "Tools" };
        var window = new Window { Content = new StackPanel { Children = { first, second } } };
        try
        {
            window.Show();
            second.IsChecked = true;
            Assert.False(first.IsChecked);
            Assert.True(second.IsChecked);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void PropertyFieldPreservesHostedTextEditor()
    {
        var input = new TextBox();
        var field = new StudioPropertyField { Label = "Name", Content = input };
        var window = new Window { Width = 280, Height = 200, Content = field };
        try
        {
            window.Show();
            input.Focus();
            window.KeyTextInput("Drawing 01");
            Assert.Equal("Drawing 01", input.Text);
            Assert.Same(input, field.Content);
            Assert.True(input.Bounds.Height >= 28);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void ControlsRenderInBothThemes(bool dark)
    {
        var section = new StudioSection
        {
            Header = "Design",
            Content = new StudioPropertyField { Label = "Name", Content = new TextBox { Text = "Untitled drawing" } }
        };
        var surface = new StudioSurface
        {
            Padding = new Thickness(12),
            Content = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    section,
                    new StudioIconButton { Content = "Export", Icon = Geometry.Parse("M0,8 L8,0 L16,8 L10,8 L10,16 L6,16 L6,8 Z") },
                    new StudioToolShelf
                    {
                        Content = new StackPanel
                        {
                            Orientation = Avalonia.Layout.Orientation.Horizontal,
                            Children = { new StudioToolButton { IsChecked = true }, new StudioToolButton() }
                        }
                    }
                }
            }
        };
        var window = new Window
        {
            Width = 360,
            Height = 360,
            Content = surface,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.True(section.IsExpanded);
            Assert.True(surface.Bounds.Width > 0);
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            Assert.True(frame!.PixelSize.Width >= 360);
            var directory = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
                frame.Save(Path.Combine(directory, dark ? "controls-dark.png" : "controls-light.png"));
            }
        }
        finally
        {
            window.Close();
        }
    }
}
