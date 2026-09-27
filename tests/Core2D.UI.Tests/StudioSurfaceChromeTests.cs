using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioSurfaceChromeTests
{
    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void ScrollThumbRetainsNativeDragAndRangeContract(bool horizontal)
    {
        var bar = new ScrollBar { Minimum = 0, Maximum = 1000, Value = 200, ViewportSize = 200,
            Orientation = horizontal ? Orientation.Horizontal : Orientation.Vertical };
        var window = new Window { Width = horizontal ? 400 : 80, Height = horizontal ? 80 : 400, Content = bar };
        try
        {
            window.Show(); Jobs();
            var thumb = bar.GetVisualDescendants().OfType<Thumb>().Single();
            Point start = thumb.TranslatePoint(new Point(thumb.Bounds.Width / 2, thumb.Bounds.Height / 2), window)!.Value;
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + (horizontal ? new Vector(45, 0) : new Vector(0, 45)));
            window.MouseUp(start + (horizontal ? new Vector(45, 0) : new Vector(0, 45)), MouseButton.Left);
            Assert.InRange(bar.Value, 201, 1000);
            bar.Value = 10000; Assert.Equal(1000, bar.Value);
            Assert.Equal(12, horizontal ? bar.Bounds.Height : bar.Bounds.Width);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void ScrollViewerOffsetAndContentSizeRemainConnected()
    {
        var content = new Border { Width = 900, Height = 1200 };
        var scroll = new ScrollViewer { Content = content, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        var window = new Window { Width = 300, Height = 240, Content = scroll };
        try
        {
            window.Show(); Jobs();
            scroll.Offset = new Vector(100, 200); Jobs();
            var bars = scroll.GetVisualDescendants().OfType<ScrollBar>().ToArray();
            Assert.Equal(100, bars.Single(x => x.Orientation == Orientation.Horizontal).Value);
            Assert.Equal(200, bars.Single(x => x.Orientation == Orientation.Vertical).Value);
            content.Height = 100; content.Width = 100; Jobs();
            Assert.Equal(default(Vector), scroll.Offset);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void SharedChoicesReadoutsAndNoticesRenderWithNativeSelection(bool dark)
    {
        var choice = new ComboBox { PlaceholderText = "Choose a mode", ItemsSource = new[] { "Design", "Data", "State" }, HorizontalAlignment = HorizontalAlignment.Stretch };
        var readout = new StudioReadout { Label = "Viewport", Value = "125% · 1440 × 900" };
        var notice = new StudioNotice { Title = "Original objects", Message = "Editing updates the existing document. Search and sorting never reorder the source collection." };
        var toggle = new StudioSettingToggle { Content = "Show details", Description = "The setting keeps the native keyboard and toggle behavior.", IsChecked = null };
        var panel = new StackPanel { Spacing = 14, Margin = new Thickness(16), Children = { new TextBlock { Text = "SHARED SURFACES" }, choice, toggle, readout, notice } };
        var window = new Window { Width = 360, Height = 500, Content = new StudioSurface { Content = panel }, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Jobs();
            var placeholder = choice.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Name == "PlaceholderTextBlock");
            Assert.True(placeholder.IsVisible); Assert.Equal("Choose a mode", placeholder.Text);
            choice.SelectedIndex = 1; Jobs(); Assert.False(placeholder.IsVisible);
            Assert.Equal("Data", choice.SelectedItem);
            readout.Value = "150% · 1440 × 900"; Jobs();
            Assert.Equal(readout.Value, readout.GetVisualDescendants().OfType<SelectableTextBlock>().Single().Text);
            Assert.True(toggle.Focus()); window.KeyPressQwerty(PhysicalKey.Space, RawInputModifiers.None); window.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
            Assert.False(toggle.IsChecked);
            var item = new ComboBoxItem { Content = "Selected mode", IsSelected = true };
            panel.Children.Add(item); Jobs();
            Assert.True(item.GetVisualDescendants().OfType<Avalonia.Controls.Shapes.Path>().Single(x => x.Name == "PART_Checkmark").IsVisible);
            StudioSecondaryViewTests.Capture(window, $"shared-surfaces-{(dark ? "dark" : "light")}.png");
        }
        finally { window.Close(); }
    }

    private static void Jobs() => Dispatcher.UIThread.RunJobs();
}
