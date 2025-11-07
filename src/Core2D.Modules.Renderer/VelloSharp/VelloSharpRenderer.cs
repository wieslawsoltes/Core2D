// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.Common;
using Core2D.ViewModels;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using VelloSharp;
using VelloSharp.Rendering;
using PathFillRule = Core2D.Model.Path.FillRule;
using SharpFillRule = VelloSharp.FillRule;

namespace Core2D.Modules.Renderer.VelloSharp;

public sealed class VelloSharpRenderer : ViewModelBase, IShapeRenderer
{
    private const double Epsilon = 0.0001;

    public VelloSharpRenderer(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public ShapeRendererStateViewModel? State { get; set; }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotImplementedException();

    public void ClearCache()
    {
    }

    public void Fill(object? dc, double x, double y, double width, double height, BaseColorViewModel? color)
    {
        if (!TryGetContext(dc, out var context) || !TryGetColor(color, out var rgba))
        {
            return;
        }

        var path = CreateRectanglePath(x, y, width, height);
        context.Scene.FillPath(path, SharpFillRule.NonZero, Matrix3x2.Identity, rgba);
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

        var strokeStyle = new StrokeStyle
        {
            Width = Math.Max(Epsilon, grid.GridStrokeThickness),
            StartCap = LineCap.Butt,
            EndCap = LineCap.Butt,
            LineJoin = LineJoin.Bevel
        };

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
            var line = new PathBuilder()
                .MoveTo(currentX, top)
                .LineTo(currentX, bottom);
            context.Scene.StrokePath(line, strokeStyle, Matrix3x2.Identity, strokeColor);
        }

        var horizontalCount = Math.Max(0, (int)Math.Floor((bottom - top) / grid.GridCellHeight));
        for (int i = 0; i <= horizontalCount; i++)
        {
            var currentY = top + i * grid.GridCellHeight;
            var line = new PathBuilder()
                .MoveTo(left, currentY)
                .LineTo(right, currentY);
            context.Scene.StrokePath(line, strokeStyle, Matrix3x2.Identity, strokeColor);
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

    public void DrawCubicBezier(object? dc, CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel? style)
        => RenderShape(dc, cubicBezier, ResolveStyle(cubicBezier, style, null));

    public void DrawQuadraticBezier(object? dc, QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel? style)
        => RenderShape(dc, quadraticBezier, ResolveStyle(quadraticBezier, style, null));

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
        var fillRule = ConvertFillRule(geometryData.FillRule);

        if (shape.IsFilled && TryGetColor(resolvedStyle.Fill?.Color, out var fillColor))
        {
            context.Scene.FillPath(geometryData.Builder, fillRule, Matrix3x2.Identity, fillColor);
        }

        if (shape.IsStroked && resolvedStyle.Stroke is { Color: { } } strokeStyle && TryGetColor(strokeStyle.Color, out var strokeColor))
        {
            var stroke = CreateStroke(strokeStyle);
            context.Scene.StrokePath(geometryData.Builder, stroke, Matrix3x2.Identity, strokeColor);
        }
    }

    private static bool TryGetContext(object? dc, out VelloSharpRenderContext context)
    {
        context = dc as VelloSharpRenderContext;
        return context is not null;
    }

    private static PathBuilder CreateRectanglePath(double x, double y, double width, double height)
    {
        return new PathBuilder()
            .MoveTo(x, y)
            .LineTo(x + width, y)
            .LineTo(x + width, y + height)
            .LineTo(x, y + height)
            .Close();
    }

    private static StrokeStyle CreateStroke(StrokeStyleViewModel strokeStyle)
    {
        return new StrokeStyle
        {
            Width = Math.Max(Epsilon, strokeStyle.Thickness),
            StartCap = ConvertCap(strokeStyle.LineCap),
            EndCap = ConvertCap(strokeStyle.LineCap),
            LineJoin = LineJoin.Round,
            DashPhase = strokeStyle.DashOffset,
            DashPattern = ParseDashPattern(strokeStyle)
        };
    }

    private static LineCap ConvertCap(Core2D.Model.Style.LineCap cap)
        => cap switch
        {
            Core2D.Model.Style.LineCap.Round => LineCap.Round,
            Core2D.Model.Style.LineCap.Square => LineCap.Square,
            _ => LineCap.Butt
        };

    private static SharpFillRule ConvertFillRule(PathFillRule fillRule)
        => fillRule == PathFillRule.EvenOdd ? SharpFillRule.EvenOdd : SharpFillRule.NonZero;

    private static double[]? ParseDashPattern(StrokeStyleViewModel strokeStyle)
    {
        if (string.IsNullOrWhiteSpace(strokeStyle.Dashes))
        {
            return null;
        }

        var parts = strokeStyle.Dashes.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return null;
        }

        var result = new double[parts.Length];
        var scale = strokeStyle.DashScale <= 0 ? 1.0 : strokeStyle.DashScale;

        for (int i = 0; i < parts.Length; i++)
        {
            if (double.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                result[i] = value * scale;
            }
            else
            {
                result[i] = 0.0;
            }
        }

        return result;
    }

    private static bool TryGetColor(BaseColorViewModel? color, out RgbaColor rgba)
    {
        if (color is ArgbColorViewModel argb)
        {
            rgba = RgbaColor.FromBytes(argb.R, argb.G, argb.B, argb.A);
            return true;
        }

        rgba = default;
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
