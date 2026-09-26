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

/// <summary>Migrates the legacy home perspective while retaining documents and recoverable tool state.</summary>
public static class StudioWorkspaceLayout
{
    /// <summary>Applies the sidebar layout once; later initialization preserves user customization.</summary>
    public static bool Apply(DockFactory factory, ProjectEditorViewModel editor)
    {
        if (factory.RootDock is not { } root || factory.HomeDock is not { VisibleDockables: { } children } home)
        {
            return false;
        }

        StudioDockGraph.RegisterTools(factory, root);
        if (StudioDockGraph.Enumerate(root).Any(x => x.Id is "StudioNavigator" or "StudioInspector"))
        {
            return false;
        }

        var documents = children.Where(ContainsDocuments).ToArray();
        if (documents.Length == 0 && factory.PagesDock is { } pages)
        {
            documents = new IDockable[] { pages };
        }

        var homeRoot = factory.FindRoot(home) ?? root;
        factory.HidePreviewingDockables(homeRoot);
        var tools = new HashSet<ITool>(WalkVisible(home).OfType<ITool>(), ReferenceEqualityComparer.Instance);
        CollectHidden(homeRoot, tools);
        if (!ReferenceEquals(root, homeRoot))
        {
            CollectHidden(root, tools);
        }

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
            // Leave 240 logical pixels for content after Dock's four-pixel frame.
            MinWidth = 244, ActiveDockable = inspector, Alignment = Alignment.Right,
            GripMode = GripMode.Hidden, IsCollapsable = false,
            VisibleDockables = factory.CreateList<IDockable>(inspector)
        };

        foreach (var tool in tools)
        {
            // Tools may have been docked inside a document subtree that will be retained.
            // Remove their old slot before adding them to a sidebar, preserving single ownership.
            if (tool.Owner is IDock { VisibleDockables: { } oldChildren } oldOwner)
            {
                oldChildren.Remove(tool);
                if (ReferenceEquals(oldOwner.ActiveDockable, tool))
                {
                    oldOwner.ActiveDockable = oldChildren.FirstOrDefault(x => x is not ISplitter);
                }
            }
            var owner = IsNavigationTool(tool.Id) ? left : right;
            tool.OriginalOwner = null;
            owner.VisibleDockables!.Add(tool);
        }

        RehomePinned(homeRoot, left, right);
        if (!ReferenceEquals(root, homeRoot))
        {
            RehomePinned(root, left, right);
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

        // Markers make nested initialization idempotent. Register owners before hiding tools.
        factory.InitLayout(root);
        foreach (var tool in tools)
        {
            factory.HideDockable(tool);
        }
        factory.SetActiveDockable(navigator);
        factory.SetActiveDockable(inspector);
        StudioDockGraph.RegisterTools(factory, root);
        return true;
    }

    private static void CollectHidden(IRootDock root, HashSet<ITool> tools)
    {
        if (root.HiddenDockables is not { } hidden)
        {
            return;
        }
        foreach (var tool in hidden.OfType<ITool>().ToArray())
        {
            tools.Add(tool);
            hidden.Remove(tool);
        }
    }

    private static void RehomePinned(IRootDock root, IToolDock left, IToolDock right)
    {
        foreach (var list in new[] { root.LeftPinnedDockables, root.RightPinnedDockables, root.TopPinnedDockables, root.BottomPinnedDockables })
        {
            if (list is null)
            {
                continue;
            }
            foreach (var tool in list)
            {
                tool.Owner = IsNavigationTool(tool.Id) ? left : right;
                tool.OriginalOwner = null;
            }
        }
    }

    private static bool IsNavigationTool(string? id) => id is "ProjectExplorer" or "ObjectBrowser"
        or "BlockLibrary" or "StyleLibrary" or "TemplateLibrary" or "ScriptLibrary" or "DatabaseLibrary";

    private static bool ContainsDocuments(IDockable dockable) => dockable is IDocumentDock
        || dockable is IDock { VisibleDockables: { } children } && children.Any(ContainsDocuments);

    private static IEnumerable<IDockable> WalkVisible(IDockable dockable)
    {
        yield return dockable;
        if (dockable is IDock { VisibleDockables: { } children })
        {
            foreach (var child in children)
            {
                foreach (var item in WalkVisible(child))
                {
                    yield return item;
                }
            }
        }
    }
}
