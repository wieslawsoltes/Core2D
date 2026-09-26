using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace Avalonia.Themes.Browser.Tests;

public class BrowserVisualStateTests
{
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void SelectionIndicatorsDoNotTintTheirLabels(bool dark)
    {
        var checkbox = new CheckBox { Content = "Include source", IsChecked = true, IsThreeState = true };
        var radio = new RadioButton { Content = "Personal", IsChecked = true };
        var window = new Window { Width = 320, Height = 160,
            Content = new StackPanel { Children = { checkbox, radio } },
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            Color foreground = Color.Parse(dark ? "#F5F5F5" : "#242424");
            var surface = Part<Border>(checkbox, "PART_Border");
            var label = Part<ContentPresenter>(checkbox, "PART_ContentPresenter");
            foreach (bool? selection in new bool?[] { true, null, false, true })
            {
                checkbox.IsChecked = selection;
                Point point = checkbox.TranslatePoint(new Point(8, 8), window)!.Value;
                window.MouseMove(point);
                Dispatcher.UIThread.RunJobs();
                Assert.Equal(0, ColorOf(surface.Background).A);
                Assert.Equal(foreground, ColorOf(label.Foreground));
            }
            Assert.Equal(Colors.White, ColorOf(Part<Ellipse>(radio, "CheckGlyph").Fill));
            radio.IsEnabled = false;
            Dispatcher.UIThread.RunJobs();
            Assert.NotEqual(Colors.White, ColorOf(Part<Ellipse>(radio, "CheckGlyph").Fill));
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void SliderValueAndThumbRemainDistinctInBothThemes(bool dark)
    {
        var slider = new Slider { Minimum = 0, Maximum = 100, Value = 50 };
        var window = new Window { Width = 360, Height = 100, Content = slider,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var thumb = Part<Thumb>(slider, "thumb");
            Assert.Equal(Color.Parse("#0D99FF"), ColorOf(Part<RepeatButton>(slider, "PART_DecreaseButton").Background));
            Assert.NotEqual(ColorOf(thumb.Background), ColorOf(thumb.BorderBrush));
            slider.Foreground = Brushes.Purple;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Colors.Purple, ColorOf(Part<RepeatButton>(slider, "PART_DecreaseButton").Background));
            Assert.Equal(50, slider.Value);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void CalendarSelectionUsesReadableRoundedNativeDayCells(bool dark)
    {
        DateTime date = new(2026, 9, 26);
        var calendar = new Calendar { DisplayDate = date, SelectedDate = date };
        var window = new Window { Width = 420, Height = 440, Content = calendar,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var day = calendar.GetVisualDescendants().OfType<CalendarDayButton>()
                .Single(button => button.DataContext is DateTime value && value.Date == date);
            var surface = Part<Border>(day, "Root");
            Assert.Equal(new CornerRadius(6), surface.CornerRadius);
            Assert.Equal(Color.Parse(dark ? "#1976DE" : "#0969DA"), ColorOf(surface.Background));
            Assert.Equal(Colors.White, ColorOf(Part<ContentPresenter>(day, "PART_ContentPresenter").Foreground));
            calendar.SelectedDate = date.AddDays(-1);
            Dispatcher.UIThread.RunJobs();
            Assert.NotEqual(Color.Parse(dark ? "#1976DE" : "#0969DA"), ColorOf(surface.Background));
        }
        finally { window.Close(); }
    }

    private static T Part<T>(Control control, string name) where T : Control =>
        control.GetVisualDescendants().OfType<T>().Single(element => element.Name == name);

    private static Color ColorOf(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;
}
