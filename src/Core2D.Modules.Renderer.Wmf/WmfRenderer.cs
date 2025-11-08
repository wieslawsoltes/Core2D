// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.SkiaSharp;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Renderer.Presenters;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Oxage.Wmf;
using Oxage.Wmf.Records;
using SkiaSharp;

namespace Core2D.Modules.Renderer.Wmf;

public partial class WmfRenderer : ViewModelBase, IShapeRenderer, IProjectExporter
{
    private const double SamplingTolerance = 0.5;

    private readonly ExportPresenter _presenter = new();

    [AutoNotify] private ShapeRendererStateViewModel? _state;

    public WmfRenderer(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _state = serviceProvider.GetService<IViewModelFactory>()?.CreateShapeRendererState();
    }

    public override object Copy(IDictionary<object, object>? shared)
        => throw new NotImplementedException();

    public void ClearCache()
    {
    }

    public void Fill(object? dc, double x, double y, double width, double height, BaseColorViewModel? color)
    {
        if (!TryGetContext(dc, out var context) || color is null)
        {
            return;
        }

        if (!TryGetColor(color, out var brushColor))
        {
            return;
        }

        context.SelectBrush(brushColor, true);
        context.SelectPen(Color.Transparent, 0, PenStyle.PS_NULL, false);
        context.Document.AddRectangle((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height));
    }

    public void Grid(object? dc, IGrid grid, double x, double y, double width, double height)
    {
        if (!TryGetContext(dc, out var context) || grid is null)
        {
            return;
        }

        if (!grid.IsGridEnabled || grid.GridCellWidth <= double.Epsilon || grid.GridCellHeight <= double.Epsilon)
        {
            return;
        }

        if (!TryGetColor(grid.GridStrokeColor, out var strokeColor))
        {
            return;
        }

        context.SelectBrush(Color.Transparent, false);
        context.SelectPen(strokeColor, grid.GridStrokeThickness, PenStyle.PS_SOLID, true);

        var left = x + grid.GridOffsetLeft;
        var top = y + grid.GridOffsetTop;
        var right = x + width - grid.GridOffsetRight;
        var bottom = y + height - grid.GridOffsetBottom;

        if (right <= left || bottom <= top)
        {
            return;
        }

        var verticalCount = Math.Max(0, (int)Math.Floor((right - left) / grid.GridCellWidth));
        for (var i = 0; i <= verticalCount; i++)
        {
            var currentX = left + i * grid.GridCellWidth;
            context.Document.AddLine(
                new Point((int)Math.Round(currentX), (int)Math.Round(top)),
                new Point((int)Math.Round(currentX), (int)Math.Round(bottom)));
        }

        var horizontalCount = Math.Max(0, (int)Math.Floor((bottom - top) / grid.GridCellHeight));
        for (var i = 0; i <= horizontalCount; i++)
        {
            var currentY = top + i * grid.GridCellHeight;
            context.Document.AddLine(
                new Point((int)Math.Round(left), (int)Math.Round(currentY)),
                new Point((int)Math.Round(right), (int)Math.Round(currentY)));
        }
    }

    public void DrawPoint(object? dc, PointShapeViewModel point, ShapeStyleViewModel? style)
        => DrawPathShape(dc, point, style);

    public void DrawLine(object? dc, LineShapeViewModel line, ShapeStyleViewModel? style)
        => DrawPathShape(dc, line, style);

    public void DrawWire(object? dc, WireShapeViewModel wire, ShapeStyleViewModel? style)
        => DrawPathShape(dc, wire, style);

    public void DrawRectangle(object? dc, RectangleShapeViewModel rectangle, ShapeStyleViewModel? style)
        => DrawPathShape(dc, rectangle, style);

    public void DrawEllipse(object? dc, EllipseShapeViewModel ellipse, ShapeStyleViewModel? style)
        => DrawPathShape(dc, ellipse, style);

    public void DrawArc(object? dc, ArcShapeViewModel arc, ShapeStyleViewModel? style)
        => DrawPathShape(dc, arc, style);

    public void DrawCubicBezier(object? dc, CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel? style)
        => DrawPathShape(dc, cubicBezier, style);

    public void DrawQuadraticBezier(object? dc, QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel? style)
        => DrawPathShape(dc, quadraticBezier, style);

    public void DrawText(object? dc, TextShapeViewModel text, ShapeStyleViewModel? style)
        => DrawPathShape(dc, text, style);

    public void DrawImage(object? dc, ImageShapeViewModel image, ShapeStyleViewModel? style)
    {
        // WMF renderer currently ignores bitmap content.
    }

    public void DrawPath(object? dc, PathShapeViewModel path, ShapeStyleViewModel? style)
        => DrawPathShape(dc, path, style);

