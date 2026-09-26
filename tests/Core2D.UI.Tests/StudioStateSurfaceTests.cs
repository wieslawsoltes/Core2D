using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Core2D.Model.History;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Shapes;
using Core2D.Views;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioStateSurfaceTests
{
    [AvaloniaTheory]
    [InlineData(false)] [InlineData(true)]
    public void StateSwitchesEditMixedOriginalObjectsAndRender(bool dark)
    {
        var first = new RectangleShapeViewModel(null) { Name = "Background", State = ShapeStateFlags.Visible | ShapeStateFlags.Printable };
        var second = new RectangleShapeViewModel(null) { Name = "Connection", State = ShapeStateFlags.Visible | ShapeStateFlags.Connector | ShapeStateFlags.Input };
        IHistory history = new StackHistory();
        var editor = new StudioShapeStateEditor { Selection = new[] { first, second } };
        StudioEditContext.SetHistory(editor, history);
        var window = new Window { Width = 340, Height = 880, Content = editor, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light };
        try
        {
            window.Show(); Jobs();
            var switches = editor.GetVisualDescendants().OfType<StudioSettingToggle>().ToArray();
            Assert.Equal(10, switches.Length);
            Assert.Null(editor.Editor!.Printable);
            var printable = switches.Single(x => AutomationProperties.GetName(x) == "Object printable");
            Assert.Null(printable.IsChecked);
            Assert.True(printable.Focus()); Press(window, PhysicalKey.Space);
            Assert.True(editor.Editor.Printable);
            Assert.True(second.State.HasFlag(ShapeStateFlags.Input));
            Assert.True(history.Undo()); Assert.False(history.CanUndo());
            Assert.Null(editor.Editor.Printable);
            StudioSecondaryViewTests.Capture(window, $"object-state-{(dark ? "dark" : "light")}.png");
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void StateMembershipAndHistoryAreReboundWithoutStaleWriters()
    {
        var first = new RectangleShapeViewModel(null) { State = ShapeStateFlags.Visible };
        var second = new RectangleShapeViewModel(null) { State = ShapeStateFlags.Default };
        var items = new ObservableCollection<BaseShapeViewModel> { first };
        var editor = new StudioShapeStateEditor { Source = second, Selection = items };
        var window = new Window { Width = 340, Height = 700, Content = editor };
        try
        {
            window.Show(); Jobs();
            var old = editor.Editor!;
            items.Add(second); Jobs();
            Assert.Equal(2, editor.Editor!.Count); Assert.False(old.CanEdit);
            old.Visible = false; Assert.True(first.State.HasFlag(ShapeStateFlags.Visible));
            items.Clear(); Jobs(); Assert.Equal(0, editor.Editor.Count);
            editor.Selection = null; Jobs(); Assert.Equal(1, editor.Editor.Count);
            Assert.False(editor.Editor.Visible);
            for (int i = 0; i < 3; i++)
            {
                var previous = editor.Editor;
                window.Content = null;
                Assert.Null(editor.Editor); Assert.False(previous.CanEdit);
                window.Content = editor; Jobs(); Assert.NotSame(previous, editor.Editor);
            }
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void DockedStateTabEditsSelectionRatherThanRendererPreferences()
    {
        using var state = new AppState();
        var shape = StudioScenarioTests.Populate(state);
        var editor = state.Editor!;
        var view = new MainView { DataContext = editor };
        var window = new Window { Width = 1440, Height = 900, Content = view };
        try
        {
            window.Show(); Jobs();
            var tabs = view.GetVisualDescendants().OfType<TabControl>().Single(x => x.Name == "InspectorTabs");
            tabs.SelectedIndex = 2; Jobs();
            var control = view.GetVisualDescendants().OfType<StudioShapeStateEditor>().Single(x => x.IsEffectivelyVisible);
            Assert.Equal(1, control.Editor!.Count);
            Assert.Equal(shape.Name, control.Editor.Title);
            var previousRendererFlags = editor.PageState!.DrawShapeState;
            editor.Project!.History.Reset();
            control.Editor.Locked = !control.Editor.Locked;
            Assert.True(editor.Project.History.CanUndo());
            editor.OnUndo();
            Assert.False(editor.Project.History.CanUndo());
            Assert.Equal(previousRendererFlags, editor.PageState.DrawShapeState);
            tabs.SelectedIndex = 0; Jobs();
            Assert.Contains(view.GetVisualDescendants().OfType<StudioSection>(), x => Equals(x.Header, "Renderer preferences"));
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void MultiSelectionMakesFirstObjectScopeExplicit()
    {
        using var state = new AppState();
        var shape = StudioScenarioTests.Populate(state);
        state.Editor!.Project!.SelectedShapes = new HashSet<BaseShapeViewModel> { shape, new RectangleShapeViewModel(null) { Name = "Other" } };
        var view = new MainView { DataContext = state.Editor };
        var window = new Window { Width = 1440, Height = 900, Content = view };
        try
        {
            window.Show(); Jobs();
            var geometry = view.GetVisualDescendants().OfType<ContentControl>().Single(x => x.Name == "SelectionGeometry");
            Assert.False(geometry.IsEffectivelyVisible);
            var section = view.GetVisualDescendants().OfType<StudioSection>().Single(x => Equals(x.Header, "First selected object"));
            Assert.True(section.IsVisible); Assert.False(section.IsExpanded);
            Assert.Contains(view.GetVisualDescendants().OfType<StudioNotice>(), x => x.Title == "First selected object's style");
        }
        finally { window.Close(); }
    }

    private static void Jobs() => Dispatcher.UIThread.RunJobs();
    private static void Press(Window w, PhysicalKey key) { w.KeyPressQwerty(key, RawInputModifiers.None); w.KeyReleaseQwerty(key, RawInputModifiers.None); Jobs(); }
}
