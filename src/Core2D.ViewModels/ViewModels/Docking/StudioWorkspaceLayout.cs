// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using Core2D.ViewModels.Docking.Tools;
using Core2D.ViewModels.Editor;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI.Controls;

namespace Core2D.ViewModels.Docking;

/// <summary>Migrates the legacy four-pane home perspective without recreating document models.</summary>
public static class StudioWorkspaceLayout
{
    /// <summary>Applies the studio layout once. Already migrated and user-customized layouts are left alone.</summary>
    public static bool Apply(DockFactory factory, ProjectEditorViewModel editor)
    {
        if (factory.RootDock is not { } root || factory.HomeDock is not { VisibleDockables: { } children } home
            || factory.FindDockable(root, x => x.Id == "StudioNavigator") is not null)
        {
            return false;
        }

        // Retain document subtrees, not just the active page: split groups keep their relative layout.
        var documents = children.Where(ContainsDocuments).ToArray();
        if (documents.Length == 0 && factory.PagesDock is { } pages)
        {
            documents = new IDockable[] { pages };
        }

        var tools = Walk(home).OfType<ITool>().ToList();
        var navigator = new StudioNavigatorViewModel
        {
            Id = "StudioNavigator", Title = "Layers & assets", Context = editor, CanClose = false
        };
        var inspector = new StudioInspectorViewModel
        {
            Id = "StudioInspector", Title = "Design", Context = editor, CanClose = false
        };
        var left = new ToolDock
        {
            Id = "StudioLeftDock", Title = "Navigation", Proportion = 0.19,
            MinWidth = 220, ActiveDockable = navigator, Alignment = Alignment.Left,
            GripMode = GripMode.Hidden, IsCollapsable = false,
            VisibleDockables = factory.CreateList<IDockable>(navigator)
        };
        var right = new ToolDock
        {
            Id = "StudioRightDock", Title = "Inspector", Proportion = 0.21,
            MinWidth = 240, ActiveDockable = inspector, Alignment = Alignment.Right,
            GripMode = GripMode.Hidden, IsCollapsable = false,
            VisibleDockables = factory.CreateList<IDockable>(inspector)
        };

        foreach (var tool in tools)
        {
            var owner = IsNavigationTool(tool.Id) ? left : right;
            owner.VisibleDockables!.Add(tool);
        }

        var layout = new List<IDockable> { left, new ProportionalDockSplitter() };
        foreach (var document in documents)
        {
            if (layout.Count > 2)
            {
                layout.Add(new ProportionalDockSplitter());
            }
            layout.Add(document);
        }
        layout.Add(new ProportionalDockSplitter());
        layout.Add(right);
        home.VisibleDockables = factory.CreateList(layout.ToArray());
        home.ActiveDockable = documents.FirstOrDefault();

        if (factory.FindDockable(root, x => x.Id == "HomeDockDock") is IDock { VisibleDockables: { } shell })
        {
            foreach (var chrome in shell.Where(x => x.Id is "HomeMenuView" or "HomeStatusBarView").ToArray())
            {
                shell.Remove(chrome);
            }
        }

        // The marker panes above make this reinitialization idempotent. Dock registers new owners
        // before hiding advanced tools, so RestoreDockable returns them to a live sidebar.
        factory.InitLayout(root);
        foreach (var tool in tools)
        {
            factory.HideDockable(tool);
        }
        factory.SetActiveDockable(navigator);
        factory.SetActiveDockable(inspector);
        return true;
    }

    private static bool IsNavigationTool(string? id) => id is "ProjectExplorer" or "ObjectBrowser"
        or "BlockLibrary" or "StyleLibrary" or "TemplateLibrary" or "ScriptLibrary" or "DatabaseLibrary";

    private static bool ContainsDocuments(IDockable dockable) => dockable is IDocumentDock
        || dockable is IDock { VisibleDockables: { } children } && children.Any(ContainsDocuments);

    private static IEnumerable<IDockable> Walk(IDockable dockable)
    {
        yield return dockable;
        if (dockable is IDock { VisibleDockables: { } children })
        {
            foreach (var child in children)
            {
                foreach (var item in Walk(child))
                {
                    yield return item;
                }
            }
        }
    }
}
