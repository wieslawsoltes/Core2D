using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Core2D.Controls.Studio;
using Core2D.Views.Containers;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioLayerNavigationTests
{
    [AvaloniaFact]
    public void HierarchyToolbarAndInlineTextEditingDoNotInvokeDocumentDelete()
    {
        using var state = new AppState();
        var shape = StudioScenarioTests.Populate(state);
        var project = state.Editor!.Project!;
        var view = new ProjectContainerView { DataContext = project };
        var window = new Window { Width = 340, Height = 600, Content = view };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var grid = view.FindControl<DataGrid>("DocumentsDataGrid")!;
            var hierarchy = Assert.IsType<HierarchicalModel>(grid.HierarchicalModel);
            Click(window, view.FindControl<StudioIconButton>("ExpandLayers")!);
            int expanded = hierarchy.ObservableFlattened.Count;
            Assert.True(expanded > project.Documents.Length);
            // Clear selection so auto-expand-to-selection does not oppose an explicit collapse.
            project.Selected = null;
            Click(window, view.FindControl<StudioIconButton>("CollapseLayers")!);
            Assert.Equal(project.Documents.Length, hierarchy.ObservableFlattened.Count);
            Click(window, view.FindControl<StudioIconButton>("ExpandLayers")!);
            Assert.Equal(expanded, hierarchy.ObservableFlattened.Count);
            project.Selected = shape;
            Click(window, view.FindControl<StudioIconButton>("RevealLayer")!);
            Dispatcher.UIThread.RunJobs();
            Assert.Same(shape, project.Selected);
            var row = view.GetVisualDescendants().OfType<StudioLayerRow>().Single(x => ReferenceEquals(x.Editor?.Item, shape));
            Assert.True(grid.Focus());
            Key(window, PhysicalKey.F2);
            Assert.True(row.IsRenaming);
            var input = row.GetVisualDescendants().OfType<TextBox>().Single();
            Assert.True(input.Focus());
            input.Text = "Temporary name";
            input.SelectAll();
            int count = project.CurrentContainer!.CurrentLayer!.Shapes.Length;
            Key(window, PhysicalKey.Delete);
            Assert.Equal(string.Empty, input.Text);
            Assert.Equal(count, project.CurrentContainer.CurrentLayer.Shapes.Length);
            Assert.Equal("Feature card", shape.Name);
            Key(window, PhysicalKey.Escape);
            Assert.False(row.IsRenaming);
            Assert.Equal("Feature card", shape.Name);
            // Reattach the whole hierarchy: scoped commands must not be restored twice.
            window.Content = null;
            window.Content = view;
            Dispatcher.UIThread.RunJobs();
            Click(window, view.FindControl<StudioIconButton>("ExpandLayers")!);
            Assert.Equal(expanded, Assert.IsType<HierarchicalModel>(grid.HierarchicalModel).ObservableFlattened.Count);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void LayerActionGlyphsTrackStateAndPreserveTheModelFlags()
    {
        using var state = new AppState();
        var shape = StudioScenarioTests.Populate(state);
        var row = new StudioLayerRow { DataContext = shape };
        var window = new Window { Width = 320, Height = 100, Content = row };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var buttons = row.GetVisualDescendants().OfType<StudioIconToggle>().ToArray();
            var visibility = buttons.Single(x => x.Name == "PART_Visible");
            var locking = buttons.Single(x => x.Name == "PART_Lock");
            var visibleIcon = visibility.Icon;
            var unlockedIcon = locking.Icon;
            row.Editor!.IsVisible = false;
            row.Editor.IsLocked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.NotSame(visibleIcon, visibility.Icon);
            Assert.NotSame(unlockedIcon, locking.Icon);
            Assert.Equal(1, visibility.Opacity);
            Assert.Equal(1, locking.Opacity);
        }
        finally { window.Close(); }
    }

    private static void Key(Window window, PhysicalKey key)
    {
        window.KeyPressQwerty(key, RawInputModifiers.None);
        window.KeyReleaseQwerty(key, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
    }
    private static void Click(Window window, Control control)
    {
        Point point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }
}
