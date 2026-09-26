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

/// <summary>Contextual state editing for one original shape or a mixed selection.</summary>
public sealed class StudioShapeStateEditor : TemplatedControl
{
    /// <summary>Defines a single source when Selection is not supplied.</summary>
    public static readonly DirectProperty<StudioShapeStateEditor, BaseShapeViewModel?> SourceProperty =
        AvaloniaProperty.RegisterDirect<StudioShapeStateEditor, BaseShapeViewModel?>(nameof(Source), x => x.Source, (x, value) => x.Source = value);
    /// <summary>Defines selected original objects; a non-null collection takes precedence over Source.</summary>
    public static readonly DirectProperty<StudioShapeStateEditor, IEnumerable<BaseShapeViewModel>?> SelectionProperty =
        AvaloniaProperty.RegisterDirect<StudioShapeStateEditor, IEnumerable<BaseShapeViewModel>?>(nameof(Selection), x => x.Selection, (x, value) => x.Selection = value);
    /// <summary>Defines the live, disposable state adapter.</summary>
    public static readonly DirectProperty<StudioShapeStateEditor, ShapeFlagsInspectorViewModel?> EditorProperty =
        AvaloniaProperty.RegisterDirect<StudioShapeStateEditor, ShapeFlagsInspectorViewModel?>(nameof(Editor), x => x.Editor);
    private BaseShapeViewModel? _source;
    private IEnumerable<BaseShapeViewModel>? _selection;
    private ShapeFlagsInspectorViewModel? _editor;
    /// <summary>Initializes source, history and collection lifetime handling.</summary>
    public StudioShapeStateEditor() => Interaction.GetBehaviors(this).Add(new StudioShapeStateEditorBehavior());
    /// <summary>Gets or sets the single source.</summary>
    public BaseShapeViewModel? Source { get => _source; set => SetAndRaise(SourceProperty, ref _source, value); }
    /// <summary>Gets or sets the selection without cloning the underlying objects.</summary>
    public IEnumerable<BaseShapeViewModel>? Selection { get => _selection; set => SetAndRaise(SelectionProperty, ref _selection, value); }
    /// <summary>Gets the adapter while attached.</summary>
    public ShapeFlagsInspectorViewModel? Editor => _editor;
    internal void Rebind(bool attached)
    {
        ShapeFlagsInspectorViewModel? previous = _editor;
        IEnumerable<BaseShapeViewModel> targets = Selection ?? (Source is { } shape ? new[] { shape } : System.Array.Empty<BaseShapeViewModel>());
        ShapeFlagsInspectorViewModel? next = attached ? new ShapeFlagsInspectorViewModel(targets, StudioEditContext.GetHistory(this)) : null;
        try { SetAndRaise(EditorProperty, ref _editor, next); }
        finally { previous?.Dispose(); }
    }
}
