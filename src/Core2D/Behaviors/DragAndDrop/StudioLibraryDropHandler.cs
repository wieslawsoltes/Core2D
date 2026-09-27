// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Behaviors.DragAndDrop;

/// <summary>Preserves block/style library copy, move and swap operations for the asset browser.</summary>
public sealed class StudioLibraryDropHandler : DefaultDropHandler
{
    /// <inheritdoc />
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state) =>
        Apply(sender, e, sourceContext, targetContext, false);
    /// <inheritdoc />
    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state) =>
        Apply(sender, e, sourceContext, targetContext, true);

    private static bool Apply(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, bool execute)
    {
        if (sender is not Control control || sourceContext is not ViewModelBase source ||
            source is not (BlockShapeViewModel or ShapeStyleViewModel) || targetContext is not LibraryViewModel library) return false;
        ViewModelBase? target = null;
        for (var visual = control.GetVisualAt(e.GetPosition(control)); visual is not null; visual = visual.GetVisualParent())
            if (visual is Control { DataContext: ViewModelBase item } && library.Items.Contains(item)) { target = item; break; }
        target ??= library.Selected;
        if (target is null || ReferenceEquals(source, target)) return false;
        foreach (ViewModelBase item in library.Items)
            if (source is BlockShapeViewModel ? item is not BlockShapeViewModel : item is not ShapeStyleViewModel) return false;
        int from = library.Items.IndexOf(source), to = library.Items.IndexOf(target);
        if (from < 0 || to < 0) return false;
        ProjectEditorViewModel? editor = null;
        for (var visual = control as Avalonia.Visual; visual is not null; visual = visual.GetVisualParent())
            if (visual is Control { DataContext: ProjectEditorViewModel model }) { editor = model; break; }
        if (editor?.ShapeService is not { } service) return false;
        if (e.DragEffects == DragDropEffects.Copy)
        {
            if (execute && source.CopyShared(null) is { } clone) { clone.Name += "-copy"; service.InsertItem(library, clone, to + 1); }
        }
        else if (e.DragEffects == DragDropEffects.Move) { if (execute) service.MoveItem(library, from, to); }
        else if (e.DragEffects == DragDropEffects.Link) { if (execute) service.SwapItem(library, from, to); }
        else return false;
        return true;
    }
}
