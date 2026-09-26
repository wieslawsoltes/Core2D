using System.Linq;
using Avalonia.Headless.XUnit;
using Core2D.ViewModels.Docking;
using Dock.Model.Controls;
using Dock.Model.Core;
using Xunit;

namespace Core2D.UI.Tests;

public class StudioLayoutTests
{
    [AvaloniaFact]
    public void EveryAdvancedPanelCanBeRestoredAndHiddenThroughTheExistingCommand()
    {
        using var state = new AppState();
        var editor = state.Editor!;
        editor.OnNewProject();
        var factory = Assert.IsType<DockFactory>(editor.DockFactory);
        var root = Assert.IsAssignableFrom<IDockable>(editor.RootDock);
        var panels = StudioDockGraph.Enumerate(root).OfType<ITool>()
            .Where(x => x.Id is not ("StudioNavigator" or "StudioInspector")).ToArray();
        Assert.Equal(16, panels.Length);
        foreach (var panel in panels)
        {
            Assert.Same(panel, factory.GetDockable<IDockable>(panel.Id));
            editor.OnToggleDockableVisibility(panel.Id);
            var owner = Assert.IsAssignableFrom<IDock>(panel.Owner);
            Assert.Contains(panel, owner.VisibleDockables!);
            editor.OnToggleDockableVisibility(panel.Id);
            Assert.DoesNotContain(panel, owner.VisibleDockables!);
        }
    }

    [AvaloniaFact]
    public void ReinitializingCustomizedOrPinnedSidebarsDoesNotMigrateAgain()
    {
        using var state = new AppState();
        var editor = state.Editor!;
        var factory = Assert.IsType<DockFactory>(editor.DockFactory);
        var home = factory.HomeDock!;
        var left = Assert.IsAssignableFrom<IToolDock>(home.VisibleDockables![0]);
        left.Proportion = 0.28;
        var children = home.VisibleDockables;
        var navigator = factory.GetDockable<IDockable>("StudioNavigator")!;
        factory.PinDockable(navigator);
        Assert.True(factory.IsDockablePinned(navigator));
        factory.InitLayout(Assert.IsAssignableFrom<IDockable>(editor.RootDock));
        Assert.Same(children, home.VisibleDockables);
        Assert.Equal(0.28, left.Proportion);
        Assert.True(factory.IsDockablePinned(navigator));
        Assert.False(StudioWorkspaceLayout.Apply(factory, editor));
    }

    [AvaloniaFact]
    public void PreviouslyHiddenLegacyPanelsGetALiveRestorationOwner()
    {
        using var state = new AppState();
        var editor = state.Editor!;
        var factory = new DockFactory(editor);
        var root = factory.CreateLayout();
        var oldOwner = StudioDockGraph.Enumerate(root).OfType<IToolDock>()
            .Single(x => x.VisibleDockables!.Any(p => p.Id == "RendererOptions"));
        var hidden = oldOwner.VisibleDockables!.Single(x => x.Id == "RendererOptions");
        oldOwner.VisibleDockables.Remove(hidden);
        hidden.Owner = oldOwner;
        root.HiddenDockables = factory.CreateList(hidden);
        editor.DockFactory = factory;
        editor.RootDock = root;
        factory.InitLayout(root);
        editor.OnToggleDockableVisibility("RendererOptions");
        var newOwner = Assert.IsAssignableFrom<IToolDock>(hidden.Owner);
        Assert.Equal("StudioRightDock", newOwner.Id);
        Assert.Contains(hidden, newOwner.VisibleDockables!);
        Assert.Contains(newOwner, StudioDockGraph.Enumerate(root));
    }
}
