using System;
using System.IO;
using System.Linq;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Style;
using Core2D.Views.Renderer;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioGridInspectorTests
{
    [AvaloniaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void GridFieldsCommitValidDraftsAndPreserveEveryOriginalSetting(bool dark)
    {
        var model = new TemplateContainerViewModel(null)
        {
            IsGridEnabled = true, IsBorderEnabled = false, GridOffsetLeft = 10, GridOffsetTop = 20,
            GridOffsetRight = 30, GridOffsetBottom = 40, GridCellWidth = 25, GridCellHeight = 50,
            GridStrokeThickness = .5, GridStrokeColor = new ArgbColorViewModel(null) { Value = ArgbColorViewModel.ToUint32(128, 100, 120, 140) }
        };
        var view = new GridView { DataContext = model };
        var window = new Window { Width = 320, Height = 660, Content = new StudioSurface { Content = view }, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Dispatcher.UIThread.RunJobs();
            var fields = view.GetVisualDescendants().OfType<StudioNumericField>().ToArray();
            var width = fields.Single(x => AutomationProperties.GetName(x) == "Grid cell width");
            var input = width.GetVisualDescendants().OfType<TextBox>().Single();
            Assert.True(input.Focus());
            input.Text = "+=5";
            Assert.Equal(25d, model.GridCellWidth);
            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Assert.Equal(30d, model.GridCellWidth);
            input.Text = "1/0";
            Assert.False(width.TryCommit());
            Assert.Equal(30d, model.GridCellWidth);
            width.CancelEdit();
            Assert.Equal(10d, model.GridOffsetLeft); Assert.Equal(20d, model.GridOffsetTop);
            Assert.Equal(30d, model.GridOffsetRight); Assert.Equal(40d, model.GridOffsetBottom);
            Assert.Equal(50d, model.GridCellHeight); Assert.Equal(.5, model.GridStrokeThickness);
            var border = view.GetVisualDescendants().OfType<StudioSwitch>().Single(x => AutomationProperties.GetName(x) == "Show page border");
            border.IsChecked = true;
            Assert.True(model.IsBorderEnabled);
            model.GridCellWidth = 40;
            Assert.Equal(40m, width.Value);
            Dispatcher.UIThread.RunJobs();
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            string? directory = Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS");
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
                frame!.Save(Path.Combine(directory, $"layout-grid-{(dark ? "dark" : "light")}.png"));
            }
        }
        finally { window.Close(); }
    }
}
