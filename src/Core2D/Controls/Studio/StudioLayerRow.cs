// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor;

namespace Core2D.Controls.Studio;

/// <summary>A compact, virtualizable hierarchy row with transactional inline rename and state actions.</summary>
public sealed class StudioLayerRow : TemplatedControl
{
    /// <summary>Defines the optional shape glyph.</summary>
    public static readonly StyledProperty<IImage?> IconProperty = AvaloniaProperty.Register<StudioLayerRow, IImage?>(nameof(Icon));
    /// <summary>Defines the fallback vector glyph for document, page and layer nodes.</summary>
    public static readonly StyledProperty<Geometry?> GlyphProperty = AvaloniaProperty.Register<StudioLayerRow, Geometry?>(nameof(Glyph));
    /// <summary>Defines the live row adapter.</summary>
    public static readonly DirectProperty<StudioLayerRow, StudioLayerRowViewModel?> EditorProperty =
        AvaloniaProperty.RegisterDirect<StudioLayerRow, StudioLayerRowViewModel?>(nameof(Editor), x => x.Editor);
    /// <summary>Defines whether inline rename is active.</summary>
    public static readonly DirectProperty<StudioLayerRow, bool> IsRenamingProperty =
        AvaloniaProperty.RegisterDirect<StudioLayerRow, bool>(nameof(IsRenaming), x => x.IsRenaming, (x, value) => x.IsRenaming = value);
    private StudioLayerRowViewModel? _editor;
    private bool _renaming;
    /// <summary>Initializes input and row-lifetime behavior.</summary>
    public StudioLayerRow() => Interaction.GetBehaviors(this).Add(new StudioLayerRowBehavior());
    /// <summary>Gets or sets the optional shape icon.</summary>
    public IImage? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    /// <summary>Gets or sets the fallback vector icon.</summary>
    public Geometry? Glyph { get => GetValue(GlyphProperty); set => SetValue(GlyphProperty, value); }
    /// <summary>Gets the current adapter.</summary>
    public StudioLayerRowViewModel? Editor => _editor;
    /// <summary>Gets or sets rename mode.</summary>
    public bool IsRenaming { get => _renaming; set => SetAndRaise(IsRenamingProperty, ref _renaming, value); }
    internal void Rebind(bool attached)
    {
        IsRenaming = false;
        _editor?.Dispose();
        SetAndRaise(EditorProperty, ref _editor, attached && DataContext is ViewModelBase item
            ? new StudioLayerRowViewModel(item, StudioEditContext.GetHistory(this)) : null);
    }
}
