// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.Common;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.ViewModels;
using Vello;
using Vello.Geometry;
using VelloSharp;
using Color = Vello.Color;
using PathFillRule = Core2D.Model.Path.FillRule;

namespace Core2D.Modules.Renderer.SparseStrips;

public sealed class SparseStripsRenderer : ViewModelBase, IShapeRenderer
{
    private const double Epsilon = 0.0001;

    public SparseStripsRenderer(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public ShapeRendererStateViewModel? State { get; set; }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotImplementedException();

    public void ClearCache()
    {
        // No caches at the moment.
    }

    public void Fill(object? dc, double x, double y, double width, double height, BaseColorViewModel? color)
    {
        if (!TryGetContext(dc, out var context) || !TryGetColor(color, out var paint))
        {
            return;
        }

        context.Context.SetPaint(paint);
        context.Context.FillRect(new Rect(x, y, width, height));
    }

    public void Grid(object? dc, IGrid grid, double x, double y, double width, double height)
    {
        if (!TryGetContext(dc, out var context) || grid is null)
        {
            return;
        }

        if (!grid.IsGridEnabled || grid.GridCellWidth <= Epsilon || grid.GridCellHeight <= Epsilon)
        {
            return;
        }

        if (!TryGetColor(grid.GridStrokeColor, out var strokeColor))
        {
            return;
        }

        var stroke = new Stroke(
            width: (float)Math.Max(Epsilon, grid.GridStrokeThickness),
            join: Join.Bevel,
            startCap: Cap.Butt,
            endCap: Cap.Butt,
            miterLimit: 4f);

        context.Context.SetStroke(stroke);
        context.Context.SetPaint(strokeColor);

        var left = x + grid.GridOffsetLeft;
        var top = y + grid.GridOffsetTop;
        var right = x + width - grid.GridOffsetRight;
        var bottom = y + height - grid.GridOffsetBottom;

        if (right <= left || bottom <= top)
        {
            return;
        }

        var verticalCount = Math.Max(0, (int)Math.Floor((right - left) / grid.GridCellWidth));
        for (int i = 0; i <= verticalCount; i++)
        {
            var currentX = left + i * grid.GridCellWidth;
            using var line = new BezPath()
                .MoveTo(currentX, top)
                .LineTo(currentX, bottom);
            context.Context.StrokePath(line);
        }

        var horizontalCount = Math.Max(0, (int)Math.Floor((bottom - top) / grid.GridCellHeight));
        for (int i = 0; i <= horizontalCount; i++)
        {
            var currentY = top + i * grid.GridCellHeight;
            using var line = new BezPath()
                .MoveTo(left, currentY)
                .LineTo(right, currentY);
            context.Context.StrokePath(line);
        }
    }

    public void DrawPoint(object? dc, PointShapeViewModel point, ShapeStyleViewModel? style)
        => RenderShape(dc, point, ResolveStyle(point, style, s => s.PointStyle));

    public void DrawLine(object? dc, LineShapeViewModel line, ShapeStyleViewModel? style)
        => RenderShape(dc, line, ResolveStyle(line, style, null));

    public void DrawWire(object? dc, WireShapeViewModel wire, ShapeStyleViewModel? style)
        => RenderShape(dc, wire, ResolveStyle(wire, style, s => s.HelperStyle));

    public void DrawRectangle(object? dc, RectangleShapeViewModel rectangle, ShapeStyleViewModel? style)
        => RenderShape(dc, rectangle, ResolveStyle(rectangle, style, null));

    public void DrawEllipse(object? dc, EllipseShapeViewModel ellipse, ShapeStyleViewModel? style)
        => RenderShape(dc, ellipse, ResolveStyle(ellipse, style, null));

    public void DrawArc(object? dc, ArcShapeViewModel arc, ShapeStyleViewModel? style)
        => RenderShape(dc, arc, ResolveStyle(arc, style, null));

    public void DrawCubicBezier(object? dc, CubicBezierShapeViewModel cubic, ShapeStyleViewModel? style)
        => RenderShape(dc, cubic, ResolveStyle(cubic, style, null));

    public void DrawQuadraticBezier(object? dc, QuadraticBezierShapeViewModel quadratic, ShapeStyleViewModel? style)
        => RenderShape(dc, quadratic, ResolveStyle(quadratic, style, null));

    public void DrawText(object? dc, TextShapeViewModel text, ShapeStyleViewModel? style)
        => RenderShape(dc, text, ResolveStyle(text, style, null));

    public void DrawImage(object? dc, ImageShapeViewModel image, ShapeStyleViewModel? style)
        => RenderShape(dc, image, ResolveStyle(image, style, null));

    public void DrawPath(object? dc, PathShapeViewModel path, ShapeStyleViewModel? style)
        => RenderShape(dc, path, ResolveStyle(path, style, null));

    private void RenderShape(object? dc, BaseShapeViewModel shape, ShapeStyleViewModel? style)
    {
        if (!TryGetContext(dc, out var context) || shape is null)
        {
            return;
        }

        var geometry = PathBuilderFactory.Create(shape);
        if (geometry is null)
        {
            return;
        }

        var resolvedStyle = style ?? shape.Style;
        if (resolvedStyle is null)
        {
            return;
        }

        var geometryData = geometry.Value;

        if (shape.IsFilled && TryGetColor(resolvedStyle.Fill?.Color, out var fillColor))
        {
            DrawFill(context, geometryData, fillColor);
        }

        if (shape.IsStroked && resolvedStyle.Stroke is { Color: { } } strokeStyle && TryGetColor(strokeStyle.Color, out var strokeColor))
        {
            DrawStroke(context, geometryData, strokeStyle, strokeColor);
        }
    }

    private static bool TryGetContext(object? dc, out SparseStripsRenderContext context)
    {
        context = dc as SparseStripsRenderContext;
        return context is not null;
    }

    private static void DrawFill(SparseStripsRenderContext context, PathGeometryData geometry, Color fill)
    {
        using var path = BuildBezPath(geometry.Builder);
        context.Context.SetFillRule(ConvertFillRule(geometry.FillRule));
        context.Context.SetPaint(fill);
        context.Context.FillPath(path);
    }

    private static void DrawStroke(SparseStripsRenderContext context, PathGeometryData geometry, StrokeStyleViewModel strokeStyle, Color color)
    {
        using var path = BuildBezPath(geometry.Builder);
        context.Context.SetFillRule(ConvertFillRule(geometry.FillRule));
        context.Context.SetPaint(color);
        context.Context.SetStroke(CreateStroke(strokeStyle));
        context.Context.StrokePath(path);
    }

    private static Stroke CreateStroke(StrokeStyleViewModel strokeStyle)
    {
        var cap = ConvertCap(strokeStyle.LineCap);
        return new Stroke(
            width: (float)Math.Max(Epsilon, strokeStyle.Thickness),
            join: Join.Round,
            startCap: cap,
            endCap: cap,
            miterLimit: 4f);
    }

    private static Cap ConvertCap(Core2D.Model.Style.LineCap lineCap)
        => lineCap switch
        {
            Core2D.Model.Style.LineCap.Round => Cap.Round,
            Core2D.Model.Style.LineCap.Square => Cap.Square,
            _ => Cap.Butt
        };

    private static Vello.FillRule ConvertFillRule(PathFillRule fillRule)
        => fillRule == PathFillRule.EvenOdd ? Vello.FillRule.EvenOdd : Vello.FillRule.NonZero;

    private static BezPath BuildBezPath(PathBuilder builder)
    {
        var bez = new BezPath();
        foreach (var element in builder.AsSpan())
        {
            switch (element.Verb)
            {
                case PathVerb.MoveTo:
                    bez.MoveTo(element.X0, element.Y0);
                    break;
                case PathVerb.LineTo:
                    bez.LineTo(element.X0, element.Y0);
                    break;
                case PathVerb.QuadTo:
                    bez.QuadTo(element.X0, element.Y0, element.X1, element.Y1);
                    break;
                case PathVerb.CubicTo:
                    bez.CurveTo(element.X0, element.Y0, element.X1, element.Y1, element.X2, element.Y2);
                    break;
                case PathVerb.Close:
                    bez.Close();
                    break;
            }
        }

        return bez;
    }

    private static bool TryGetColor(BaseColorViewModel? color, out Color paint)
    {
        if (color is ArgbColorViewModel argb)
        {
            paint = new Color(argb.R, argb.G, argb.B, argb.A);
            return true;
        }

        paint = default;
        return false;
    }

    private ShapeStyleViewModel? ResolveStyle(BaseShapeViewModel shape, ShapeStyleViewModel? style, Func<ShapeRendererStateViewModel, ShapeStyleViewModel?>? fallback)
    {
        if (style is not null)
        {
            return style;
        }

        if (shape.Style is not null)
        {
            return shape.Style;
        }

        if (fallback is not null && State is not null)
        {
            return fallback(State);
        }

        return null;
    }
}
