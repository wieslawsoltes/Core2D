// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Objects;
using ACadSharp.Tables;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Model.Style;
using Core2D.ViewModels;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Modules.Renderer.Dwg;

public partial class DwgRenderer : ViewModelBase, IShapeRenderer
{
    private readonly ICache<string, ImageDefinition>? _biCache;
    private readonly double _sourceDpi = 96.0;
    private readonly double _targetDpi = 72.0;
    private double _pageHeight;
    private string? _outputPath;
    internal Layer? _currentLayer;

    [AutoNotify] private ShapeRendererStateViewModel? _state;

    public DwgRenderer(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _state = serviceProvider.GetService<IViewModelFactory>()?.CreateShapeRendererState();
        _biCache = serviceProvider.GetService<IViewModelFactory>()?.CreateCache<string, ImageDefinition>();
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    private static readonly double s_lineweightFactor = 96.0 / 2540.0;

    private static readonly short[] s_lineweights = { -3, -2, -1, 0, 5, 9, 13, 15, 18, 20, 25, 30, 35, 40, 50, 53, 60, 70, 80, 90, 100, 106, 120, 140, 158, 200, 211 };

    private static LineWeightType ToLineweight(double thickness)
    {
        var lineweight = (short)(thickness / s_lineweightFactor);
        var nearest = s_lineweights.MinBy(x => Math.Abs((long)x - lineweight));
        return (LineWeightType)nearest;
    }

    private static Color ToColor(BaseColorViewModel colorViewModel)
    {
        return colorViewModel switch
        {
            ArgbColorViewModel argbColor => new Color(argbColor.R, argbColor.G, argbColor.B),
            _ => throw new NotSupportedException($"The {colorViewModel.GetType()} color type is not supported."),
        };
    }

    private static short ToTransparency(BaseColorViewModel colorViewModel)
    {
        switch (colorViewModel)
        {
            case ArgbColorViewModel argbColor:
                return (short)(90.0 - argbColor.A * 90.0 / 255.0);
            default:
                throw new NotSupportedException($"The {colorViewModel.GetType()} color type is not supported.");
        }
    }

    private double ToDwgX(double x) => x;

    private double ToDwgY(double y) => _pageHeight - y;

    private Line CreateLine(double x1, double y1, double x2, double y2)
    {
        var vx1 = ToDwgX(x1);
        var vy1 = ToDwgY(y1);
        var vx2 = ToDwgX(x2);
        var vy2 = ToDwgY(y2);
        return new Line(new CSMath.XYZ(vx1, vy1, 0), new CSMath.XYZ(vx2, vy2, 0));
    }

    private Ellipse CreateEllipseEntity(double x, double y, double width, double height)
    {
        // ACadSharp ellipse uses center+major axis endpoint vector and radius ratio
        var cx = ToDwgX(x + width / 2.0);
        var cy = ToDwgY(y + height / 2.0);
        var center = new CSMath.XYZ(cx, cy, 0);
        var major = Math.Max(width, height) / 2.0;
        var minor = Math.Min(width, height) / 2.0;
        var ratio = minor / major;
        var majorAxis = (height > width)
            ? new CSMath.XYZ(0, major, 0)
            : new CSMath.XYZ(major, 0, 0);

        return new Ellipse
        {
            Center = center,
            MajorAxisEndPoint = majorAxis,
            RadiusRatio = ratio,
            StartParameter = 0.0,
            EndParameter = Math.PI * 2.0
        };
    }

    private void DrawLineInternal(CadDocument doc, Layer layer, ShapeStyleViewModel style, bool isStroked, double x1, double y1, double x2, double y2)
    {
        if (!isStroked || style.Stroke?.Color is null)
        {
            return;
        }

        var stroke = ToColor(style.Stroke.Color);
        var strokeTransparency = ToTransparency(style.Stroke.Color);
        var lineweight = ToLineweight(style.Stroke.Thickness);

        var ln = CreateLine(x1, y1, x2, y2);
        ln.Layer = layer;
        ln.Color = stroke;
        ln.Transparency = new Transparency(strokeTransparency);
        ln.LineWeight = lineweight;
        doc.Entities.Add(ln);
    }

    private void DrawLineInternal(CadDocument doc, Layer layer, BaseColorViewModel colorViewModel, double thickness, double x1, double y1, double x2, double y2)
    {
        var stroke = ToColor(colorViewModel);
        var strokeTransparency = ToTransparency(colorViewModel);
        var lineweight = ToLineweight(thickness);

        var ln = CreateLine(x1, y1, x2, y2);
        ln.Layer = layer;
        ln.Color = stroke;
        ln.Transparency = new Transparency(strokeTransparency);
        ln.LineWeight = lineweight;
        doc.Entities.Add(ln);
    }

    private void FillRectangle(CadDocument doc, Layer layer, double x, double y, double width, double height, BaseColorViewModel colorViewModel)
    {
        var fill = ToColor(colorViewModel);
        var fillTransparency = ToTransparency(colorViewModel);

        var bp = new Hatch.BoundaryPath();
        bp.Edges.Add(new Hatch.BoundaryPath.Line { Start = new CSMath.XY(ToDwgX(x), ToDwgY(y)), End = new CSMath.XY(ToDwgX(x + width), ToDwgY(y)) });
        bp.Edges.Add(new Hatch.BoundaryPath.Line { Start = new CSMath.XY(ToDwgX(x + width), ToDwgY(y)), End = new CSMath.XY(ToDwgX(x + width), ToDwgY(y + height)) });
        bp.Edges.Add(new Hatch.BoundaryPath.Line { Start = new CSMath.XY(ToDwgX(x + width), ToDwgY(y + height)), End = new CSMath.XY(ToDwgX(x), ToDwgY(y + height)) });
        bp.Edges.Add(new Hatch.BoundaryPath.Line { Start = new CSMath.XY(ToDwgX(x), ToDwgY(y + height)), End = new CSMath.XY(ToDwgX(x), ToDwgY(y)) });

        var hatch = new Hatch
        {
            Pattern = HatchPattern.Solid,
        };
        hatch.Paths.Add(bp);
        hatch.Layer = layer;
        hatch.Color = fill;
        hatch.Transparency = new Transparency(fillTransparency);
        doc.Entities.Add(hatch);
    }

    private void StrokeRectangle(CadDocument doc, Layer layer, ShapeStyleViewModel style, double x, double y, double width, double height)
    {
        DrawLineInternal(doc, layer, style, true, x, y, x + width, y);
        DrawLineInternal(doc, layer, style, true, x + width, y, x + width, y + height);
        DrawLineInternal(doc, layer, style, true, x + width, y + height, x, y + height);
        DrawLineInternal(doc, layer, style, true, x, y + height, x, y);
    }

    private void StrokeRectangle(CadDocument doc, Layer layer, BaseColorViewModel colorViewModel, double thickness, double x, double y, double width, double height)
    {
        DrawLineInternal(doc, layer, colorViewModel, thickness, x, y, x + width, y);
        DrawLineInternal(doc, layer, colorViewModel, thickness, x + width, y, x + width, y + height);
        DrawLineInternal(doc, layer, colorViewModel, thickness, x + width, y + height, x, y + height);
        DrawLineInternal(doc, layer, colorViewModel, thickness, x, y + height, x, y);
    }

    private void FillEllipse(CadDocument doc, Layer layer, double x, double y, double width, double height, BaseColorViewModel colorViewModel)
    {
        // Approximate ellipse with a polyline boundary
        var fill = ToColor(colorViewModel);
        var fillTransparency = ToTransparency(colorViewModel);

        int steps = 72;
        var verts = new List<CSMath.XYZ>(steps);
        double cx = x + width / 2.0;
        double cy = y + height / 2.0;
        for (int i = 0; i < steps; i++)
        {
            double t = i * (2.0 * Math.PI) / steps;
            double px = cx + (width / 2.0) * Math.Cos(t);
            double py = cy + (height / 2.0) * Math.Sin(t);
            verts.Add(new CSMath.XYZ(ToDwgX(px), ToDwgY(py), 0));
        }

        var bp = new Hatch.BoundaryPath();
        var pl = new Hatch.BoundaryPath.Polyline
        {
            IsClosed = true,
            Vertices = verts
        };
        bp.Edges.Add(pl);

        var hatch = new Hatch
        {
            Pattern = HatchPattern.Solid,
        };
        hatch.Paths.Add(bp);
        hatch.Layer = layer;
        hatch.Color = fill;
        hatch.Transparency = new Transparency(fillTransparency);
        doc.Entities.Add(hatch);
    }

    private LwPolyline CreatePolyline(IEnumerable<(double x, double y)> points, bool isClosed)
    {
        var pl = new LwPolyline
        {
            IsClosed = isClosed
        };
        foreach (var (x, y) in points)
        {
            pl.Vertices.Add(new LwPolyline.Vertex(new CSMath.XY(ToDwgX(x), ToDwgY(y))));
        }
        return pl;
    }

    private IEnumerable<(double x, double y)> SampleQuadratic(double x0, double y0, double x1, double y1, double x2, double y2, int steps = 24)
    {
        for (int i = 1; i <= steps; i++)
        {
            double t = i / (double)steps;
            double mt = 1 - t;
            double x = mt * mt * x0 + 2 * mt * t * x1 + t * t * x2;
            double y = mt * mt * y0 + 2 * mt * t * y1 + t * t * y2;
            yield return (x, y);
        }
    }

    private IEnumerable<(double x, double y)> SampleCubic(double x0, double y0, double x1, double y1, double x2, double y2, double x3, double y3, int steps = 24)
    {
        for (int i = 1; i <= steps; i++)
        {
            double t = i / (double)steps;
            double mt = 1 - t;
            double x = mt * mt * mt * x0 + 3 * mt * mt * t * x1 + 3 * mt * t * t * x2 + t * t * t * x3;
            double y = mt * mt * mt * y0 + 3 * mt * mt * t * y1 + 3 * mt * t * t * y2 + t * t * t * y3;
            yield return (x, y);
        }
    }

    public void ClearCache()
    {
        _biCache?.Reset();
    }

    public void Fill(object? dc, double x, double y, double width, double height, BaseColorViewModel? colorViewModel)
    {
        if (_currentLayer is null)
        {
            return;
        }

        if (dc is not CadDocument doc)
        {
            return;
        }

        if (colorViewModel is null)
        {
            return;
        }

        FillRectangle(doc, _currentLayer, x, y, width, height, colorViewModel);
    }

    public void Grid(object? dc, IGrid grid, double x, double y, double width, double height)
    {
        if (_currentLayer is null)
        {
            return;
        }

        if (dc is not CadDocument doc)
        {
            return;
        }

        if (grid.GridStrokeColor is null)
        {
            return;
        }

        if (!grid.IsGridEnabled && !grid.IsBorderEnabled)
        {
            return;
        }

        var rect = Spatial.Rect2.FromPoints(
            x + grid.GridOffsetLeft,
            y + grid.GridOffsetTop,
            x + width - grid.GridOffsetLeft + grid.GridOffsetRight,
            y + height - grid.GridOffsetTop + grid.GridOffsetBottom);

        if (grid.IsGridEnabled)
        {
            // vertical lines
            if (grid.GridCellWidth > 0)
            {
                var xStart = rect.X;
                var xEnd = rect.X + rect.Width;
                for (double gx = xStart; gx <= xEnd; gx += grid.GridCellWidth)
                {
                    DrawLineInternal(doc, _currentLayer, grid.GridStrokeColor, grid.GridStrokeThickness, gx, rect.Y, gx, rect.Y + rect.Height);
                }
            }
            // horizontal lines
            if (grid.GridCellHeight > 0)
            {
                var yStart = rect.Y;
                var yEnd = rect.Y + rect.Height;
                for (double gy = yStart; gy <= yEnd; gy += grid.GridCellHeight)
                {
                    DrawLineInternal(doc, _currentLayer, grid.GridStrokeColor, grid.GridStrokeThickness, rect.X, gy, rect.X + rect.Width, gy);
                }
            }
        }

        if (grid.IsBorderEnabled)
        {
            StrokeRectangle(doc, _currentLayer, grid.GridStrokeColor, grid.GridStrokeThickness, rect.X, rect.Y, rect.Width, rect.Height);
        }
    }

    public void DrawPoint(object? dc, PointShapeViewModel point, ShapeStyleViewModel? style)
    {
        // no-op
    }

    public void DrawLine(object? dc, LineShapeViewModel line, ShapeStyleViewModel? style)
    {
        if (_currentLayer is null)
        {
            return;
        }

        if (dc is not CadDocument doc)
        {
            return;
        }
        if (line.Start is null || line.End is null)
        {
            return;
        }

        if (style is null)
        {
            return;
        }

        if (line.IsStroked)
        {
            DrawLineInternal(doc, _currentLayer, style, line.IsStroked, line.Start.X, line.Start.Y, line.End.X, line.End.Y);
        }
    }

    public void DrawRectangle(object? dc, RectangleShapeViewModel rectangle, ShapeStyleViewModel? style)
    {
        if (_currentLayer is null)
        {
            return;
        }

        if (dc is not CadDocument doc)
        {
            return;
        }

        if (rectangle.TopLeft is null || rectangle.BottomRight is null)
        {
            return;
        }

        if (style is null)
        {
            return;
        }

        if (rectangle.IsStroked || rectangle.IsFilled)
        {
            var rect = Spatial.Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y);

            if (rectangle.IsFilled && style.Fill?.Color is { })
            {
                FillRectangle(doc, _currentLayer, rect.X, rect.Y, rect.Width, rect.Height, style.Fill.Color);
            }
            if (rectangle.IsStroked && style.Stroke?.Color is { })
            {
                StrokeRectangle(doc, _currentLayer, style, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }
    }

    public void DrawEllipse(object? dc, EllipseShapeViewModel ellipse, ShapeStyleViewModel? style)
    {
        if (_currentLayer is null)
        {
            return;
        }

        if (dc is not CadDocument doc)
        {
            return;
        }

        if (ellipse.TopLeft is null || ellipse.BottomRight is null)
        {
            return;
        }

        if (style is null)
        {
            return;
        }

        var rect = Spatial.Rect2.FromPoints(
            ellipse.TopLeft.X,
            ellipse.TopLeft.Y,
            ellipse.BottomRight.X,
            ellipse.BottomRight.Y);

        if (ellipse.IsFilled && style.Fill?.Color is { })
        {
            FillEllipse(doc, _currentLayer, rect.X, rect.Y, rect.Width, rect.Height, style.Fill.Color);
        }
        if (ellipse.IsStroked && style.Stroke?.Color is { })
        {
            var e = CreateEllipseEntity(rect.X, rect.Y, rect.Width, rect.Height);
            e.Layer = _currentLayer;
            e.Color = ToColor(style.Stroke.Color);
            e.Transparency = new Transparency(ToTransparency(style.Stroke.Color));
            e.LineWeight = ToLineweight(style.Stroke.Thickness);
            doc.Entities.Add(e);
        }
    }

    public void DrawArc(object? dc, ArcShapeViewModel arc, ShapeStyleViewModel? style)
    {
        if (_currentLayer is null)
        {
            return;
        }

        if (dc is not CadDocument doc)
        {
            return;
        }

        if (arc.Point1 is null || arc.Point2 is null || arc.Point3 is null || arc.Point4 is null)
        {
            return;
        }

        if (style is null)
        {
            return;
        }

        var a = new Spatial.Arc.GdiArc(
            Spatial.Point2.FromXY(arc.Point1.X, arc.Point1.Y),
            Spatial.Point2.FromXY(arc.Point2.X, arc.Point2.Y),
            Spatial.Point2.FromXY(arc.Point3.X, arc.Point3.Y),
            Spatial.Point2.FromXY(arc.Point4.X, arc.Point4.Y));

        // Approximate fill with polyline wedge if needed
        if (arc.IsFilled && style.Fill?.Color is { })
        {
            var fill = ToColor(style.Fill.Color);
            var fillTransparency = ToTransparency(style.Fill.Color);

            int steps = 64;
            var verts = new List<CSMath.XYZ>();
            double cx = a.X + a.Width / 2.0;
            double cy = a.Y + a.Height / 2.0;
            double start = -a.StartAngle * Math.PI / 180.0;
            double end = -a.EndAngle * Math.PI / 180.0;
            if (a.Height > a.Width)
            {
                start += Math.PI / 2.0;
                end += Math.PI / 2.0;
            }
            // ensure direction from start to end CCW
            if (end < start)
            {
                end += 2.0 * Math.PI;
            }
            verts.Add(new CSMath.XYZ(ToDwgX(cx), ToDwgY(cy), 0));
            for (int i = 0; i <= steps; i++)
            {
                double t = start + (end - start) * i / steps;
                double px = cx + (a.Width / 2.0) * Math.Cos(t);
                double py = cy + (a.Height / 2.0) * Math.Sin(t);
                verts.Add(new CSMath.XYZ(ToDwgX(px), ToDwgY(py), 0));
            }
            var bp = new Hatch.BoundaryPath();
            var pl = new Hatch.BoundaryPath.Polyline { IsClosed = true, Vertices = verts };
            bp.Edges.Add(pl);
            var hatch = new Hatch { Pattern = HatchPattern.Solid };
            hatch.Paths.Add(bp);
            hatch.Layer = _currentLayer;
            hatch.Color = fill;
            hatch.Transparency = new Transparency(fillTransparency);
            doc.Entities.Add(hatch);
        }

        if (arc.IsStroked && style.Stroke?.Color is { })
        {
            // Stroke as an elliptical arc using Ellipse with parameters
            var cx = a.X + a.Width / 2.0;
            var cy = a.Y + a.Height / 2.0;
            var major = Math.Max(a.Width, a.Height) / 2.0;
            var minor = Math.Min(a.Width, a.Height) / 2.0;
            var ratio = minor / major;
            var majorAxis = (a.Height > a.Width) ? new CSMath.XYZ(0, major, 0) : new CSMath.XYZ(major, 0, 0);

            var el = new Ellipse
            {
                Center = new CSMath.XYZ(ToDwgX(cx), ToDwgY(cy), 0),
                MajorAxisEndPoint = majorAxis,
                RadiusRatio = ratio,
            };

            double start = -a.StartAngle * Math.PI / 180.0;
            double end = -a.EndAngle * Math.PI / 180.0;
            if (a.Height > a.Width)
            {
                start += Math.PI / 2.0;
                end += Math.PI / 2.0;
            }
            el.StartParameter = start;
            el.EndParameter = end;

            el.Layer = _currentLayer;
            el.Color = ToColor(style.Stroke.Color);
            el.Transparency = new Transparency(ToTransparency(style.Stroke.Color));
            el.LineWeight = ToLineweight(style.Stroke.Thickness);
            doc.Entities.Add(el);
        }
    }

    public void DrawCubicBezier(object? dc, CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel? style)
    {
        if (_currentLayer is null)
        {
            return;
        }
        if (dc is not CadDocument doc)
        {
            return;
        }
        if (cubicBezier.Point1 is null || cubicBezier.Point2 is null || cubicBezier.Point3 is null || cubicBezier.Point4 is null)
        {
            return;
        }
        if (style is null)
        {
            return;
        }
        if (!cubicBezier.IsStroked && !cubicBezier.IsFilled)
        {
            return;
        }

        var pts = new List<(double x, double y)>();
        pts.Add((cubicBezier.Point1.X, cubicBezier.Point1.Y));
        pts.AddRange(SampleCubic(
            cubicBezier.Point1.X,
            cubicBezier.Point1.Y,
            cubicBezier.Point2.X,
            cubicBezier.Point2.Y,
            cubicBezier.Point3.X,
            cubicBezier.Point3.Y,
            cubicBezier.Point4.X,
            cubicBezier.Point4.Y));

        if (cubicBezier.IsFilled && style.Fill?.Color is { })
        {
            var fill = ToColor(style.Fill.Color);
            var fillTransparency = ToTransparency(style.Fill.Color);
            var bp = new Hatch.BoundaryPath();
            var poly = new Hatch.BoundaryPath.Polyline { IsClosed = false };
            poly.Vertices = pts.Select(p => new CSMath.XYZ(ToDwgX(p.x), ToDwgY(p.y), 0)).ToList();
            bp.Edges.Add(poly);
            var hatch = new Hatch { Pattern = HatchPattern.Solid };
            hatch.Paths.Add(bp);
            hatch.Layer = _currentLayer;
            hatch.Color = fill;
            hatch.Transparency = new Transparency(fillTransparency);
            doc.Entities.Add(hatch);
        }

        if (cubicBezier.IsStroked && style.Stroke?.Color is { })
        {
            var pl = CreatePolyline(pts, false);
            pl.Layer = _currentLayer;
            pl.Color = ToColor(style.Stroke.Color);
            pl.Transparency = new Transparency(ToTransparency(style.Stroke.Color));
            pl.LineWeight = ToLineweight(style.Stroke.Thickness);
            doc.Entities.Add(pl);
        }
    }

    public void DrawQuadraticBezier(object? dc, QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel? style)
    {
        if (_currentLayer is null)
        {
            return;
        }
        if (dc is not CadDocument doc)
        {
            return;
        }
        if (quadraticBezier.Point1 is null || quadraticBezier.Point2 is null || quadraticBezier.Point3 is null)
        {
            return;
        }
        if (style is null)
        {
            return;
        }
        if (!quadraticBezier.IsStroked && !quadraticBezier.IsFilled)
        {
            return;
        }

        var pts = new List<(double x, double y)>();
        pts.Add((quadraticBezier.Point1.X, quadraticBezier.Point1.Y));
        pts.AddRange(SampleQuadratic(
            quadraticBezier.Point1.X,
            quadraticBezier.Point1.Y,
            quadraticBezier.Point2.X,
            quadraticBezier.Point2.Y,
            quadraticBezier.Point3.X,
            quadraticBezier.Point3.Y));

        if (quadraticBezier.IsFilled && style.Fill?.Color is { })
        {
            var fill = ToColor(style.Fill.Color);
            var fillTransparency = ToTransparency(style.Fill.Color);
            var bp = new Hatch.BoundaryPath();
            var poly = new Hatch.BoundaryPath.Polyline { IsClosed = false };
            poly.Vertices = pts.Select(p => new CSMath.XYZ(ToDwgX(p.x), ToDwgY(p.y), 0)).ToList();
            bp.Edges.Add(poly);
            var hatch = new Hatch { Pattern = HatchPattern.Solid };
            hatch.Paths.Add(bp);
            hatch.Layer = _currentLayer;
            hatch.Color = fill;
            hatch.Transparency = new Transparency(fillTransparency);
            doc.Entities.Add(hatch);
        }

        if (quadraticBezier.IsStroked && style.Stroke?.Color is { })
        {
            var pl = CreatePolyline(pts, false);
            pl.Layer = _currentLayer;
            pl.Color = ToColor(style.Stroke.Color);
            pl.Transparency = new Transparency(ToTransparency(style.Stroke.Color));
            pl.LineWeight = ToLineweight(style.Stroke.Thickness);
            doc.Entities.Add(pl);
        }
    }

    public void DrawText(object? dc, TextShapeViewModel text, ShapeStyleViewModel? style)
    {
        if (_currentLayer is null)
        {
            return;
        }
        if (dc is not CadDocument doc)
        {
            return;
        }
        if (text.TopLeft is null || text.BottomRight is null)
        {
            return;
        }
        if (style is null || style.TextStyle is null)
        {
            return;
        }
        if (style.Stroke?.Color is null)
        {
            return;
        }

        var rect = Spatial.Rect2.FromPoints(
            text.TopLeft.X,
            text.TopLeft.Y,
            text.BottomRight.X,
            text.BottomRight.Y);

        var stroke = ToColor(style.Stroke.Color);
        var strokeTransparency = ToTransparency(style.Stroke.Color);

        var x = rect.X;
        var y = rect.Y;

        // map alignment
        var attachmentPoint = style.TextStyle.TextVAlignment switch
        {
            TextVAlignment.Center => style.TextStyle.TextHAlignment switch
            {
                TextHAlignment.Center => AttachmentPointType.MiddleCenter,
                TextHAlignment.Right => AttachmentPointType.MiddleRight,
                _ => AttachmentPointType.MiddleLeft,
            },
            TextVAlignment.Bottom => style.TextStyle.TextHAlignment switch
            {
                TextHAlignment.Center => AttachmentPointType.BottomCenter,
                TextHAlignment.Right => AttachmentPointType.BottomRight,
                _ => AttachmentPointType.BottomLeft,
            },
            _ => style.TextStyle.TextHAlignment switch
            {
                TextHAlignment.Center => AttachmentPointType.TopCenter,
                TextHAlignment.Right => AttachmentPointType.TopRight,
                _ => AttachmentPointType.TopLeft,
            },
        };

        var ts = new TextStyle(style.TextStyle.FontName)
        {
            Filename = style.TextStyle.FontFile ?? string.Empty
        };

        var mtext = new MText
        {
            InsertPoint = new CSMath.XYZ(ToDwgX(x), ToDwgY(y), 0),
            Height = style.TextStyle.FontSize * _targetDpi / _sourceDpi,
            RectangleWidth = rect.Width,
            Style = ts,
            AttachmentPoint = attachmentPoint,
            Value = text.Text ?? string.Empty
        };

        mtext.Layer = _currentLayer;
        mtext.Color = stroke;
        mtext.Transparency = new Transparency(strokeTransparency);
        doc.Entities.Add(mtext);
    }

    public void DrawImage(object? dc, ImageShapeViewModel image, ShapeStyleViewModel? style)
    {
        if (_currentLayer is null)
        {
            return;
        }
        if (dc is not CadDocument doc)
        {
            return;
        }
        if (image.TopLeft is null || image.BottomRight is null)
        {
            return;
        }
        if (image.Key is null)
        {
            return;
        }
        var bytes = State?.ImageCache?.GetImage(image.Key);
        if (bytes is null)
        {
            return;
        }
        var rect = Spatial.Rect2.FromPoints(
            image.TopLeft.X,
            image.TopLeft.Y,
            image.BottomRight.X,
            image.BottomRight.Y);

        var defCached = _biCache?.Get(image.Key);
        if (defCached is { })
        {
            var raster = new RasterImage(defCached)
            {
                InsertPoint = new CSMath.XYZ(ToDwgX(rect.X), ToDwgY(rect.Y + rect.Height), 0),
                UVector = new CSMath.XYZ(rect.Width, 0, 0),
                VVector = new CSMath.XYZ(0, -rect.Height, 0),
                Size = new CSMath.XY(rect.Width, rect.Height),
                ShowImage = true
            };
            raster.Layer = _currentLayer;
            doc.Entities.Add(raster);
        }
        else
        {
            if (State?.ImageCache is { } && !string.IsNullOrEmpty(image.Key) && !string.IsNullOrEmpty(_outputPath))
            {
                var path = System.IO.Path.Combine(_outputPath, System.IO.Path.GetFileName(image.Key));
                System.IO.File.WriteAllBytes(path, bytes);
                var def = new ImageDefinition
                {
                    FileName = path,
                    Size = new CSMath.XY((int)rect.Width, (int)rect.Height),
                    DefaultSize = new CSMath.XY(1d, 1d),
                    Units = ResolutionUnit.None
                };
                _biCache?.Set(image.Key, def);

                var raster = new RasterImage(def)
                {
                    InsertPoint = new CSMath.XYZ(ToDwgX(rect.X), ToDwgY(rect.Y + rect.Height), 0),
                    UVector = new CSMath.XYZ(rect.Width, 0, 0),
                    VVector = new CSMath.XYZ(0, -rect.Height, 0),
                    Size = new CSMath.XY(rect.Width, rect.Height),
                    ShowImage = true
                };
                raster.Layer = _currentLayer;
                doc.Entities.Add(raster);
            }
        }
    }

    public void DrawPath(object? dc, PathShapeViewModel path, ShapeStyleViewModel? style)
    {
        if (_currentLayer is null)
        {
            return;
        }
        if (dc is not CadDocument doc)
        {
            return;
        }
        if (style is null)
        {
            return;
        }

        if (!path.IsStroked && !path.IsFilled)
        {
            return;
        }

        var allEntities = new List<Entity>();
        var allBounds = new List<Hatch.BoundaryPath>();

        foreach (var pf in path.Figures)
        {
            if (pf.StartPoint is null)
            {
                continue;
            }

            var points = new List<(double x, double y)>();
            points.Add((pf.StartPoint.X, pf.StartPoint.Y));
            var current = pf.StartPoint;
            foreach (var segment in pf.Segments)
            {
                switch (segment)
                {
                    case LineSegmentViewModel lineSegment when lineSegment.Point is { }:
                        points.Add((lineSegment.Point.X, lineSegment.Point.Y));
                        current = lineSegment.Point;
                        break;
                    case CubicBezierSegmentViewModel cubic when cubic.Point1 is { } && cubic.Point2 is { } && cubic.Point3 is { }:
                        var cubicPts = SampleCubic(current.X, current.Y, cubic.Point1.X, cubic.Point1.Y, cubic.Point2.X, cubic.Point2.Y, cubic.Point3.X, cubic.Point3.Y);
                        points.AddRange(cubicPts);
                        current = cubic.Point3;
                        break;
                    case QuadraticBezierSegmentViewModel quad when quad.Point1 is { } && quad.Point2 is { }:
                        var quadPts = SampleQuadratic(current.X, current.Y, quad.Point1.X, quad.Point1.Y, quad.Point2.X, quad.Point2.Y);
                        points.AddRange(quadPts);
                        current = quad.Point2;
                        break;
                    case ArcSegmentViewModel arcSeg when arcSeg.Point is { } && arcSeg.Size is { }:
                        // Approximate elliptical arc segment
                        var rx = arcSeg.Size.Width;
                        var ry = arcSeg.Size.Height;
                        var end = arcSeg.Point;
                        // simple linear approx fallback
                        points.Add((end.X, end.Y));
                        current = end;
                        break;
                    default:
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                }
            }

            // hatch boundary
            if (points.Count >= 2)
            {
                var bp = new Hatch.BoundaryPath();
                var polyEdge = new Hatch.BoundaryPath.Polyline { IsClosed = pf.IsClosed };
                polyEdge.Vertices = points.Select(p => new CSMath.XYZ(ToDwgX(p.x), ToDwgY(p.y), 0)).ToList();
                bp.Edges.Add(polyEdge);
                allBounds.Add(bp);

                var pl = CreatePolyline(points, pf.IsClosed);
                allEntities.Add(pl);
            }
        }

        if (path.IsFilled && style.Fill?.Color is { } && allBounds.Count > 0)
        {
            var fill = ToColor(style.Fill.Color);
            var fillTransparency = ToTransparency(style.Fill.Color);
            var hatch = new Hatch { Pattern = HatchPattern.Solid };
            foreach (var b in allBounds)
            {
                hatch.Paths.Add(b);
            }
            hatch.Layer = _currentLayer;
            hatch.Color = fill;
            hatch.Transparency = new Transparency(fillTransparency);
            doc.Entities.Add(hatch);
        }

        if (path.IsStroked && style.Stroke?.Color is { })
        {
            var stroke = ToColor(style.Stroke.Color);
            var strokeTransparency = ToTransparency(style.Stroke.Color);
            var lineweight = ToLineweight(style.Stroke.Thickness);

            foreach (var e in allEntities)
            {
                e.Layer = _currentLayer;
                e.Color = stroke;
                e.Transparency = new Transparency(strokeTransparency);
                e.LineWeight = lineweight;
                doc.Entities.Add(e);
            }
        }
    }
}
