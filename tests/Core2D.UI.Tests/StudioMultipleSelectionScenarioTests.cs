using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.ViewModels;
using Core2D.ViewModels.Shapes;
using Core2D.Views;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioMultipleSelectionScenarioTests
{
    [AvaloniaTheory]
    [InlineData(false, 1440, 900)]
    [InlineData(true, 1440, 900)]
    [InlineData(true, 1024, 768)]
    public void MixedSelectionSwitchesAndBoundsRenderInTheActualWorkspace(bool dark, int width, int height)
    {
        using var state = new AppState();
        var a = StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        var project = editor.Project!;
        var b = project.CurrentContainer!.CurrentLayer!.Shapes.OfType<RectangleShapeViewModel>().First(x => x != a);
        a.IsFilled = true; b.IsFilled = false;
        project.SelectedShapes = new HashSet<BaseShapeViewModel> { a, b };
        var view = new MainView { DataContext = editor };
        var window = new Window { Width = width, Height = height, Content = view, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            editor.Platform?.OnZoomAutoFit();
            var selection = view.GetVisualDescendants().OfType<StudioSelectionEditor>().Single();
            Assert.True(selection.IsVisible);
            Assert.Null(selection.Editor!.IsFilled);
            var fill = selection.GetVisualDescendants().OfType<StudioSwitch>().Single(x => Equals(x.Content, "Fill"));
            Assert.Null(fill.IsChecked);
            project.History!.Reset();
            Assert.True(fill.Focus());
            StudioAdvancedInputTests.Key(window, PhysicalKey.Space);
            Assert.NotNull(fill.IsChecked);
            Assert.Equal(a.IsFilled, b.IsFilled);
            Assert.True(project.History.Undo());
            Assert.Null(selection.Editor.IsFilled);
            Assert.False(project.History.CanUndo());
            selection.Editor.AnchorIndex = 4;
            window.MouseMove(new Point(width / 2d, height / 2d));
            using var frame = window.CaptureRenderedFrame();
            Assert.NotNull(frame);
            if (Environment.GetEnvironmentVariable("CORE2D_UI_ARTIFACTS") is { Length: > 0 } folder)
            {
                Directory.CreateDirectory(folder);
                frame!.Save(Path.Combine(folder, $"multi-workspace-{width}-{(dark ? "dark" : "light")}.png"));
            }
        }
        finally { window.Close(); }
    }
}
