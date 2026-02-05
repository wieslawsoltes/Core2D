// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia.Controls.DataGridHierarchical;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Wizard.Export.Scopes;

namespace Core2D.Behaviors;

internal static class GridTextHelper
{
    public static string GetDisplayText(object? item)
    {
        if (item is HierarchicalNode hierarchicalNode)
        {
            return GetDisplayText(hierarchicalNode.Item);
        }

        return item switch
        {
            null => string.Empty,
            ExportScopeNodeViewModel scope => scope.Title ?? string.Empty,
            ObjectBrowserNode browserNode => browserNode.Title ?? string.Empty,
            ViewModelBase viewModel => GetViewModelName(viewModel),
            _ => item?.ToString() ?? string.Empty
        };
    }

    public static void TrySetDisplayText(object? item, string? value)
    {
        if (item is HierarchicalNode node)
        {
            TrySetDisplayText(node.Item, value);
            return;
        }

        if (item is ViewModelBase viewModel && value is { })
        {
            viewModel.Name = value;
        }
    }

    private static string GetViewModelName(ViewModelBase viewModel)
    {
        if (!string.IsNullOrWhiteSpace(viewModel.Name))
        {
            return viewModel.Name;
        }

        return viewModel.GetType().Name.Replace("ViewModel", "", System.StringComparison.Ordinal);
    }
}
