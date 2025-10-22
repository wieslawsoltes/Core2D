// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class GridDrawNode : DrawNode, IGridDrawNode
{
    public A.Rect Rect { get; set; }
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
        Rect = new A.Rect(
            X + Grid.GridOffsetLeft,
            Y + Grid.GridOffsetTop,
            Width - Grid.GridOffsetLeft + Grid.GridOffsetRight,
            Height - Grid.GridOffsetTop + Grid.GridOffsetBottom);
        Center = Rect.Center;
    }

    public override void UpdateStyle()
    {
        if (Grid.GridStrokeColor is { })
        {
            Stroke = AvaloniaDrawUtil.ToPen(Grid.GridStrokeColor, Grid.GridStrokeThickness);
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
        if (Stroke is { } && Stroke.Thickness != thickness)
        {
            if (Grid.GridStrokeColor is { })
            {
                Stroke = AvaloniaDrawUtil.ToPen(Grid.GridStrokeColor, thickness);
            }
            else
            {
                Stroke = null;
            }
        }

        OnDraw(dc, zoom);
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not AM.DrawingContext context)
        {
            return;
        }

        if (Stroke is null)
        {
            return;
        }
        
        if (Grid.GridStrokeColor is { })
        {
            if (Grid.IsGridEnabled)
            {
                var ox = Rect.X;
                var ex = Rect.X + Rect.Width;
                var oy = Rect.Y;
                var ey = Rect.Y + Rect.Height;
                var cw = Grid.GridCellWidth;
                var ch = Grid.GridCellHeight;

                for (var x = ox + cw; x < ex; x += cw)
                {
                    var p0 = new A.Point(x, oy);
                    var p1 = new A.Point(x, ey);
                    context.DrawLine(Stroke, p0, p1);
                }

                for (var y = oy + ch; y < ey; y += ch)
                {
                    var p0 = new A.Point(ox, y);
                    var p1 = new A.Point(ex, y);
                    context.DrawLine(Stroke, p0, p1);
                }
            }

            if (Grid.IsBorderEnabled)
            {
                context.DrawRectangle(null, Stroke, Rect);
            }
        }
    }
}
