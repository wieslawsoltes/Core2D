// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Shapes;

namespace Core2D.Controls.Studio;

/// <summary>A compact position/size inspector for shapes represented by two corner points.</summary>
public class StudioBoundsEditor : TemplatedControl
{
    /// <summary>Defines the first existing corner.</summary>
    public static readonly StyledProperty<PointShapeViewModel?> StartProperty = AvaloniaProperty.Register<StudioBoundsEditor, PointShapeViewModel?>(nameof(Start));
    /// <summary>Defines the second existing corner.</summary>
    public static readonly StyledProperty<PointShapeViewModel?> EndProperty = AvaloniaProperty.Register<StudioBoundsEditor, PointShapeViewModel?>(nameof(End));
    /// <summary>Defines the active bounds presentation adapter.</summary>
    public static readonly DirectProperty<StudioBoundsEditor, BoundsInspectorViewModel?> EditorProperty = AvaloniaProperty.RegisterDirect<StudioBoundsEditor, BoundsInspectorViewModel?>(nameof(Editor), x => x.Editor);
    private BoundsInspectorViewModel? _editor;

    /// <summary>Initializes the scoped binding lifecycle.</summary>
    public StudioBoundsEditor() => Interaction.GetBehaviors(this).Add(new StudioBoundsEditorBehavior());
    /// <summary>Gets or sets the first corner.</summary>
    public PointShapeViewModel? Start { get => GetValue(StartProperty); set => SetValue(StartProperty, value); }
    /// <summary>Gets or sets the second corner.</summary>
    public PointShapeViewModel? End { get => GetValue(EndProperty); set => SetValue(EndProperty, value); }
    /// <summary>Gets the current adapter, or null while detached.</summary>
    public BoundsInspectorViewModel? Editor => _editor;

    internal void Rebind(bool attached)
    {
        _editor?.Dispose();
        SetAndRaise(EditorProperty, ref _editor, attached && Start is { } start && End is { } end
            ? new BoundsInspectorViewModel(start, end, StudioEditContext.GetHistory(this)) : null);
    }
}
