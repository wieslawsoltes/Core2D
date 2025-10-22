// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes.Markers;

internal abstract class MarkerBase : IMarker
{
    public BaseShapeViewModel? ShapeViewModel { get; set; }
    public ShapeStyleViewModel? ShapeStyleViewModel { get; set; }
    public ArrowStyleViewModel? Style { get; set; }
    public SKPaint? Brush { get; set; }
    public SKPaint? Pen { get; set; }
    public SKMatrix Rotation { get; set; }
    public SKPoint Point { get; set; }

    public abstract void Draw(object? dc);

    public virtual void UpdateStyle()
    {
        if (ShapeStyleViewModel?.Fill?.Color is { })
        {
            Brush = SkiaSharpDrawUtil.ToSKPaintBrush(ShapeStyleViewModel.Fill.Color);
        }

        if (ShapeStyleViewModel?.Stroke is { })
        {
            Pen = SkiaSharpDrawUtil.ToSKPaintPen(ShapeStyleViewModel, ShapeStyleViewModel.Stroke.Thickness);
        }
    }
}
