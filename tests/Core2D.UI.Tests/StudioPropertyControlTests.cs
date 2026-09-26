using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.Model.History;
using Core2D.Model.Style;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Views.Style;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioPropertyControlTests
{
    [AvaloniaFact]
    public void NumericDraftCommitsOnceAndPreservesTheBinding()
    {
        var source = new NumericUpDown { Value = 100 };
        var field = new StudioNumericField { Prefix = "W", Minimum = 0, Maximum = 1000 };
        field.Bind(StudioNumericField.ValueProperty, new Binding(nameof(source.Value)) { Source = source, Mode = BindingMode.TwoWay });
        var window = new Window { Width = 280, Height = 90, Content = field };
        try
        {
            window.Show();
            var input = field.GetVisualDescendants().OfType<TextBox>().Single();
            Assert.True(input.Focus());
            int changes = 0;
            source.PropertyChanged += (_, e) => { if (e.Property == NumericUpDown.ValueProperty) changes++; };
            input.Text = "(24+8)/2";
            Assert.Equal(100m, source.Value);
            Key(window, PhysicalKey.Enter);
            Assert.Equal(16m, source.Value);
            Assert.Equal(1, changes);
            input.Text = "1/0";
            Key(window, PhysicalKey.Enter);
            Assert.Equal(16m, source.Value);
            Assert.NotNull(field.Error);
            Key(window, PhysicalKey.Escape);
            Assert.Equal("16", field.DraftText);
            Assert.Null(field.Error);
            source.Value = 80;
            Assert.Equal("80", input.Text);
            input.Text = "+=10%";
            Key(window, PhysicalKey.Enter);
            Assert.Equal(88m, source.Value);
            field.IsReadOnly = true;
            input.Text = "90";
            Assert.False(field.TryCommit());
            Assert.Equal(88m, source.Value);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void NumericScrubCancelsWhenTheModelOrSelectionChanges()
    {
        var field = new StudioNumericField { Prefix = "X", Value = 10 };
        var window = new Window { Width = 280, Height = 90, Content = field };
        try
        {
            window.Show();
            var handle = field.GetVisualDescendants().OfType<Border>().Single(x => x.Name == "PART_ScrubHandle");
            var start = handle.TranslatePoint(new Point(handle.Bounds.Width / 2, handle.Bounds.Height / 2), window)!.Value;
            int commits = 0;
            field.PropertyChanged += (_, e) => { if (e.Property == StudioNumericField.ValueProperty) commits++; };
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(30, 0));
            Assert.Equal(10m, field.Value);
            window.MouseUp(start + new Vector(30, 0), MouseButton.Left);
            Assert.Equal(20m, field.Value);
            Assert.Equal(1, commits);
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(90, 0));
            field.Value = 200;
            window.MouseUp(start + new Vector(90, 0), MouseButton.Left);
            Assert.Equal(200m, field.Value);
            field.DraftText = "999";
            field.DataContext = new object();
            Assert.Equal("200", field.DraftText);
            field.Value = null;
            Assert.Empty(field.DraftText);
            Assert.False(field.TryCommit());
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void SegmentsNavigateAroundDisabledOptionsAndPreserveEnumBinding()
    {
        var source = new TextStyleViewModel(null) { TextHAlignment = TextHAlignment.Left };
        var strip = new StudioSegmentedControl { ItemsSource = TextStyleViewModel.TextHAlignmentValues };
        strip.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(source.TextHAlignment)) { Source = source, Mode = BindingMode.TwoWay });
        var window = new Window { Width = 280, Height = 90, Content = strip };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            strip.ContainerFromIndex(1)!.IsEnabled = false;
            Assert.True(strip.ContainerFromIndex(0)!.Focus());
            Key(window, PhysicalKey.ArrowRight);
            Assert.Equal(TextHAlignment.Right, source.TextHAlignment);
            Key(window, PhysicalKey.Home);
            Assert.Equal(TextHAlignment.Left, source.TextHAlignment);
            source.TextHAlignment = TextHAlignment.Right;
            Assert.Equal(2, strip.SelectedIndex);
            strip.FlowDirection = FlowDirection.RightToLeft;
            Key(window, PhysicalKey.ArrowRight);
            Assert.Equal(TextHAlignment.Left, source.TextHAlignment);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void PaletteAndSwitchKeepTheirLiveValues()
    {
        var source = new StudioColorField { Color = Color.FromArgb(80, 0, 0, 0) };
        var palette = new StudioColorPalette();
        palette.Bind(StudioColorPalette.ColorProperty, new Binding(nameof(source.Color)) { Source = source, Mode = BindingMode.TwoWay });
        palette.SelectedItem = Color.Parse("#9747FF");
        Assert.Equal(Color.FromArgb(80, 151, 71, 255), source.Color);
        source.Color = Color.FromArgb(44, 1, 2, 3);
        Assert.Equal(-1, palette.SelectedIndex);
        Assert.Equal(44, palette.Color.A);
        var toggle = new StudioSwitch { Content = "Snap to grid", IsChecked = false };
        var window = new Window { Width = 280, Height = 90, Content = toggle };
        try
        {
            window.Show();
            Assert.True(toggle.Focus());
            Key(window, PhysicalKey.Space);
            Assert.True(toggle.IsChecked);
            toggle.IsEnabled = false;
            Key(window, PhysicalKey.Space);
            Assert.True(toggle.IsChecked);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void SearchClearAndBoundsLifecycleWorkInTheRealTemplates()
    {
        var search = new StudioSearchBox { Text = "Heading" };
        var start = new PointShapeViewModel(null) { X = 10, Y = 20 };
        var end = new PointShapeViewModel(null) { X = 110, Y = 70 };
        IHistory history = new StackHistory();
        var bounds = new StudioBoundsEditor { Start = start, End = end };
        var panel = new StackPanel { Children = { search, bounds } };
        StudioEditContext.SetHistory(panel, history);
        var window = new Window { Width = 300, Height = 300, Content = panel };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var clear = search.GetVisualDescendants().OfType<Button>().Single(x => x.Name == "PART_Clear");
            Click(window, clear);
            Assert.Equal(string.Empty, search.Text);
            Assert.True(search.IsFocused);
            Assert.NotNull(bounds.Editor);
            bounds.Editor!.Width = 200;
            Assert.True(history.CanUndo());
            var old = bounds.Editor;
            panel.Children.Remove(bounds);
            Assert.Null(bounds.Editor);
            old.Width = 500;
            Assert.Equal(210, end.X);
            panel.Children.Add(bounds);
            Dispatcher.UIThread.RunJobs();
            Assert.NotSame(old, bounds.Editor);
            Assert.True(history.Undo());
            Assert.Equal(100, bounds.Editor!.Width);
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void TypographyControlsEditTheModelAndRenderInBothThemes(bool dark)
    {
        var model = new TextStyleViewModel(null) { FontName = "Inter", FontSize = 24, TextHAlignment = TextHAlignment.Left, TextVAlignment = TextVAlignment.Top };
        var view = new TextStyleView { DataContext = model };
        var palette = new StudioColorPalette { Color = Color.Parse("#9747FF") };
        var search = new StudioSearchBox { Text = "Feature card" };
        var bounds = new StudioBoundsEditor
        {
            Start = new PointShapeViewModel(null) { X = 44, Y = 180 },
            End = new PointShapeViewModel(null) { X = 310, Y = 358 }
        };
        var panel = new StackPanel { Spacing = 14, Margin = new Thickness(16), Children =
        {
            new TextBlock { Text = "PROPERTY CONTROLS", FontSize = 11 }, search, bounds,
            new Separator(), new TextBlock { Text = "Typography", FontWeight = FontWeight.SemiBold }, view,
            new Separator(), new TextBlock { Text = "Color presets", FontWeight = FontWeight.SemiBold }, palette,
            new StudioColorField { Color = Color.FromArgb(128, 151, 71, 255) },
            new StudioSwitch { Content = "Snap to grid", IsChecked = true },
            new StudioPropertyRow { Label = "Precision", Content = new StudioNumericField { Prefix = "#", Value = 0.125m, Increment = 0.125m } }
        }};
        var window = new Window { Width = 340, Height = 720, Content = new StudioSurface { Content = panel }, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var bold = view.GetVisualDescendants().OfType<StudioIconToggle>().Single(x => Equals(x.Content, "B"));
            Click(window, bold);
            Assert.True(model.FontStyle.HasFlag(FontStyleFlags.Bold));
            var size = view.GetVisualDescendants().OfType<StudioNumericField>().Single();
            size.DraftText = "(24+8)/2";
            Assert.True(size.TryCommit());
            Assert.Equal(16, model.FontSize);
            var fonts = view.GetVisualDescendants().OfType<StudioFontPicker>().Single();
            Assert.NotNull(fonts.ItemsSource);
            Assert.NotEmpty(fonts.ItemsSource!.Cast<object>());
            fonts.Text = "Custom font";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Custom font", model.FontName);
            model.FontName = "Inter";
            fonts.IsDropDownOpen = false;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Inter", fonts.GetVisualDescendants().OfType<TextBox>().Single().Text);
            // Settle compositor jobs as well as UI bindings before capturing the final model state.
            window.MouseMove(new Point(330, 710));
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            string? directory = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
                frame!.Save(Path.Combine(directory, $"property-controls-{(dark ? "dark" : "light")}.png"));
            }
        }
        finally { window.Close(); }
    }

    private static void Key(Window window, PhysicalKey key)
    {
        window.KeyPressQwerty(key, RawInputModifiers.None);
        window.KeyReleaseQwerty(key, RawInputModifiers.None);
    }

    private static void Click(Window window, Control control)
    {
        Point point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }
}
