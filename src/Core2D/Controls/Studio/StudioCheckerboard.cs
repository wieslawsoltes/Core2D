// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>Draws a clipped transparency grid without allocating a control for each cell.</summary>
public class StudioCheckerboard : Control
{
    /// <summary>Defines the logical checker size.</summary>
    public static readonly StyledProperty<double> CellSizeProperty = AvaloniaProperty.Register<StudioCheckerboard, double>(
        nameof(CellSize), 4, validate: value => double.IsFinite(value) && value >= 2);
    /// <summary>Defines the first checker brush.</summary>
    public static readonly StyledProperty<IBrush?> LightBrushProperty = AvaloniaProperty.Register<StudioCheckerboard, IBrush?>(nameof(LightBrush));
    /// <summary>Defines the second checker brush.</summary>
    public static readonly StyledProperty<IBrush?> DarkBrushProperty = AvaloniaProperty.Register<StudioCheckerboard, IBrush?>(nameof(DarkBrush));

    static StudioCheckerboard() => AffectsRender<StudioCheckerboard>(CellSizeProperty, LightBrushProperty, DarkBrushProperty);

    /// <summary>Gets or sets the cell size in device-independent pixels.</summary>
    public double CellSize { get => GetValue(CellSizeProperty); set => SetValue(CellSizeProperty, value); }
    /// <summary>Gets or sets the light brush.</summary>
    public IBrush? LightBrush { get => GetValue(LightBrushProperty); set => SetValue(LightBrushProperty, value); }
    /// <summary>Gets or sets the dark brush.</summary>
    public IBrush? DarkBrush { get => GetValue(DarkBrushProperty); set => SetValue(DarkBrushProperty, value); }

    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        Rect area = new(Bounds.Size);
        context.FillRectangle(LightBrush ?? Brushes.White, area);
        // This is a swatch surface. Bound work even if it is accidentally arranged as a large canvas.
        double size = Math.Max(CellSize, Math.Max(area.Width, area.Height) / 128);
        for (int row = 0; row * size < area.Height; row++)
        {
            for (int column = row % 2; column * size < area.Width; column += 2)
            {
                double x = column * size, y = row * size;
                context.FillRectangle(DarkBrush ?? Brushes.LightGray,
                    new Rect(x, y, Math.Min(size, area.Width - x), Math.Min(size, area.Height - y)));
            }
        }
    }
}