    public void Save(Stream stream, PageContainerViewModel container)
    {
        if (State is null)
        {
            State = ServiceProvider.GetService<IViewModelFactory>()?.CreateShapeRendererState();
        }

        if (State is null)
        {
            return;
        }

        if (container.Template is not { })
        {
            return;
        }

        var width = (int)Math.Max(1, Math.Round(container.Template.Width));
        var height = (int)Math.Max(1, Math.Round(container.Template.Height));

        var context = new WmfRenderContext(width, height);
        try
        {
            var originalFlags = State.DrawShapeState;
            State.DrawShapeState = ShapeStateFlags.Printable;

            _presenter.Render(context, this, null, container.Template, 0, 0);
            _presenter.Render(context, this, null, container, 0, 0);

            State.DrawShapeState = originalFlags;

            context.Dispose();
            context.Document.Save(stream);
        }
        finally
        {
            context.Dispose();
        }
    }

    public void Save(Stream stream, DocumentContainerViewModel document)
    {
        throw new NotSupportedException("Saving documents as wmf drawing is not supported.");
    }

    public void Save(Stream stream, ProjectContainerViewModel project)
    {
        throw new NotSupportedException("Saving projects as wmf drawing is not supported.");
    }

    private void DrawPathShape(object? dc, BaseShapeViewModel shape, ShapeStyleViewModel? style)
    {
        if (!TryGetContext(dc, out var context))
        {
            return;
        }

        var resolvedStyle = style ?? shape.Style;
        if (resolvedStyle is null)
        {
            return;
        }

        using var skPath = PathGeometryConverter.ToSKPath(shape);
        if (skPath is null)
        {
            return;
        }

        var sampled = Approximate(skPath);
        if (sampled.Count < 2)
        {
            return;
        }

        var hasFill = false;
        Color fillColor = default;
        if (shape.IsFilled && resolvedStyle.Fill?.Color is { } fillColorModel &&
            TryGetColor(fillColorModel, out var resolvedFill))
        {
            hasFill = true;
            fillColor = resolvedFill;
        }

        var hasStroke = false;
        Color strokeColor = default;
        if (shape.IsStroked && resolvedStyle.Stroke?.Color is { } strokeColorModel &&
            TryGetColor(strokeColorModel, out var resolvedStroke))
        {
            hasStroke = true;
            strokeColor = resolvedStroke;
        }

        if (hasFill && sampled.Count >= 3)
        {
            var polygon = CreateClosedPolygon(sampled);
            context.SelectBrush(fillColor, true);
            context.SelectPen(Color.Transparent, 0, PenStyle.PS_NULL, false);
            context.Document.AddPolygon(polygon);
        }

        if (hasStroke)
        {
            var thickness = resolvedStyle.Stroke?.Thickness ?? 1.0;
            context.SelectBrush(Color.Transparent, false);
            context.SelectPen(strokeColor, thickness, PenStyle.PS_SOLID, true);
            context.Document.AddPolyline(sampled);
        }
    }

    private static bool TryGetContext(object? dc, out WmfRenderContext context)
    {
        context = dc as WmfRenderContext;
        return context is not null;
    }

    private static bool TryGetColor(BaseColorViewModel? color, out Color result)
    {
        if (color is ArgbColorViewModel argb)
        {
            result = Color.FromArgb(argb.A, argb.R, argb.G, argb.B);
            return true;
        }

        result = Color.Black;
        return false;
    }

    private static List<Point> Approximate(SKPath path)
    {
        var points = new List<Point>();
        using var measure = new SKPathMeasure(path, false);

        if (!AppendContourPoints(measure, points))
        {
            return points;
        }

        while (measure.NextContour())
        {
            AppendContourPoints(measure, points);
        }

        return points;
    }

    private static bool AppendContourPoints(SKPathMeasure measure, IList<Point> points)
    {
        if (!measure.GetPosition(0, out var start))
        {
            return false;
        }

        AddPoint(points, start);

        var length = measure.Length;
        if (length <= float.Epsilon)
        {
            return true;
        }

        var step = Math.Max((float)SamplingTolerance, 0.5f);
        var segments = Math.Max(2, (int)Math.Ceiling(length / step));
        var segmentLength = length / (segments - 1);

        for (var i = 1; i < segments; i++)
        {
            var distance = Math.Min(length, i * segmentLength);
            if (measure.GetPosition(distance, out var position))
            {
                AddPoint(points, position);
            }
        }

        return true;
    }

    private static void AddPoint(IList<Point> points, SKPoint value)
    {
        var point = new Point((int)Math.Round(value.X), (int)Math.Round(value.Y));
        if (points.Count == 0)
        {
            points.Add(point);
            return;
        }

        var last = points[points.Count - 1];
        if (last.X != point.X || last.Y != point.Y)
        {
            points.Add(point);
        }
    }

    private static List<Point> CreateClosedPolygon(List<Point> points)
    {
        var polygon = new List<Point>(points);
        if (points.Count > 2)
        {
            var first = points[0];
            var last = points[points.Count - 1];
            if (first.X != last.X || first.Y != last.Y)
            {
                polygon.Add(first);
            }
        }
        return polygon;
    }
}
