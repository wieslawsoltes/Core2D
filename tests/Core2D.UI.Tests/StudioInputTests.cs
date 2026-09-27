using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioInputTests
{
    [AvaloniaFact]
    public void NumericKeyboardEditsKeepTheTwoWayBinding()
    {
        var source = new TextBox { Text = "12" };
        var field = new StudioNumberBox { Prefix = "X" };
        field.Bind(TextBox.TextProperty, new Binding(nameof(TextBox.Text)) { Source = source, Mode = BindingMode.TwoWay });
        var window = new Window { Width = 260, Height = 80, Content = field };
        try
        {
            window.Show();
            Assert.True(field.Focus());
            window.KeyPressQwerty(PhysicalKey.ArrowUp, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.ArrowUp, RawInputModifiers.None);
            Assert.Equal("13", source.Text);
            window.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.Shift);
            window.KeyReleaseQwerty(PhysicalKey.ArrowDown, RawInputModifiers.Shift);
            Assert.Equal("3", source.Text);
            source.Text = "48";
            Assert.Equal("48", field.Text);
            field.IsReadOnly = true;
            window.KeyPressQwerty(PhysicalKey.ArrowUp, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.ArrowUp, RawInputModifiers.None);
            Assert.Equal("48", field.Text);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void NumericScrubCommitsOnceAndEscapeCancels()
    {
        var field = new StudioNumberBox { Prefix = "X", Text = "10" };
        var window = new Window { Width = 260, Height = 80, Content = field };
        try
        {
            window.Show();
            var handle = field.GetVisualDescendants().OfType<ContentPresenter>().Single(x => x.Name == "PART_InnerLeft");
            var start = handle.TranslatePoint(new Point(handle.Bounds.Width / 2, handle.Bounds.Height / 2), window)!.Value;
            var changes = 0;
            field.PropertyChanged += (_, e) => { if (e.Property == TextBox.TextProperty) changes++; };
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(30, 0));
            Assert.Equal("10", field.Text);
            window.MouseUp(start + new Vector(30, 0), MouseButton.Left);
            Assert.Equal("20", field.Text);
            Assert.Equal(1, changes);
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(60, 0));
            window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            window.MouseUp(start + new Vector(60, 0), MouseButton.Left);
            Assert.Equal("20", field.Text);
            Assert.Equal(1, changes);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void HexEditsPreserveAlphaAndRejectInvalidColors()
    {
        var field = new StudioColorField { Color = Color.FromArgb(128, 20, 30, 40) };
        field.Hex = "#336699";
        Assert.Equal(Color.FromArgb(128, 51, 102, 153), field.Color);
        Assert.Throws<FormatException>(() => field.Hex = "not hex");
        Assert.Equal(Color.FromArgb(128, 51, 102, 153), field.Color);
        field.OpacityPercent = 25;
        Assert.Equal((byte)64, field.Color.A);
        Assert.Equal("336699", field.Hex);
        field.Hex = "FF112233";
        Assert.Equal(100m, field.OpacityPercent);
        Assert.Throws<ArgumentOutOfRangeException>(() => field.OpacityPercent = 101);
    }

    [AvaloniaFact]
    public void ColorPickerAndCustomComboPopupOpenAndClose()
    {
        var color = new StudioColorField { Color = Colors.CornflowerBlue };
        var combo = new ComboBox { ItemsSource = new[] { "Left", "Center", "Right" }, SelectedIndex = 0 };
        var window = new Window { Width = 300, Height = 220, Content = new StackPanel { Children = { color, combo } } };
        try
        {
            window.Show();
            color.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(color.GetVisualDescendants().OfType<Popup>().Single().IsOpen);
            color.IsOpen = false;
            combo.Focus();
            window.KeyPressQwerty(PhysicalKey.F4, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.F4, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();
            Assert.True(combo.IsDropDownOpen);
            combo.SelectedIndex = 2;
            window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            Assert.False(combo.IsDropDownOpen);
            Assert.Equal("Right", combo.SelectedItem);
        }
        finally { window.Close(); }
    }
}
