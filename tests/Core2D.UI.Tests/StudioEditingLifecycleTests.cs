using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.ViewModels;
using Core2D.Views;
using Core2D.ViewModels.Style;
using Core2D.Views.Style;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioEditingLifecycleTests
{
    [AvaloniaFact]
    public void FontPickerTracksExternalChangesAndNativeTextInput()
    {
        var model = new TextStyleViewModel(null) { FontName = "Inter" };
        var view = new TextStyleView { DataContext = model };
        var window = new Window { Width = 320, Height = 400, Content = view };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var picker = Assert.Single(view.GetVisualDescendants().OfType<StudioFontPicker>());
            var input = Assert.Single(picker.GetVisualDescendants().OfType<TextBox>());
            Assert.Equal("Inter", input.Text);
            model.FontName = "Custom font";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Custom font", input.Text);
            model.FontName = "Inter";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Inter", input.Text);
            Assert.True(input.Focus());
            input.Text = "Document font";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Document font", model.FontName);
            model.FontName = "Other font";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal("Other font", input.Text);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void SegmentedSelectionUsesNeutralThemeTokensAndUpdatesLive()
    {
        var strip = new StudioSegmentedControl { ItemsSource = new[] { "One", "Two" }, SelectedIndex = 0 };
        var window = new Window { Width = 280, Height = 100, Content = strip, RequestedThemeVariant = ThemeVariant.Dark };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var item = strip.ContainerFromIndex(0)!;
            var presenter = item.GetVisualDescendants().OfType<ContentPresenter>().Single(x => x.Name == "PART_ContentPresenter");
            Assert.Equal(Color.Parse("#555555"), Assert.IsAssignableFrom<ISolidColorBrush>(presenter.Background).Color);
            window.RequestedThemeVariant = ThemeVariant.Light;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Colors.White, Assert.IsAssignableFrom<ISolidColorBrush>(presenter.Background).Color);
            Assert.Equal(0, strip.SelectedIndex);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void RemovingOrDisablingAnEditorCancelsCapturedScrubbing()
    {
        var field = new StudioNumericField { Prefix = "X", Value = 10 };
        var panel = new StackPanel { Children = { field } };
        var window = new Window { Width = 280, Height = 120, Content = panel };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var handle = field.GetVisualDescendants().OfType<Border>().Single(x => x.Name == "PART_ScrubHandle");
            Point start = handle.TranslatePoint(new Point(handle.Bounds.Width / 2, handle.Bounds.Height / 2), window)!.Value;
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(30, 0));
            panel.Children.Remove(field);
            window.MouseUp(start + new Vector(30, 0), MouseButton.Left);
            Assert.Equal(10m, field.Value);
            panel.Children.Add(field);
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(30, 0));
            panel.IsEnabled = false;
            window.MouseUp(start + new Vector(30, 0), MouseButton.Left);
            Assert.Equal(10m, field.Value);
            panel.IsEnabled = true;
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(30, 0));
            window.MouseUp(start + new Vector(30, 0), MouseButton.Left);
            Assert.Equal(20m, field.Value);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void DockedBoundsEditUsesDocumentHistoryAndEditorUndoRedo()
    {
        using var state = new AppState();
        var selected = StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        var project = editor.Project!;
        project.History.Reset();
        var view = new MainView { DataContext = editor };
        var window = new Window { Width = 1440, Height = 900, Content = view };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var bounds = Assert.Single(view.GetVisualDescendants().OfType<StudioBoundsEditor>());
            Assert.Same(project.History, StudioEditContext.GetHistory(bounds));
            var width = bounds.GetVisualDescendants().OfType<StudioNumericField>().Single(x => x.Prefix == "W");
            var input = Assert.Single(width.GetVisualDescendants().OfType<TextBox>());
            Assert.True(input.Focus());
            input.Text = "+=24";
            Assert.Equal(310d, selected.BottomRight!.X);
            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            window.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(334d, selected.BottomRight.X);
            Assert.True(editor.CanUndo());
            editor.OnUndo();
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(310d, selected.BottomRight.X);
            Assert.False(editor.CanUndo());
            Assert.True(editor.CanRedo());
            editor.OnRedo();
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(334d, selected.BottomRight.X);
            Assert.Equal(44d, selected.TopLeft!.X);
        }
        finally
        {
            window.Close();
        }
    }
}
