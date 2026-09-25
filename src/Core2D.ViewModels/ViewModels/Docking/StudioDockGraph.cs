// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Collections.Generic;
using Dock.Model.Controls;
using Dock.Model.Core;

namespace Core2D.ViewModels.Docking;

/// <summary>Traverses dock state, including nonvisual tool registries and floating windows.</summary>
public static class StudioDockGraph
{
    /// <summary>Enumerates each dockable once without following cyclic owner references.</summary>
    public static IEnumerable<IDockable> Enumerate(IDockable root)
    {
        var pending = new Stack<IDockable>();
        var visited = new HashSet<IDockable>(ReferenceEqualityComparer.Instance);
        pending.Push(root);
        while (pending.TryPop(out var item))
        {
            if (!visited.Add(item))
            {
                continue;
            }
            yield return item;
            if (item is IDock dock)
            {
                Push(dock.VisibleDockables, pending);
            }
            if (item is IRootDock rootDock)
            {
                Push(rootDock.HiddenDockables, pending);
                Push(rootDock.LeftPinnedDockables, pending);
                Push(rootDock.RightPinnedDockables, pending);
                Push(rootDock.TopPinnedDockables, pending);
                Push(rootDock.BottomPinnedDockables, pending);
                if (rootDock.PinnedDock is { } preview)
                {
                    pending.Push(preview);
                }
                if (rootDock.Windows is { } windows)
                {
                    foreach (var window in windows)
                    {
                        if (window.Layout is { } layout)
                        {
                            pending.Push(layout);
                        }
                    }
                }
            }
        }
    }

    /// <summary>Registers stable panel IDs so existing menu commands can restore hidden tools.</summary>
    public static void RegisterTools(DockFactory factory, IDockable root)
    {
        if (factory.DockableLocator is not { } locator)
        {
            return;
        }
        foreach (var item in Enumerate(root))
        {
            if (item is ITool && !string.IsNullOrEmpty(item.Id))
            {
                var tool = item;
                locator[tool.Id] = () => tool;
            }
        }
    }

    private static void Push(IList<IDockable>? items, Stack<IDockable> pending)
    {
        if (items is not null)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                pending.Push(items[i]);
            }
        }
    }
}
