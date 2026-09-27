using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace Avalonia.Themes.Browser.Tests;

public class BrowserAuxiliaryTests
{
    public static IEnumerable<object[]> Variants()
    {
        for (int kind = 0; kind < 10; kind++)
        {
            yield return new object[] { kind, false };
            yield return new object[] { kind, true };
        }
    }

    [AvaloniaTheory]
    [MemberData(nameof(Variants))]
    public void AuxiliaryControlsMaterializeImportedTemplates(int kind, bool dark)
    {
        // Explicit factories avoid reflection and exercise templates beyond the main gallery.
        TemplatedControl control = kind switch
        {
            0 => new RefreshContainer { Content = new ScrollViewer { Content = new TextBlock { Text = "Refresh content" } } },
            1 => new RefreshVisualizer(),
            2 => new DatePickerPresenter(),
            3 => new TimePickerPresenter(),
            4 => new TransitioningContentControl { Content = "Transition content" },
            5 => new MenuFlyoutPresenter { Items = { new MenuItem { Header = "Action" } } },
            6 => new FlyoutPresenter { Content = "Popover content" },
            7 => new WindowNotificationManager(),
            8 => new Carousel { Items = { new TextBlock { Text = "First" }, new TextBlock { Text = "Second" } } },
            9 => new ButtonSpinner { Content = new TextBox { Text = "12" } },
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        var window = new Window { Width = 600, Height = 500, Content = control,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            Assert.NotNull(control.Template);
            Assert.True(control.Bounds.Width > 0);
            Assert.NotEmpty(control.GetVisualDescendants());
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void DisabledTextKeepsASeparateReadableSurface(bool dark)
    {
        var input = new TextBox { Text = "Disabled input", IsEnabled = false };
        var window = new Window { Width = 340, Height = 100, Content = input,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var surface = input.GetVisualDescendants().OfType<Border>().Single(x => x.Name == "PART_BorderElement");
            var background = Assert.IsAssignableFrom<ISolidColorBrush>(surface.Background).Color;
            var foreground = Assert.IsAssignableFrom<ISolidColorBrush>(input.Foreground).Color;
            Assert.NotEqual(background, foreground);
            Assert.Equal(Color.Parse(dark ? "#383838" : "#F5F5F5"), background);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void ComboBoxPopupAndFlyoutRetainNativeOpenAndDismiss(bool dark)
    {
        var combo = new ComboBox { ItemsSource = new[] { "Design", "Data", "State" }, SelectedIndex = 0 };
        var flyout = new Flyout { Content = new TextBox { Text = "Popup text" } };
        var button = new Button { Content = "Open", Flyout = flyout };
        var window = new Window { Width = 440, Height = 320,
            Content = new StackPanel { Spacing = 12, Children = { combo, button } },
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            combo.Focus(); combo.IsDropDownOpen = true; Dispatcher.UIThread.RunJobs();
            Assert.True(combo.IsDropDownOpen);
            combo.SelectedIndex = 2; combo.IsDropDownOpen = false;
            Assert.Equal("State", combo.SelectedItem);
            flyout.ShowAt(button); Dispatcher.UIThread.RunJobs(); Assert.True(flyout.IsOpen);
            flyout.Hide(); Dispatcher.UIThread.RunJobs(); Assert.False(flyout.IsOpen);
            Assert.Equal("Popup text", Assert.IsType<TextBox>(flyout.Content).Text);
        }
        finally { window.Close(); }
    }
}
