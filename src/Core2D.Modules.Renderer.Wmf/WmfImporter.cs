// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.IO;
using Core2D.Model;
using Core2D.ViewModels;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Oxage.Wmf;
using Oxage.Wmf.Records;
using PathFillRule = Core2D.Model.Path.FillRule;

namespace Core2D.Modules.Renderer.Wmf;

public sealed class WmfImporter : IWmfImporter
{
    private readonly IServiceProvider? _serviceProvider;

    public WmfImporter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public WmfImportResult? Import(Stream stream)
    {
        var viewModelFactory = _serviceProvider.GetService<IViewModelFactory>();
        if (viewModelFactory is null)
        {
            return null;
        }

        var document = new WmfDocument();
        document.Load(stream);

        var shapes = new List<BaseShapeViewModel>();
        var styles = new List<ShapeStyleViewModel>();
        var styleCache = new Dictionary<StyleKey, ShapeStyleViewModel>();

        var brushHandles = new Dictionary<int, BrushInfo>();
        var penHandles = new Dictionary<int, PenInfo>();
        BrushInfo? currentBrush = null;
        PenInfo? currentPen = null;
        var nextHandle = 0;

        foreach (var record in document.Records)
        {
            switch (record)
            {
                case WmfCreateBrushIndirectRecord brushRecord:
                {
                    brushHandles[nextHandle++] = new BrushInfo(brushRecord.Color, brushRecord.Style);
                    break;
                }
                case WmfCreatePenIndirectRecord penRecord:
                {
                    penHandles[nextHandle++] = new PenInfo(penRecord.Color, penRecord.Style, Math.Max(1, penRecord.Width.X));
                    break;
                }
                case WmfCreateFontIndirectRecord:
                {
                    nextHandle++;
                    break;
                }
                case WmfSelectObjectRecord selectRecord:
                {
                    if (brushHandles.TryGetValue(selectRecord.ObjectIndex, out var bInfo))
                    {
                        currentBrush = bInfo;
                    }
                    else if (penHandles.TryGetValue(selectRecord.ObjectIndex, out var pInfo))
                    {
                        currentPen = pInfo;
                    }
                    break;
                }
                case WmfDeleteObjectRecord deleteRecord:
                {
                    brushHandles.Remove(deleteRecord.ObjectIndex);
                    penHandles.Remove(deleteRecord.ObjectIndex);
                    break;
                }
                case WmfRectangleRecord rectangleRecord:
                {
                    var rect = rectangleRecord.GetRectangle();
                    var shapeStyle = ResolveStyle(viewModelFactory, currentPen, currentBrush, styleCache, styles);
                    var rectangle = viewModelFactory.CreateRectangleShape(rect.Left, rect.Top, rect.Right, rect.Bottom, shapeStyle, currentPen?.IsVisible ?? false, currentBrush?.IsVisible ?? false);
                    shapes.Add(rectangle);
                    break;
                }
                case WmfEllipseRecord ellipseRecord:
                {
                    var rect = ellipseRecord.GetRectangle();
                    var shapeStyle = ResolveStyle(viewModelFactory, currentPen, currentBrush, styleCache, styles);
                    var ellipse = viewModelFactory.CreateEllipseShape(rect.Left, rect.Top, rect.Right, rect.Bottom, shapeStyle, currentPen?.IsVisible ?? false, currentBrush?.IsVisible ?? false);
                    shapes.Add(ellipse);
                    break;
                }
                case WmfPolygonRecord polygonRecord:
                {
                    var shapeStyle = ResolveStyle(viewModelFactory, currentPen, currentBrush, styleCache, styles);
                    var path = CreatePath(viewModelFactory, polygonRecord.Points, shapeStyle, currentPen?.IsVisible ?? false, currentBrush?.IsVisible ?? false, true);
                    shapes.Add(path);
                    break;
                }
                case WmfPolylineRecord polylineRecord:
                {
                    var shapeStyle = ResolveStyle(viewModelFactory, currentPen, currentBrush, styleCache, styles);
                    var path = CreatePath(viewModelFactory, polylineRecord.Points, shapeStyle, currentPen?.IsVisible ?? false, currentBrush?.IsVisible ?? false, false);
                    shapes.Add(path);
                    break;
                }
            }
        }

        return new WmfImportResult(shapes, styles);
    }

    private static ShapeStyleViewModel ResolveStyle(IViewModelFactory factory, PenInfo? pen, BrushInfo? brush, IDictionary<StyleKey, ShapeStyleViewModel> cache, IList<ShapeStyleViewModel> styles)
    {
        var strokeColor = pen?.Color ?? Color.Black;
        var fillColor = brush?.Color ?? Color.Transparent;
        var thickness = pen?.Width ?? 1;
        var key = new StyleKey(strokeColor.ToArgb(), pen?.IsVisible ?? false, fillColor.ToArgb(), brush?.IsVisible ?? false, thickness);

        if (cache.TryGetValue(key, out var style))
        {
            return style;
        }

        var stroke = factory.CreateArgbColor(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B);
        var fill = factory.CreateArgbColor(fillColor.A, fillColor.R, fillColor.G, fillColor.B);

        style = factory.CreateShapeStyle(
            name: string.Empty,
            stroke,
            fill,
            thickness,
            factory.CreateTextStyle(),
            factory.CreateArrowStyle(),
            factory.CreateArrowStyle());

        cache[key] = style;
        styles.Add(style);
        return style;
    }

    private static PathShapeViewModel CreatePath(IViewModelFactory factory, IList<Point> points, ShapeStyleViewModel style, bool isStroked, bool isFilled, bool isClosed)
    {
        if (points.Count == 0)
        {
            return factory.CreatePathShape(string.Empty, style, ImmutableArray<PathFigureViewModel>.Empty, PathFillRule.Nonzero, isStroked, isFilled && isClosed);
        }

        var path = factory.CreatePathShape(string.Empty, style, ImmutableArray<PathFigureViewModel>.Empty, PathFillRule.Nonzero, isStroked, isFilled && isClosed);
        var geometry = factory.CreateGeometryContext(path);
        var start = factory.CreatePointShape(points[0].X, points[0].Y);
        geometry.BeginFigure(start, isClosed);

        for (var i = 1; i < points.Count; i++)
        {
            geometry.LineTo(factory.CreatePointShape(points[i].X, points[i].Y));
        }

        return path;
    }

    private sealed record BrushInfo(Color Color, BrushStyle Style)
    {
        public bool IsVisible => Style != BrushStyle.BS_NULL;
    }

    private sealed record PenInfo(Color Color, PenStyle Style, int Width)
    {
        public bool IsVisible => Style != PenStyle.PS_NULL;
    }

    private readonly record struct StyleKey(int Stroke, bool IsStroked, int Fill, bool IsFilled, double Thickness);
}
