// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.ObjectModel;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Editor;

public sealed class ObjectBrowserState
{
    private readonly ObservableCollection<ObjectBrowserNode> _rootItems = new();
    private readonly ReadOnlyCollection<ObjectBrowserNode> _rootItemsView;
    private readonly ReadOnlyCollection<ObjectBrowserNode> _categoryNodes;
    private ObjectBrowserNode? _rootNode;

    public ObjectBrowserState()
    {
        _categoryNodes = new ReadOnlyCollection<ObjectBrowserNode>(CreateCategoryNodes());
        _rootItemsView = new ReadOnlyCollection<ObjectBrowserNode>(_rootItems);
    }

    public ReadOnlyCollection<ObjectBrowserNode> RootItems => _rootItemsView;

    public void UpdateProject(ProjectContainerViewModel? project)
    {
        _rootItems.Clear();

        if (project is null)
        {
            return;
        }

        _rootNode ??= new ObjectBrowserNode(project.Name, _ => _categoryNodes)
        {
            IsExpanded = true
        };

        _rootNode.Title = project.Name;
        _rootItems.Add(_rootNode);
    }

    private static ObjectBrowserNode[] CreateCategoryNodes()
    {
        return new[]
        {
            new ObjectBrowserNode("Styles", project => project is null
                ? Array.Empty<object>()
                : project.StyleLibraries),
            new ObjectBrowserNode("Blocks", project => project is null
                ? Array.Empty<object>()
                : project.GroupLibraries),
            new ObjectBrowserNode("Databases", project => project is null
                ? Array.Empty<object>()
                : project.Databases),
            new ObjectBrowserNode("Templates", project => project is null
                ? Array.Empty<object>()
                : project.Templates),
            new ObjectBrowserNode("Scripts", project => project is null
                ? Array.Empty<object>()
                : project.Scripts),
            new ObjectBrowserNode("Documents", project => project is null
                ? Array.Empty<object>()
                : project.Documents)
        };
    }
}
