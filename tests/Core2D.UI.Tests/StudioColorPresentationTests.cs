using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using SkiaSharp;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioColorPresentationTests
{
    [AvaloniaFact]
    public void CompactOpacityReservesNoSpaceForMissingPrefixOrInnerContent()
    {
        var field = new StudioNumericField { Width = 72, Value = 75.29m, Suffix = "%" };
        var window = new Window { Width = 200, Height = 80, Content = field };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var handle = field.GetVisualDescendants().OfType<Border>().Single(x => x.Name == "PART_ScrubHandle");
            Assert.False(handle.IsVisible);
            var input = StudioAdvancedInputTests.Input(field);
            var scroll = input.GetVisualDescendants().OfType<ScrollViewer>().Single(x => x.Name == "PART_ScrollViewer");
            var text = new FormattedText(input.Text!, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(input.FontFamily, input.FontStyle, input.FontWeight), input.FontSize, Brushes.Black);
            Assert.True(scroll.Viewport.Width >= text.Width, $"Text requires {text.Width}, viewport provides {scroll.Viewport.Width}.");
            field.Prefix = "A";
            Dispatcher.UIThread.RunJobs();
            Assert.True(handle.IsVisible);
            field.Prefix = "";
            field.Suffix = null;
            Dispatcher.UIThread.RunJobs();
            Assert.False(handle.IsVisible);
            Assert.Equal(75.29m, field.Value);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void RealizedModelSwitchesPreserveBytesAndColorPlanePaintsHsv(bool dark)
    {
        Color original = Color.FromArgb(73, 17, 93, 201);
        var editor = new StudioColorEditor { Color = original };
        var window = new Window
        {
            Width = 300,
            Height = 420,
            Content = editor,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            int writes = 0;
            editor.PropertyChanged += (_, e) => { if (e.Property == StudioColorEditor.ColorProperty) writes++; };
            for (int i = 0; i < 12; i++)
            {
                editor.ModelIndex = i % 3;
                Dispatcher.UIThread.RunJobs();
                Assert.Equal(original, editor.Color);
            }
            Assert.Equal(0, writes);
            editor.Hsv = new HsvColor(.5, 120, 1, 1);
            var plane = Assert.Single(editor.GetVisualDescendants().OfType<StudioColorPlane>());
            window.MouseMove(new Point(299, 419));
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            using var stream = new MemoryStream();
            frame!.Save(stream);
            stream.Position = 0;
            using var bitmap = SKBitmap.Decode(stream);
            foreach (var position in new[] { new Point(.25, .25), new Point(.75, .25), new Point(.75, .75) })
            {
                Point point = plane.TranslatePoint(new Point(plane.Bounds.Width * position.X, plane.Bounds.Height * position.Y), window)!.Value;
                var actual = bitmap.GetPixel((int)point.X, (int)point.Y);
                Color expected = new HsvColor(1, 120, position.X, 1 - position.Y).ToRgb();
                Assert.InRange(Math.Abs(actual.Red - expected.R), 0, 3);
                Assert.InRange(Math.Abs(actual.Green - expected.G), 0, 3);
                Assert.InRange(Math.Abs(actual.Blue - expected.B), 0, 3);
            }
        }
        finally
        {
            window.Close();
        }
    }
}
