// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;

namespace Core2D.Controls.Studio;

/// <summary>Native, document-scoped inspector for a multiple selection.</summary>
public class StudioSelectionEditor : TemplatedControl
{
    /// <summary>Defines the selected objects, supplied by the document.</summary>
    public static readonly DirectProperty<StudioSelectionEditor, IEnumerable<BaseShapeViewModel>?> SelectionProperty =
        AvaloniaProperty.RegisterDirect<StudioSelectionEditor, IEnumerable<BaseShapeViewModel>?>(nameof(Selection), x => x.Selection, (x, value) => x.Selection = value);
    /// <summary>Defines the current disposable presentation adapter.</summary>
    public static readonly DirectProperty<StudioSelectionEditor, SelectionInspectorViewModel?> EditorProperty =
        AvaloniaProperty.RegisterDirect<StudioSelectionEditor, SelectionInspectorViewModel?>(nameof(Editor), x => x.Editor);
    private IEnumerable<BaseShapeViewModel>? _selection;
    private SelectionInspectorViewModel? _editor;

    /// <summary>Creates the editor and its visual/selection lifecycle.</summary>
    public StudioSelectionEditor() => Interaction.GetBehaviors(this).Add(new StudioSelectionEditorBehavior());
    /// <summary>Gets or sets the selection source.</summary>
    public IEnumerable<BaseShapeViewModel>? Selection { get => _selection; set => SetAndRaise(SelectionProperty, ref _selection, value); }
    /// <summary>Gets the active adapter, or null while detached.</summary>
    public SelectionInspectorViewModel? Editor => _editor;

    internal void Rebind(bool attached)
    {
        _editor?.Dispose();
        SetAndRaise(EditorProperty, ref _editor, attached && Selection is { } selection
            ? new SelectionInspectorViewModel(selection, StudioEditContext.GetHistory(this)) : null);
    }
}
