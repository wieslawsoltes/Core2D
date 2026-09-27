// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Core2D.Model.Style;

namespace Core2D.Controls.Studio;

/// <summary>Renders a bounded dash pattern without allocating geometry on every frame.</summary>
public class StudioStrokePreview : Control
{
    /// <summary>Defines the model dash pattern.</summary>
    public static readonly StyledProperty<string?> PatternProperty = AvaloniaProperty.Register<StudioStrokePreview, string?>(nameof(Pattern));
    /// <summary>Defines the preview stroke brush.</summary>
    public static readonly StyledProperty<IBrush?> StrokeProperty = AvaloniaProperty.Register<StudioStrokePreview, IBrush?>(nameof(Stroke), Brushes.Gray);
    private Pen? _pen;
    /// <summary>Initializes a solid preview.</summary>
    public StudioStrokePreview() => RebuildPen();
    /// <summary>Gets or sets the dash pattern.</summary>
    public string? Pattern { get => GetValue(PatternProperty); set => SetValue(PatternProperty, value); }
    /// <summary>Gets or sets the stroke brush.</summary>
    public IBrush? Stroke { get => GetValue(StrokeProperty); set => SetValue(StrokeProperty, value); }
    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PatternProperty || change.Property == StrokeProperty) { RebuildPen(); InvalidateVisual(); }
    }
    private void RebuildPen()
    {
        _pen = DashPattern.TryNormalize(Pattern, out _, out double[] lengths)
            ? new Pen(Stroke, 1.5, lengths.Length == 0 ? null : new DashStyle(lengths, 0)) : null;
    }
    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (_pen is not null && Bounds.Width > 16 && Bounds.Height > 4)
            context.DrawLine(_pen, new Point(8, Bounds.Height / 2), new Point(Bounds.Width - 8, Bounds.Height / 2));
    }
}
