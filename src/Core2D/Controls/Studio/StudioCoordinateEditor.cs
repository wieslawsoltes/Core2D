// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor;

namespace Core2D.Controls.Studio;

/// <summary>Paired transactional coordinate fields shared by point, arc-size and template inspectors.</summary>
public sealed class StudioCoordinateEditor : TemplatedControl
{
    /// <summary>Defines the original point or size model.</summary>
    public static readonly DirectProperty<StudioCoordinateEditor, ViewModelBase?> SourceProperty =
        AvaloniaProperty.RegisterDirect<StudioCoordinateEditor, ViewModelBase?>(nameof(Source), x => x.Source, (x, value) => x.Source = value);
    /// <summary>Defines the document-scoped presentation adapter.</summary>
    public static readonly DirectProperty<StudioCoordinateEditor, CoordinatePairInspectorViewModel?> EditorProperty =
        AvaloniaProperty.RegisterDirect<StudioCoordinateEditor, CoordinatePairInspectorViewModel?>(nameof(Editor), x => x.Editor);
    private ViewModelBase? _source;
    private CoordinatePairInspectorViewModel? _editor;
    /// <summary>Initializes explicit source and history lifetime handling.</summary>
    public StudioCoordinateEditor() => Interaction.GetBehaviors(this).Add(new StudioCoordinateEditorBehavior());
    /// <summary>Gets or sets the original coordinate model, without cloning it.</summary>
    public ViewModelBase? Source { get => _source; set => SetAndRaise(SourceProperty, ref _source, value); }
    /// <summary>Gets the adapter while the control is attached.</summary>
    public CoordinatePairInspectorViewModel? Editor => _editor;
    internal void Rebind(bool attached)
    {
        CoordinatePairInspectorViewModel? previous = _editor;
        CoordinatePairInspectorViewModel? next = attached && Source is
            ViewModels.Shapes.PointShapeViewModel or ViewModels.Path.PathSizeViewModel or ViewModels.Containers.TemplateContainerViewModel
            ? new CoordinatePairInspectorViewModel(Source!, StudioEditContext.GetHistory(this)) : null;
        // Publish the replacement before disposing the old adapter. Its final CanEdit=false
        // notification must not transiently disable the retained TextBox and steal keyboard focus.
        try { SetAndRaise(EditorProperty, ref _editor, next); }
        finally { previous?.Dispose(); }
    }
}
