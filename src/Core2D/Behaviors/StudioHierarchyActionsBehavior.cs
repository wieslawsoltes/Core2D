// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Containers;

namespace Core2D.Behaviors;

/// <summary>Connects hierarchy toolbar actions to the existing virtualized ProDataGrid model.</summary>
public sealed class StudioHierarchyActionsBehavior : Behavior<Control>
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject?.AddHandler(Button.ClickEvent, OnClick);
        AssociatedObject?.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble);
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        AssociatedObject?.RemoveHandler(Button.ClickEvent, OnClick);
        AssociatedObject?.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
        base.OnDetaching();
    }
    private DataGrid? Grid => AssociatedObject?.FindControl<DataGrid>("DocumentsDataGrid");
    private void OnClick(object? sender, RoutedEventArgs e)
    {
        if (Grid is not { } grid || e.Source is not Button button) return;
        switch (button.Name)
        {
            case "CollapseLayers": grid.HierarchicalModel?.CollapseAll(); break;
            case "ExpandLayers": grid.HierarchicalModel?.ExpandAll(); break;
            case "RevealLayer":
                if (AssociatedObject?.DataContext is not ProjectContainerViewModel project || project.Selected is null) return;
                (grid.HierarchicalModel as HierarchicalModel)?.TryExpandToItem(project.Selected, out _);
                grid.ScrollIntoView(project.Selected, null);
                break;
            default: return;
        }
        e.Handled = true;
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.F2 || AssociatedObject?.DataContext is not ProjectContainerViewModel project) return;
        StudioLayerRow? row = AssociatedObject.GetVisualDescendants().OfType<StudioLayerRow>().FirstOrDefault(x => ReferenceEquals(x.Editor?.Item, project.Selected));
        if (row is null) return;
        row.IsRenaming = true;
        e.Handled = true;
    }
}
