using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Notifications;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using global::BrowserTheme.Catalog;
using Xunit;

namespace Avalonia.Themes.Browser.Tests;

public class BrowserThemeTests
{
    [AvaloniaTheory]
    [MemberData(nameof(ThemeInventory.Resources), MemberType = typeof(ThemeInventory))]
    public void EveryImportedThemeKeyResolvesWithoutFluent(object key, bool dark)
    {
        var provider = (IResourceNode)Theme;
        Assert.True(provider.TryGetResource(key, dark ? ThemeVariant.Dark : ThemeVariant.Light, out var value), $"Missing imported theme: {key}");
        Assert.IsType<ControlTheme>(value);
    }

    [AvaloniaTheory]
    [InlineData(false, 0)] [InlineData(true, 0)]
    [InlineData(false, 1)] [InlineData(true, 1)]
    [InlineData(false, 2)] [InlineData(true, 2)]
    public void CompleteControlFamiliesRenderAndKeepNativeTemplates(bool dark, int page)
    {
        Theme.Density = BrowserDensity.Comfortable;
        var view = new GalleryView();
        var window = new Window { Width = 1080, Height = 940, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Jobs();
            view.FindControl<TabControl>("Pages")!.SelectedIndex = page;
            Jobs();
            if (page == 2)
            {
                view.FindControl<Calendar>("Calendar")!.DisplayDate = new DateTime(2026, 9, 26);
                view.FindControl<Calendar>("Calendar")!.SelectedDate = new DateTime(2026, 9, 26);
                view.FindControl<DatePicker>("Date")!.SelectedDate = new DateTimeOffset(2026, 9, 26, 0, 0, 0, TimeSpan.Zero);
                view.FindControl<TimePicker>("Time")!.SelectedTime = TimeSpan.FromHours(14.5);
            }
            Assert.True(view.Bounds.Width >= 1080);
            Assert.True(view.GetVisualDescendants().OfType<TemplatedControl>().Count(x => x.Template is not null) > 15);
            window.MouseMove(new Point(1070, 930));
            Capture(window, $"browser-family-{page}-{(dark ? "dark" : "light")}.png");
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(BrowserDensity.Compact, 28)]
    [InlineData(BrowserDensity.Comfortable, 36)]
    [InlineData(BrowserDensity.Touch, 44)]
    public void LiveDensityPreservesFocusedEditorAndText(BrowserDensity density, double height)
    {
        Theme.Density = BrowserDensity.Comfortable;
        var input = new TextBox { Text = "Draft", Width = 240 };
        var button = new Button { Content = "Action" };
        var window = new Window { Width = 300, Height = 180, Content = new StackPanel { Spacing = 10, Children = { input, button } } };
        try
        {
            window.Show(); Jobs();
            var presenter = input.GetVisualDescendants().OfType<TextPresenter>().Single();
            Assert.True(input.Focus());
            input.CaretIndex = 3;
            Theme.Density = density; Jobs();
            Assert.Same(presenter, input.GetVisualDescendants().OfType<TextPresenter>().Single());
            Assert.True(input.IsFocused);
            Assert.Equal("Draft", input.Text);
            Assert.Equal(3, input.CaretIndex);
            Assert.Equal(height, button.MinHeight);
            Assert.Equal(height, input.MinHeight);
            Capture(window, $"browser-density-{density}.png");
        }
        finally { window.Close(); Theme.Density = BrowserDensity.Comfortable; }
    }

    [AvaloniaFact]
    public void LocalSemanticBrushOverridesAndRuntimeThemeChangesReachTemplates()
    {
        var button = new Button { Content = "Scoped surface" };
        var input = new TextBox { Text = "Theme-aware" };
        var panel = new StackPanel { Spacing = 8, Children = { button, input } };
        var window = new Window { Width = 320, Height = 180, Content = panel, RequestedThemeVariant = ThemeVariant.Light };
        try
        {
            window.Show(); Jobs();
            Assert.Equal(Color.Parse("#F5F5F5"), Brush(button.Background));
            window.RequestedThemeVariant = ThemeVariant.Dark; Jobs();
            Assert.Equal(Color.Parse("#383838"), Brush(button.Background));
            var custom = new SolidColorBrush(Color.Parse("#483E60"));
            panel.Resources["BrowserInputBrush"] = custom; Jobs();
            Assert.Same(custom, button.Background);
            Assert.Same(custom, input.Background);
            panel.Resources.Remove("BrowserInputBrush"); Jobs();
            Assert.Equal(Color.Parse("#383838"), Brush(input.Background));
            Assert.Equal(Color.Parse("#F5F5F5"), Brush(input.Foreground));
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void ButtonsChecksRadiosAndSwitchesRetainNativeKeyboardBehavior()
    {
        var button = new Button { Content = "Action", Classes = { "primary" } };
        int count = 0; button.Click += (_, _) => count++;
        var check = new CheckBox { Content = "Check", IsThreeState = true, IsChecked = false };
        var left = new RadioButton { Content = "Left", GroupName = "options", IsChecked = true };
        var right = new RadioButton { Content = "Right", GroupName = "options" };
        var toggle = new ToggleSwitch { Content = "Sync", IsChecked = false };
        var window = new Window { Width = 360, Height = 400, Content = new StackPanel { Spacing = 8, Children = { button, check, left, right, toggle } } };
        try
        {
            window.Show(); Jobs();
            button.Focus(); Press(window, PhysicalKey.Space); Assert.Equal(1, count);
            button.IsEnabled = false; Press(window, PhysicalKey.Space); Assert.Equal(1, count);
            check.Focus(); Press(window, PhysicalKey.Space); Assert.True(check.IsChecked);
            Press(window, PhysicalKey.Space); Assert.Null(check.IsChecked);
            right.Focus(); Press(window, PhysicalKey.Space); Assert.True(right.IsChecked); Assert.False(left.IsChecked);
            toggle.Focus(); Press(window, PhysicalKey.Space); Assert.True(toggle.IsChecked);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void FullTextTemplateSupportsSlotsWatermarkPasswordAndNativeEditing()
    {
        var input = new TextBox { Watermark = "Name", UseFloatingWatermark = true, Classes = { "revealPasswordButton" }, PasswordChar = '*', InnerLeftContent = "#", InnerRightContent = "px" };
        var window = new Window { Width = 420, Height = 100, Content = input };
        try
        {
            window.Show(); Jobs();
            Assert.NotNull(input.GetVisualDescendants().OfType<TextPresenter>().Single());
            Assert.NotNull(input.GetVisualDescendants().OfType<ContentPresenter>().Single(x => x.Name == "PART_InnerLeft"));
            input.Focus(); window.KeyTextInput("Example"); Assert.Equal("Example", input.Text);
            input.SelectAll(); Press(window, PhysicalKey.Backspace); Assert.Equal(string.Empty, input.Text);
            input.Text = "secret";
            Assert.Equal('*', input.GetVisualDescendants().OfType<TextPresenter>().Single().PasswordChar);
            input.RevealPassword = true; Jobs();
            Assert.True(input.GetVisualDescendants().OfType<TextPresenter>().Single().RevealPassword);
            Assert.NotNull(input.ContextFlyout);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void SliderAndScrollBarPreserveNativeRangeAndThumbDirection(bool vertical)
    {
        var slider = new Slider { Orientation = vertical ? Orientation.Vertical : Orientation.Horizontal, Minimum = 0, Maximum = 100, Value = 40, SmallChange = 5, Height = vertical ? 250 : 40, Width = vertical ? 40 : 250 };
        var bar = new ScrollBar { Orientation = slider.Orientation, Minimum = 0, Maximum = 1000, Value = 200, ViewportSize = 200, AllowAutoHide = false, Height = vertical ? 250 : 12, Width = vertical ? 12 : 250 };
        var window = new Window { Width = 350, Height = 650, Content = new StackPanel { Spacing = 16, Children = { slider, bar } } };
        try
        {
            window.Show(); Jobs();
            slider.Focus(); Press(window, vertical ? PhysicalKey.ArrowUp : PhysicalKey.ArrowRight); Assert.Equal(45, slider.Value);
            var thumb = bar.GetVisualDescendants().OfType<Thumb>().Single();
            Point p = thumb.TranslatePoint(new Point(thumb.Bounds.Width / 2, thumb.Bounds.Height / 2), window)!.Value;
            Vector delta = vertical ? new Vector(0, 30) : new Vector(30, 0);
            window.MouseDown(p, MouseButton.Left); window.MouseMove(p + delta); window.MouseUp(p + delta, MouseButton.Left);
            Assert.True(bar.Value > 200);
            bar.Value = 2000; Assert.Equal(1000, bar.Value);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(Dock.Top)] [InlineData(Dock.Bottom)] [InlineData(Dock.Left)] [InlineData(Dock.Right)]
    public void TabsKeepAllFourPlacementsAndSelectedContent(Dock placement)
    {
        var first = new TextBox { Text = "First" }; var second = new TextBox { Text = "Second" };
        var tabs = new TabControl { TabStripPlacement = placement, Items = { new TabItem { Header = "Design", Content = first }, new TabItem { Header = "Data", Content = second } } };
        var window = new Window { Width = 400, Height = 260, Content = tabs };
        try
        {
            window.Show(); Jobs();
            Assert.Same(first, tabs.SelectedContent);
            tabs.SelectedIndex = 1; Jobs(); Assert.Same(second, tabs.SelectedContent);
            Assert.True(second.Bounds.Width > 0);
            Assert.True(second.Focus()); window.KeyTextInput(" edit"); Assert.Contains("edit", second.Text);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void DateAndTimePickersOpenTheirImportedPresenters()
    {
        var date = new DatePicker { SelectedDate = new DateTimeOffset(2026, 9, 26, 0, 0, 0, TimeSpan.Zero) };
        var time = new TimePicker { SelectedTime = TimeSpan.FromHours(14) };
        var window = new Window { Width = 500, Height = 600, Content = new StackPanel { Children = { date, time } } };
        try
        {
            window.Show(); Jobs();
            Assert.NotEmpty(date.GetVisualDescendants().OfType<Button>());
            var dateButton = date.GetVisualDescendants().OfType<Button>().First(x => x.IsEffectivelyVisible);
            Click(window, dateButton); Jobs();
            Assert.Equal(26, date.SelectedDate!.Value.Day);
            Assert.NotNull(date.Template); Assert.NotNull(time.Template);
            Press(window, PhysicalKey.Escape);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void NotificationAndValidationSurfacesResolveWithoutAppStyles()
    {
        var input = new TextBox { Text = "invalid" };
        DataValidationErrors.SetErrors(input, new[] { "A value is required." });
        var card = new NotificationCard { Content = new Notification("Saved", "Your changes are stored.", NotificationType.Success) };
        var window = new Window { Width = 400, Height = 220, Content = new StackPanel { Spacing = 8, Children = { input, card } } };
        try
        {
            window.Show(); Jobs();
            Assert.True(DataValidationErrors.GetHasErrors(input));
            Assert.NotNull(card.Template);
            Assert.NotEmpty(input.GetVisualDescendants().OfType<DataValidationErrors>());
            Capture(window, "browser-feedback.png");
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void InvalidDensityDoesNotCorruptExistingResources()
    {
        var before = Theme.Density;
        Assert.Throws<ArgumentOutOfRangeException>(() => Theme.Density = (BrowserDensity)42);
        Assert.Equal(before, Theme.Density);
    }

    internal static BrowserTheme Theme => Application.Current!.Styles.OfType<BrowserTheme>().Single();
    private static Color Brush(IBrush? value) => Assert.IsAssignableFrom<ISolidColorBrush>(value).Color;
    private static void Jobs() => Dispatcher.UIThread.RunJobs();
    private static void Press(Window window, PhysicalKey key)
    { window.KeyPressQwerty(key, RawInputModifiers.None); window.KeyReleaseQwerty(key, RawInputModifiers.None); Jobs(); }
    private static void Click(Window window, Control control)
    {
        Point point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Jobs();
    }
    private static void Capture(Window window, string name)
    {
        using var frame = window.CaptureRenderedFrame(); Assert.NotNull(frame);
        string? directory = Environment.GetEnvironmentVariable("BROWSER_THEME_ARTIFACTS");
        if (string.IsNullOrEmpty(directory)) return;
        Directory.CreateDirectory(directory); frame!.Save(Path.Combine(directory, name));
    }
}
