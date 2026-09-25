// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>Renders an image's alpha mask in the local foreground color, including per-window themes.</summary>
public class StudioGlyph : Control
{
    /// <summary>Defines the source image. Shared drawing resources are never mutated.</summary>
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<StudioGlyph, IImage?>(nameof(Source));

    /// <summary>Defines the foreground used to tint the glyph.</summary>
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<StudioGlyph, IBrush?>(nameof(Foreground), Brushes.Black);

    // DrawingImage is an IImage, but not an IImageBrushSource. An Image visual supports both
    // vector and bitmap sources without rasterizing or changing their shared drawing resources.
    private readonly VisualBrush _mask = new() { Stretch = Stretch.Uniform };

    static StudioGlyph()
    {
        AffectsRender<StudioGlyph>(SourceProperty, ForegroundProperty);
        AffectsMeasure<StudioGlyph>(SourceProperty);
    }

    /// <summary>Gets or sets the source image.</summary>
    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>Gets or sets the glyph color.</summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Source is null || Foreground is null || Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        var bounds = new Rect(Bounds.Size);
        using (context.PushOpacityMask(_mask, bounds))
        {
            context.DrawRectangle(Foreground, null, bounds);
        }
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize) => Source?.Size ?? default;

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceProperty)
        {
            _mask.Visual = Source is { } image
                ? new Image { Source = image, Stretch = Stretch.Uniform }
                : null;
        }
    }
}
