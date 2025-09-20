// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class GridDrawNode : DrawNode, IGridDrawNode
{
    public SKRect Rect { get; set; }
    public IGrid Grid { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public GridDrawNode(IGrid grid, double x, double y, double width, double height)
    {
        Grid = grid;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = true;
        ScaleSize = false;
        Rect = SKRect.Create(
            (float)(X + Grid.GridOffsetLeft),
            (float)(Y + Grid.GridOffsetTop),
            (float)(Width - Grid.GridOffsetLeft + Grid.GridOffsetRight),
            (float)(Height - Grid.GridOffsetTop + Grid.GridOffsetBottom));
        Center = new SKPoint(Rect.MidX, Rect.MidY);
    }

    public override void UpdateStyle()
    {
        if (Grid.GridStrokeColor is { })
        {
            Stroke = SkiaSharpDrawUtil.ToSKPaintPen(Grid.GridStrokeColor, Grid.GridStrokeThickness);
        }
        else
        {
            Stroke = null;
        }
    }

    public override void Draw(object? dc, double zoom)
    {
        var scale = ScaleSize ? 1.0 / zoom : 1.0;

        double thickness = Grid.GridStrokeThickness;

        if (ScaleThickness)
        {
            thickness /= zoom;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (scale != 1.0)
        {
            thickness /= scale;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (Stroke is { } && Stroke.StrokeWidth != thickness)
        {
            Stroke.StrokeWidth = (float)thickness;
        }

        OnDraw(dc, zoom);
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (Grid.GridStrokeColor is { })
        {
            if (Grid.IsGridEnabled)
            {
                var ox = Rect.Left;
                var ex = Rect.Left + Rect.Width;
                var oy = Rect.Top;
                var ey = Rect.Top + Rect.Height;
                var cw = (float)Grid.GridCellWidth;
                var ch = (float)Grid.GridCellHeight;

                for (var x = ox + cw; x < ex; x += cw)
                {
                    var p0 = new SKPoint(x, oy);
                    var p1 = new SKPoint(x, ey);
                    canvas.DrawLine(p0, p1, Stroke);
                }

                for (var y = oy + ch; y < ey; y += ch)
                {
                    var p0 = new SKPoint(ox, y);
                    var p1 = new SKPoint(ex, y);
                    canvas.DrawLine(p0, p1, Stroke);
                }
            }

            if (Grid.IsBorderEnabled)
            {
                canvas.DrawRect(Rect, Stroke);
            }
        }
    }
}
