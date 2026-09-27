using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.Model.History;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Views;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioAdvancedInputTests
{
    [AvaloniaFact]
    public void NativeNameDraftRecordsOneDocumentUndoAndReconnectsAfterReattachment()
    {
        var model = StudioSelectionModelTests.Rectangle(0, 0, 10, 10);
        model.Name = "Before";
        IHistory history = new StackHistory();
        var field = new StudioNameField { DataContext = model };
        field.Bind(StudioTextField.ValueProperty, new Binding(nameof(model.Name)) { Mode = BindingMode.TwoWay });
        var panel = new StackPanel { Children = { field, new Button { Content = "Other" } } };
        StudioEditContext.SetHistory(panel, history);
        var window = new Window { Width = 320, Height = 160, Content = panel };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            TextBox input = Input(field);
            Assert.True(input.Focus());
            input.Text = " After ";
            Assert.Equal("Before", model.Name);
            Key(window, PhysicalKey.Enter);
            Assert.Equal("After", model.Name);
            Assert.True(history.Undo());
            Assert.Equal("Before", input.Text);
            Assert.False(history.CanUndo());
            input.Text = "   ";
            Key(window, PhysicalKey.Enter);
            Assert.NotNull(field.Error);
            Assert.Equal("Before", model.Name);
            Key(window, PhysicalKey.Escape);
            Assert.Null(field.Error);
            input.Text = "Abandoned";
            model.Name = "External";
            Assert.Equal("External", input.Text);
            for (int i = 0; i < 2; i++)
            {
                panel.Children.Remove(field);
                panel.Children.Insert(0, field);
                Dispatcher.UIThread.RunJobs();
                input = Input(field);
                Assert.True(input.Focus());
                input.Text = $"Pass {i}";
                Key(window, PhysicalKey.Enter);
                Assert.Equal($"Pass {i}", model.Name);
                Assert.True(history.Undo());
                Assert.False(history.CanUndo());
            }
            input.Text = "Must not commit";
            field.IsReadOnly = true;
            Assert.Equal("External", field.DraftText);
            Assert.False(field.TryCommit());
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void DashPresetsCustomDraftAndHistoryUseTheSameCanonicalModel()
    {
        var model = new StrokeStyleViewModel(null) { Dashes = "8 4" };
        var field = new StudioDashField { DataContext = model };
        field.Bind(StudioTextField.ValueProperty, new Binding(nameof(model.Dashes)) { Mode = BindingMode.TwoWay });
        IHistory history = new StackHistory();
        StudioEditContext.SetHistory(field, history);
        var window = new Window { Width = 300, Height = 100, Content = field };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Contains(field.GetVisualDescendants(), x => x is StudioStrokePreview);
            Assert.Equal("Dashed", field.SelectedPreset!.Name);
            TextBox input = field.GetVisualDescendants().OfType<TextBox>().Single(x => x.Name == "PART_Input" && ReferenceEquals(x.TemplatedParent, field));
            input.Focus();
            input.Text = "2.5, 4; 1 3";
            Assert.Equal("8 4", model.Dashes);
            Key(window, PhysicalKey.Enter);
            Assert.Equal("2.5 4 1 3", model.Dashes);
            Assert.Null(field.SelectedPreset);
            Assert.True(history.Undo());
            Assert.Equal("Dashed", field.SelectedPreset!.Name);
            Assert.False(history.CanUndo());
            field.SelectedPreset = field.Presets[2];
            Assert.Equal("1 3", model.Dashes);
            Assert.True(history.Undo());
            Assert.False(history.CanUndo());
            field.DraftText = "-4 2";
            Assert.False(field.TryCommit());
            Assert.Equal("8 4", model.Dashes);
            field.IsReadOnly = true;
            field.SelectedPreset = field.Presets[0];
            Assert.Equal("8 4", model.Dashes);
            Assert.False(history.CanUndo());
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void AnchorPickerHasTwoDimensionalKeyboardNavigationAndRtl()
    {
        var picker = new StudioAnchorPicker();
        var window = new Window { Width = 100, Height = 100, Content = picker };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.True(picker.ContainerFromIndex(0)!.Focus());
            Key(window, PhysicalKey.ArrowRight);
            Key(window, PhysicalKey.ArrowDown);
            Assert.Equal(4, picker.SelectedIndex);
            Key(window, PhysicalKey.End);
            Assert.Equal(8, picker.SelectedIndex);
            Key(window, PhysicalKey.ArrowRight);
            Assert.Equal(8, picker.SelectedIndex);
            picker.FlowDirection = FlowDirection.RightToLeft;
            Key(window, PhysicalKey.ArrowRight);
            Assert.Equal(7, picker.SelectedIndex);
            Key(window, PhysicalKey.Home);
            Assert.Equal(0, picker.SelectedIndex);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void SelectionControlTracksCollectionReplacementAndStopsDisposedEditors()
    {
        var a = StudioSelectionModelTests.Rectangle(0, 0, 100, 50);
        var b = StudioSelectionModelTests.Rectangle(200, 0, 100, 50);
        var source = new ObservableCollection<BaseShapeViewModel> { a };
        var control = new StudioSelectionEditor { Selection = source };
        var window = new Window { Width = 320, Height = 650, Content = control };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, control.Editor!.Count);
            var old = control.Editor;
            source.Add(b);
            Assert.Equal(2, control.Editor!.Count);
            old.X = 60;
            Assert.Equal(0d, a.TopLeft!.X);
            window.Content = null;
            Assert.Null(control.Editor);
            source.Clear();
            window.Content = control;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, control.Editor!.Count);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void RealDockedMultiSelectionEditsUseTheEditorHistoryAndCancelOldDrafts()
    {
        using var state = new AppState();
        var a = StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        var project = editor.Project!;
        var b = project.CurrentContainer!.CurrentLayer!.Shapes.OfType<RectangleShapeViewModel>().First(x => x != a);
        project.SelectedShapes = new HashSet<BaseShapeViewModel> { a, b };
        var view = new MainView { DataContext = editor };
        var window = new Window { Width = 1440, Height = 900, Content = view };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var selection = Assert.Single(view.GetVisualDescendants().OfType<StudioSelectionEditor>());
            Assert.True(selection.IsVisible);
            Assert.Same(project.History, StudioEditContext.GetHistory(selection));
            var x = selection.GetVisualDescendants().OfType<StudioNumericField>().Single(f => f.Prefix == "X");
            project.History!.Reset();
            double ax = a.TopLeft!.X, bx = b.TopLeft!.X;
            Input(x).Focus();
            Input(x).Text = "+=20";
            Assert.Equal(ax, a.TopLeft.X);
            Key(window, PhysicalKey.Enter);
            Assert.Equal(ax + 20, a.TopLeft.X);
            Assert.Equal(bx + 20, b.TopLeft.X);
            editor.OnUndo();
            Assert.Equal(ax, a.TopLeft.X);
            Assert.Equal(bx, b.TopLeft.X);
            Assert.False(editor.CanUndo());
            Input(x).Text = "+=999";
            project.SelectedShapes = new HashSet<BaseShapeViewModel> { a };
            Dispatcher.UIThread.RunJobs();
            Assert.False(selection.IsVisible);
            Assert.Equal(ax, a.TopLeft.X);
            Assert.Equal(bx, b.TopLeft.X);
            Assert.False(editor.CanUndo());
        }
        finally { window.Close(); }
    }

    internal static TextBox Input(Control control) => control.GetVisualDescendants().OfType<TextBox>().Single();
    internal static void Key(Window window, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        window.KeyPressQwerty(key, modifiers);
        window.KeyReleaseQwerty(key, modifiers);
    }
}
