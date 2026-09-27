using System;
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
using Xunit;

namespace Core2D.UI.Tests;

public class StudioColorWorkbenchTests
{
    [AvaloniaFact]
    public void SwitchingModelsNeverChangesCanonicalColorOrAlpha()
    {
        var color = Color.FromArgb(73, 17, 93, 201);
        var editor = new StudioColorEditor { Color = color };
        int writes = 0;
        editor.PropertyChanged += (_, e) => { if (e.Property == StudioColorEditor.ColorProperty) writes++; };
        for (int i = 0; i < 20; i++)
        {
            editor.ModelIndex = i % 3;
            Assert.Equal(color, editor.Color);
        }
        Assert.Equal(0, writes);
        editor.ModelIndex = 0;
        editor.First = 128;
        Assert.Equal(Color.FromArgb(73, 128, 93, 201), editor.Color);
        editor.Alpha = 50;
        Assert.Equal(Color.FromArgb(128, 128, 93, 201), editor.Color);
        editor.Hex = "FF5500";
        Assert.Equal(Color.FromArgb(128, 255, 85, 0), editor.Color);
        editor.Hex = "44ABCDEF";
        Assert.Equal(Color.FromArgb(68, 171, 205, 239), editor.Color);
        editor.Hex = "invalid";
        Assert.Equal(Color.FromArgb(68, 171, 205, 239), editor.Color);
    }

    [AvaloniaFact]
    public void HslHsbAndAchromaticHueRemainEditable()
    {
        var editor = new StudioColorEditor { Color = Colors.Red, ModelIndex = 1 };
        Assert.Equal(100m, editor.Second);
        Assert.Equal(50m, editor.Third);
        editor.First = 120;
        Assert.Equal(Colors.Lime, editor.Color);
        editor.ModelIndex = 2;
        Assert.Equal(100m, editor.Third);
        editor.Second = 0;
        Assert.Equal(Colors.White, editor.Color);
        editor.Hue = 240;
        Assert.Equal(Colors.White, editor.Color);
        editor.Second = 100;
        Assert.Equal(Colors.Blue, editor.Color);
        editor.Color = Color.FromArgb(37, 100, 100, 100);
        Assert.Equal(240d, editor.Hue);
        editor.Hue = double.NaN;
        Assert.Equal(240d, editor.Hue);
    }

    [AvaloniaFact]
    public void ColorPlanePointerKeyboardEscapeAndExternalUpdatesPreserveTheBinding()
    {
        var editor = new StudioColorEditor { Color = Colors.Red };
        var window = new Window { Width = 300, Height = 430, Content = editor };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var plane = Assert.Single(editor.GetVisualDescendants().OfType<StudioColorPlane>());
            Point center = plane.TranslatePoint(new Point(plane.Bounds.Width / 2, plane.Bounds.Height / 2), window)!.Value;
            window.MouseDown(center, MouseButton.Left);
            Assert.InRange(plane.Hsv.S, .49, .51);
            Assert.InRange(plane.Hsv.V, .49, .51);
            StudioAdvancedInputTests.Key(window, PhysicalKey.Escape);
            window.MouseUp(center, MouseButton.Left);
            Assert.Equal(Colors.Red, editor.Color);
            Assert.True(plane.Focus());
            StudioAdvancedInputTests.Key(window, PhysicalKey.ArrowLeft);
            Assert.InRange(plane.Hsv.S, .989, .991);
            StudioAdvancedInputTests.Key(window, PhysicalKey.ArrowDown, RawInputModifiers.Shift);
            Assert.InRange(plane.Hsv.V, .899, .901);
            window.MouseDown(center, MouseButton.Left);
            editor.Color = Colors.Lime;
            window.MouseMove(center + new Vector(30, 10));
            window.MouseUp(center + new Vector(30, 10), MouseButton.Left);
            Assert.Equal(Colors.Lime, editor.Color);
            var hue = editor.GetVisualDescendants().OfType<StudioColorSlider>().Single(x => x.Maximum == 360);
            Assert.True(hue.Focus());
            StudioAdvancedInputTests.Key(window, PhysicalKey.ArrowRight);
            Assert.Equal(121d, editor.Hue, 5);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void HexFieldDoesNotPublishIncompleteTypingAndReconnectsInPopup()
    {
        var field = new StudioColorField { Color = Color.FromArgb(80, 10, 20, 30) };
        var window = new Window { Width = 340, Height = 600, Content = new StackPanel { Children = { field } } };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var hex = field.GetVisualDescendants().OfType<StudioHexField>().Single();
            var input = StudioAdvancedInputTests.Input(hex);
            input.Focus();
            input.Text = "FF";
            Assert.Equal(Color.FromArgb(80, 10, 20, 30), field.Color);
            StudioAdvancedInputTests.Key(window, PhysicalKey.Enter);
            Assert.NotNull(hex.Error);
            input.Text = "FF8800";
            StudioAdvancedInputTests.Key(window, PhysicalKey.Enter);
            Assert.Equal(Color.FromArgb(80, 255, 136, 0), field.Color);
            for (int i = 0; i < 2; i++)
            {
                field.IsOpen = true;
                Dispatcher.UIThread.RunJobs();
                var popup = field.GetVisualDescendants().OfType<Popup>().Single();
                Assert.True(popup.IsOpen);
                var colorEditor = popup.Child!.GetVisualDescendants().OfType<StudioColorEditor>().Single();
                colorEditor.Hex = i == 0 ? "0000FF" : "00FF00";
                Assert.Equal(i == 0 ? Color.FromArgb(80, 0, 0, 255) : Color.FromArgb(80, 0, 255, 0), field.Color);
                field.IsOpen = false;
            }
        }
        finally { window.Close(); }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void AdvancedControlGalleryUsesRealBindingsAndRendersInBothThemes(bool dark)
    {
        var editor = new StudioColorEditor { Color = Color.FromArgb(192, 151, 71, 255) };
        var dash = new StudioDashField { Value = "8 3 1 3" };
        var name = new StudioNameField { Value = "Feature card" };
        var panel = new StackPanel { Spacing = 12, Margin = new Thickness(16), Children =
        {
            new TextBlock { Text = "COLOR & STROKE", FontWeight = FontWeight.SemiBold }, editor,
            new StudioColorPalette { Color = editor.Color }, new Separator(),
            new TextBlock { Text = "Layer name", FontWeight = FontWeight.SemiBold }, name,
            new TextBlock { Text = "Stroke pattern", FontWeight = FontWeight.SemiBold }, dash,
            new StudioPropertyRow { Label = "Resize origin", Content = new StudioAnchorPicker { SelectedIndex = 4 } }
        }};
        var window = new Window { Width = 340, Height = 740, Content = new StudioSurface { Content = panel }, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(4, editor.GetVisualDescendants().OfType<StudioNumericField>().Count());
            Assert.True(name.TryCommit());
            Assert.True(dash.TryCommit());
            window.MouseMove(new Point(339, 739));
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            if (Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS") is { Length: > 0 } folder)
            {
                Directory.CreateDirectory(folder);
                frame!.Save(Path.Combine(folder, $"advanced-controls-{(dark ? "dark" : "light")}.png"));
            }
        }
        finally { window.Close(); }
    }
}
