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
    // Target block to route entities (paper space layout) when exporting documents.
    private BlockRecord? _targetBlock;

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

    private void AddEntity(CadDocument doc, Entity entity)
    {
        if (_targetBlock is { })
        {
            _targetBlock.Entities.Add(entity);
        }
        else
        {
            doc.Entities.Add(entity);
        }
    }

    private Line CreateLine(double x1, double y1, double x2, double y2)
    {
        var vx1 = ToDwgX(x1);
        var vy1 = ToDwgY(y1);
        var vx2 = ToDwgX(x2);
        var vy2 = ToDwgY(y2);
        return new Line(new CSMath.XYZ(vx1, vy1, 0), new CSMath.XYZ(vx2, vy2, 0));
    }

    private static (double x, double y) RotatePoint(double px, double py, double cx, double cy, double angle)
    {
        var dx = px - cx;
        var dy = py - cy;
        var cos = Math.Cos(angle);
        var sin = Math.Sin(angle);
        var rx = dx * cos - dy * sin + cx;
        var ry = dx * sin + dy * cos + cy;
        return (rx, ry);
    }

    private void DrawArrowHead(CadDocument doc, Layer layer, double x, double y, double angle, ShapeStyleViewModel shapeStyle, ArrowStyleViewModel arrowStyle, bool isFilled)
    {
        if (shapeStyle.Stroke?.Color is null)
        {
            return;
        }
        var color = shapeStyle.Stroke.Color;
        var thickness = shapeStyle.Stroke.Thickness;
        var hasFill = isFilled && shapeStyle.Fill?.Color is { };
        var fillColor = hasFill ? shapeStyle.Fill!.Color : null;

        switch (arrowStyle.ArrowType)
        {
            case Core2D.Model.Style.ArrowType.Arrow:
            {
                var rx2 = 2.0 * arrowStyle.RadiusX;
                var ry2 = 2.0 * arrowStyle.RadiusY;
                var apex = (x, y);
                var b1 = RotatePoint(x - rx2, y + ry2, x, y, angle);
                var b2 = RotatePoint(x - rx2, y - ry2, x, y, angle);
                if (hasFill && fillColor is { })
                {
                    var solid = new Solid
                    {
                        FirstCorner = new CSMath.XYZ(ToDwgX(apex.Item1), ToDwgY(apex.Item2), 0),
                        SecondCorner = new CSMath.XYZ(ToDwgX(b1.x), ToDwgY(b1.y), 0),
                        ThirdCorner = new CSMath.XYZ(ToDwgX(b2.x), ToDwgY(b2.y), 0),
                        FourthCorner = new CSMath.XYZ(ToDwgX(b2.x), ToDwgY(b2.y), 0)
                    };
                    solid.Layer = layer;
                    solid.Color = ToColor(color);
                    solid.Transparency = new Transparency(ToTransparency(color));
                    AddEntity(doc, solid);
                }
                else
                {
                    // Outline V-tip only
                    var (x11, y11) = b1;
                    var (x21, y21) = apex;
                    var (x12, y12) = b2;
                    var (x22, y22) = apex;
                    DrawLineInternal(doc, layer, color, thickness, x11, y11, x21, y21);
                    DrawLineInternal(doc, layer, color, thickness, x12, y12, x22, y22);
                }
                break;
            }
            case Core2D.Model.Style.ArrowType.Rectangle:
            {
                var rx = arrowStyle.RadiusX;
                var ry = arrowStyle.RadiusY;
                var sx = 2.0 * rx;
                var sy = 2.0 * ry;
                var (ltx, lty) = RotatePoint(x - sx, y - sy, x, y, angle);
                var (rtx, rty) = RotatePoint(x, y - sy, x, y, angle);
                var (rbx, rby) = RotatePoint(x, y + sy, x, y, angle);
                var (lbx, lby) = RotatePoint(x - sx, y + sy, x, y, angle);
                DrawLineInternal(doc, layer, color, thickness, ltx, lty, rtx, rty);
                DrawLineInternal(doc, layer, color, thickness, rtx, rty, rbx, rby);
                DrawLineInternal(doc, layer, color, thickness, rbx, rby, lbx, lby);
                DrawLineInternal(doc, layer, color, thickness, lbx, lby, ltx, lty);
                if (hasFill && fillColor is { })
                {
                    var bp = new Hatch.BoundaryPath();
                    var poly = new Hatch.BoundaryPath.Polyline { IsClosed = true };
                    poly.Vertices = new List<CSMath.XYZ>
                    {
                        new(ToDwgX(ltx), ToDwgY(lty), 0),
                        new(ToDwgX(rtx), ToDwgY(rty), 0),
                        new(ToDwgX(rbx), ToDwgY(rby), 0),
                        new(ToDwgX(lbx), ToDwgY(lby), 0)
                    };
                    bp.Edges.Add(poly);
                    var hatch = new Hatch { Pattern = HatchPattern.Solid };
                    hatch.Paths.Add(bp);
                    hatch.Layer = layer;
                    hatch.Color = ToColor(fillColor);
                    hatch.Transparency = new Transparency(ToTransparency(fillColor));
                    AddEntity(doc, hatch);
                }
                break;
            }
            case Core2D.Model.Style.ArrowType.Ellipse:
            {
                // True ellipse cap, rotated around the endpoint
                var rx = arrowStyle.RadiusX;
                var ry = arrowStyle.RadiusY;
                double cx = x - rx;
                double cy = y;
                double major = Math.Max(rx, ry);
                double minor = Math.Min(rx, ry);
                // Major axis vector oriented by angle, account for Y flip on vectors
                double vx = major * Math.Cos(angle);
                double vy = major * Math.Sin(angle);
                var el = new Ellipse
                {
                    Center = new CSMath.XYZ(ToDwgX(cx), ToDwgY(cy), 0),
                    MajorAxisEndPoint = new CSMath.XYZ(vx, -vy, 0),
                    RadiusRatio = major != 0 ? (minor / major) : 1.0,
                    StartParameter = 0.0,
                    EndParameter = 2.0 * Math.PI
                };
                el.Layer = layer;
                el.Color = ToColor(color);
                el.Transparency = new Transparency(ToTransparency(color));
                el.LineWeight = ToLineweight(thickness);
                AddEntity(doc, el);
                if (hasFill && fillColor is { })
                {
                    var bp = new Hatch.BoundaryPath();
                    var edge = new Hatch.BoundaryPath.Ellipse
                    {
                        Center = new CSMath.XY(ToDwgX(cx), ToDwgY(cy)),
                        MajorAxisEndPoint = new CSMath.XY(vx, -vy),
                        MinorToMajorRatio = major != 0 ? (minor / major) : 1.0,
                        StartAngle = 0.0,
                        EndAngle = 2.0 * Math.PI,
                        IsCounterclockwise = true,
                        CounterClockWise = true
                    };
                    bp.Edges.Add(edge);
                    var hatch = new Hatch { Pattern = HatchPattern.Solid };
                    hatch.Paths.Add(bp);
                    hatch.Layer = layer;
                    hatch.Color = ToColor(fillColor);
                    hatch.Transparency = new Transparency(ToTransparency(fillColor));
                    AddEntity(doc, hatch);
                }
                break;
            }
        }
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
        ApplyLineType(doc, ln, style);
        AddEntity(doc, ln);
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
        AddEntity(doc, ln);
    }

    private static IEnumerable<double> ParseDashArray(string? dashes)
    {
        if (string.IsNullOrWhiteSpace(dashes)) yield break;
        var parts = dashes!.Split(new[] { ' ', ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var p in parts)
        {
            if (double.TryParse(p, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v))
            {
                yield return v;
            }
        }
    }

    private static string BuildLineTypeName(IEnumerable<double> pattern, double offset)
    {
        var key = string.Join("_", pattern.Select(v => v.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)));
        return $"LT_{key}_o{offset.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)}";
    }

    private void ApplyLineType(CadDocument doc, Entity entity, ShapeStyleViewModel style)
    {
        var dashString = style.Stroke?.Dashes;
        if (string.IsNullOrWhiteSpace(dashString))
        {
            return;
        }

        var raw = ParseDashArray(dashString).ToList();
        if (raw.Count == 0)
        {
            return;
        }

        // Ensure even-length pattern by repeating if necessary
        if (raw.Count % 2 != 0)
        {
            raw.AddRange(raw);
        }

        // Scale pattern by stroke thickness to match other renderers’ parity
        var thickness = Math.Max(0.0, style.Stroke?.Thickness ?? 0.0);
        var scaled = raw.Select(v => v * (thickness > 0 ? thickness : 1.0)).ToList();

        if (scaled.All(v => v <= 0))
        {
            // Invalid pattern; bail out
            return;
        }

        // Alternate sign per segment: +dash, -gap, +dash, -gap, ...
        var signed = new List<double>(scaled.Count);
        for (int i = 0; i < scaled.Count; i++)
        {
            var len = Math.Abs(scaled[i]);
            if (len == 0)
            {
                signed.Add(0.0); // dot
            }
            else
            {
                signed.Add((i % 2 == 0) ? len : -len);
            }
        }

        // Normalize dash offset to pattern length and rotate/trim the first segment accordingly
        double cycle = signed.Sum(s => Math.Abs(s));
        double dashOffset = style.Stroke?.DashOffset ?? 0.0;
        // Use the same scale as pattern
        double effectiveOffset = (thickness > 0 ? dashOffset * thickness : dashOffset);
        if (cycle > 0 && effectiveOffset != 0)
        {
            double s = effectiveOffset % cycle;
            if (s < 0) s += cycle; // keep positive

            if (s > 0)
            {
                int i = 0;
                var abs = signed.Select(Math.Abs).ToArray();
                while (s > 0 && abs.Length > 0)
                {
                    double seg = abs[i];
                    if (s >= seg)
                    {
                        s -= seg;
                        i = (i + 1) % abs.Length;
                        if (Math.Abs(s - 0) < 1e-9)
                            break;
                        // If we consumed a full cycle, break
                        if (i == 0 && s > 0)
                        {
                            // Remaining offset beyond full cycles is already modded; exit
                            break;
                        }
                    }
                    else
                    {
                        // Trim the first segment
                        double trimmed = seg - s;
                        s = 0;
                        // Build rotated + trimmed list
                        var rotated = new List<double>(signed.Count);
                        double sign = Math.Sign(signed[i]);
                        rotated.Add(sign * trimmed);
                        for (int k = 1; k < signed.Count; k++)
                        {
                            int idx = (i + k) % signed.Count;
                            rotated.Add(signed[idx]);
                        }
                        signed = rotated;
                        break;
                    }
                }
            }
        }

        // Build or reuse the linetype
        var name = BuildLineTypeName(signed.Select(Math.Abs), effectiveOffset);
        if (!doc.LineTypes.TryGetValue(name, out var lt))
        {
            lt = new LineType(name)
            {
                Description = $"Core2D pattern {dashString} (th={thickness:0.###}, off={effectiveOffset:0.###})"
            };
            foreach (var seg in signed)
            {
                lt.AddSegment(new ACadSharp.Tables.LineType.Segment { Length = seg });
            }
            doc.LineTypes.Add(lt);
        }
        entity.LineType = lt;
        if (style.Stroke?.DashScale > 0)
        {
            entity.LineTypeScale = style.Stroke.DashScale;
        }
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
        AddEntity(doc, hatch);
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
        // Use an elliptical hatch boundary edge for fidelity
        var fill = ToColor(colorViewModel);
        var fillTransparency = ToTransparency(colorViewModel);

        double cx = x + width / 2.0;
        double cy = y + height / 2.0;
        double major = Math.Max(width, height) / 2.0;
        double minor = Math.Min(width, height) / 2.0;
        var bp = new Hatch.BoundaryPath();
        var edge = new Hatch.BoundaryPath.Ellipse
        {
            Center = new CSMath.XY(ToDwgX(cx), ToDwgY(cy)),
            MajorAxisEndPoint = (height > width)
                ? new CSMath.XY(0, major)
                : new CSMath.XY(major, 0),
            MinorToMajorRatio = major != 0 ? (minor / major) : 1.0,
            StartAngle = 0.0,
            EndAngle = 2.0 * Math.PI,
            IsCounterclockwise = true,
            CounterClockWise = true
        };
        bp.Edges.Add(edge);

        var hatch = new Hatch
        {
            Pattern = HatchPattern.Solid,
        };
        hatch.Paths.Add(bp);
        hatch.Layer = layer;
        hatch.Color = fill;
        hatch.Transparency = new Transparency(fillTransparency);
        AddEntity(doc, hatch);
    }

    

    private Hatch.BoundaryPath.Spline CreateHatchSplineCubic(double p1x, double p1y, double c1x, double c1y, double c2x, double c2y, double p4x, double p4y)
    {
        var edge = new Hatch.BoundaryPath.Spline
        {
            Degree = 3,
            Rational = false
        };
        edge.ControlPoints.Add(new CSMath.XYZ(ToDwgX(p1x), ToDwgY(p1y), 1));
        edge.ControlPoints.Add(new CSMath.XYZ(ToDwgX(c1x), ToDwgY(c1y), 1));
        edge.ControlPoints.Add(new CSMath.XYZ(ToDwgX(c2x), ToDwgY(c2y), 1));
        edge.ControlPoints.Add(new CSMath.XYZ(ToDwgX(p4x), ToDwgY(p4y), 1));
        edge.Knots.AddRange(new double[] { 0, 0, 0, 0, 1, 1, 1, 1 });
        return edge;
    }

    private Hatch.BoundaryPath.Spline CreateHatchSplineQuadratic(double p1x, double p1y, double c1x, double c1y, double p3x, double p3y)
    {
        var edge = new Hatch.BoundaryPath.Spline
        {
            Degree = 2,
            Rational = false
        };
        edge.ControlPoints.Add(new CSMath.XYZ(ToDwgX(p1x), ToDwgY(p1y), 1));
        edge.ControlPoints.Add(new CSMath.XYZ(ToDwgX(c1x), ToDwgY(c1y), 1));
        edge.ControlPoints.Add(new CSMath.XYZ(ToDwgX(p3x), ToDwgY(p3y), 1));
        edge.Knots.AddRange(new double[] { 0, 0, 0, 1, 1, 1 });
        return edge;
    }

    private Spline CreateEntitySplineCubic(double p1x, double p1y, double c1x, double c1y, double c2x, double c2y, double p4x, double p4y)
    {
        var sp = new Spline
        {
            Degree = 3
        };
        sp.ControlPoints.Add(new CSMath.XYZ(ToDwgX(p1x), ToDwgY(p1y), 0));
        sp.ControlPoints.Add(new CSMath.XYZ(ToDwgX(c1x), ToDwgY(c1y), 0));
        sp.ControlPoints.Add(new CSMath.XYZ(ToDwgX(c2x), ToDwgY(c2y), 0));
        sp.ControlPoints.Add(new CSMath.XYZ(ToDwgX(p4x), ToDwgY(p4y), 0));
        sp.Weights.AddRange(new double[] { 1, 1, 1, 1 });
        sp.Knots.AddRange(new double[] { 0, 0, 0, 0, 1, 1, 1, 1 });
        return sp;
    }

    private Spline CreateEntitySplineQuadratic(double p1x, double p1y, double c1x, double c1y, double p3x, double p3y)
    {
        var sp = new Spline
        {
            Degree = 2
        };
        sp.ControlPoints.Add(new CSMath.XYZ(ToDwgX(p1x), ToDwgY(p1y), 0));
        sp.ControlPoints.Add(new CSMath.XYZ(ToDwgX(c1x), ToDwgY(c1y), 0));
        sp.ControlPoints.Add(new CSMath.XYZ(ToDwgX(p3x), ToDwgY(p3y), 0));
        sp.Weights.AddRange(new double[] { 1, 1, 1 });
        sp.Knots.AddRange(new double[] { 0, 0, 0, 1, 1, 1 });
        return sp;
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

        if (grid.IsGridEnabled && grid.GridCellWidth > 0 && grid.GridCellHeight > 0)
        {
            var pattern = new HatchPattern("GRID");
            // Horizontal lines
            pattern.Lines.Add(new HatchPattern.Line
            {
                Angle = 0.0,
                BasePoint = new CSMath.XY(0, 0),
                Offset = new CSMath.XY(0, grid.GridCellHeight)
            });
            // Vertical lines (90 degrees)
            pattern.Lines.Add(new HatchPattern.Line
            {
                Angle = Math.PI / 2.0,
                BasePoint = new CSMath.XY(0, 0),
                Offset = new CSMath.XY(grid.GridCellWidth, 0)
            });

            var hatch = new Hatch
            {
                Pattern = pattern,
                PatternType = HatchPatternType.PatternFill
            };

            var bp = new Hatch.BoundaryPath();
            bp.Edges.Add(new Hatch.BoundaryPath.Line { Start = new CSMath.XY(ToDwgX(rect.X), ToDwgY(rect.Y)), End = new CSMath.XY(ToDwgX(rect.X + rect.Width), ToDwgY(rect.Y)) });
            bp.Edges.Add(new Hatch.BoundaryPath.Line { Start = new CSMath.XY(ToDwgX(rect.X + rect.Width), ToDwgY(rect.Y)), End = new CSMath.XY(ToDwgX(rect.X + rect.Width), ToDwgY(rect.Y + rect.Height)) });
            bp.Edges.Add(new Hatch.BoundaryPath.Line { Start = new CSMath.XY(ToDwgX(rect.X + rect.Width), ToDwgY(rect.Y + rect.Height)), End = new CSMath.XY(ToDwgX(rect.X), ToDwgY(rect.Y + rect.Height)) });
            bp.Edges.Add(new Hatch.BoundaryPath.Line { Start = new CSMath.XY(ToDwgX(rect.X), ToDwgY(rect.Y + rect.Height)), End = new CSMath.XY(ToDwgX(rect.X), ToDwgY(rect.Y)) });
            hatch.Paths.Add(bp);
            hatch.Layer = _currentLayer;
            hatch.Color = ToColor(grid.GridStrokeColor);
            hatch.Transparency = new Transparency(ToTransparency(grid.GridStrokeColor));
            AddEntity(doc, hatch);
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
            var x1 = line.Start.X;
            var y1 = line.Start.Y;
            var x2 = line.End.X;
            var y2 = line.End.Y;

            var nx1 = x1;
            var ny1 = y1;
            var nx2 = x2;
            var ny2 = y2;

            if (style.Stroke?.StartArrow is { } sa && sa.ArrowType != Core2D.Model.Style.ArrowType.None)
            {
                var a1 = Math.Atan2(y1 - y2, x1 - x2);
                if (sa.ArrowType is Core2D.Model.Style.ArrowType.Rectangle or Core2D.Model.Style.ArrowType.Ellipse)
                {
                    var sx = 2.0 * sa.RadiusX;
                    (nx1, ny1) = RotatePoint(x1 - sx, y1, x1, y1, a1);
                }
                DrawArrowHead(doc, _currentLayer, x1, y1, a1, style, sa, line.IsFilled);
            }

            if (style.Stroke?.EndArrow is { } ea && ea.ArrowType != Core2D.Model.Style.ArrowType.None)
            {
                var a2 = Math.Atan2(y2 - y1, x2 - x1);
                if (ea.ArrowType is Core2D.Model.Style.ArrowType.Rectangle or Core2D.Model.Style.ArrowType.Ellipse)
                {
                    var sx = 2.0 * ea.RadiusX;
                    (nx2, ny2) = RotatePoint(x2 - sx, y2, x2, y2, a2);
                }
                DrawArrowHead(doc, _currentLayer, x2, y2, a2, style, ea, line.IsFilled);
            }

            DrawLineInternal(doc, _currentLayer, style, true, nx1, ny1, nx2, ny2);
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
            ApplyLineType(doc, e, style);
            AddEntity(doc, e);
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

        // Fill with an elliptical arc wedge boundary (ellipse edge + two radial lines)
        if (arc.IsFilled && style.Fill?.Color is { })
        {
            var fill = ToColor(style.Fill.Color);
            var fillTransparency = ToTransparency(style.Fill.Color);
            double cx = a.X + a.Width / 2.0;
            double cy = a.Y + a.Height / 2.0;
            double start = -a.StartAngle * Math.PI / 180.0;
            double end = -a.EndAngle * Math.PI / 180.0;
            if (a.Height > a.Width)
            {
                start += Math.PI / 2.0;
                end += Math.PI / 2.0;
            }
            var bp = new Hatch.BoundaryPath();
            double major = Math.Max(a.Width, a.Height) / 2.0;
            double minor = Math.Min(a.Width, a.Height) / 2.0;
            bool majorVertical = a.Height > a.Width;

            // Elliptical arc edge
            var edge = new Hatch.BoundaryPath.Ellipse
            {
                Center = new CSMath.XY(ToDwgX(cx), ToDwgY(cy)),
                MajorAxisEndPoint = majorVertical ? new CSMath.XY(0, major) : new CSMath.XY(major, 0),
                MinorToMajorRatio = major != 0 ? (minor / major) : 1.0,
                StartAngle = start,
                EndAngle = end,
                IsCounterclockwise = true,
                CounterClockWise = true
            };
            // Compute arc endpoints in model coords
            (double sx, double sy) = majorVertical
                ? (cx + minor * Math.Cos(start), cy + major * Math.Sin(start))
                : (cx + major * Math.Cos(start), cy + minor * Math.Sin(start));
            (double ex, double ey) = majorVertical
                ? (cx + minor * Math.Cos(end), cy + major * Math.Sin(end))
                : (cx + major * Math.Cos(end), cy + minor * Math.Sin(end));

            // Radial edges to close the wedge
            var radial1 = new Hatch.BoundaryPath.Line
            {
                Start = new CSMath.XY(ToDwgX(cx), ToDwgY(cy)),
                End = new CSMath.XY(ToDwgX(sx), ToDwgY(sy))
            };
            var radial2 = new Hatch.BoundaryPath.Line
            {
                Start = new CSMath.XY(ToDwgX(ex), ToDwgY(ey)),
                End = new CSMath.XY(ToDwgX(cx), ToDwgY(cy))
            };

            bp.Edges.Add(radial1);
            bp.Edges.Add(edge);
            bp.Edges.Add(radial2);
            var hatch = new Hatch { Pattern = HatchPattern.Solid };
            hatch.Paths.Add(bp);
            hatch.Layer = _currentLayer;
            hatch.Color = fill;
            hatch.Transparency = new Transparency(fillTransparency);
            AddEntity(doc, hatch);
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
            ApplyLineType(doc, el, style);
            AddEntity(doc, el);
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

        

        if (cubicBezier.IsFilled && style.Fill?.Color is { })
        {
            var fill = ToColor(style.Fill.Color);
            var fillTransparency = ToTransparency(style.Fill.Color);
            var bp = new Hatch.BoundaryPath();
            var edge = CreateHatchSplineCubic(
                cubicBezier.Point1.X, cubicBezier.Point1.Y,
                cubicBezier.Point2.X, cubicBezier.Point2.Y,
                cubicBezier.Point3.X, cubicBezier.Point3.Y,
                cubicBezier.Point4.X, cubicBezier.Point4.Y);
            bp.Edges.Add(edge);
            var hatch = new Hatch { Pattern = HatchPattern.Solid };
            hatch.Paths.Add(bp);
            hatch.Layer = _currentLayer;
            hatch.Color = fill;
            hatch.Transparency = new Transparency(fillTransparency);
            AddEntity(doc, hatch);
        }

        if (cubicBezier.IsStroked && style.Stroke?.Color is { })
        {
            var sp = CreateEntitySplineCubic(
                cubicBezier.Point1.X, cubicBezier.Point1.Y,
                cubicBezier.Point2.X, cubicBezier.Point2.Y,
                cubicBezier.Point3.X, cubicBezier.Point3.Y,
                cubicBezier.Point4.X, cubicBezier.Point4.Y);
            sp.Layer = _currentLayer;
            sp.Color = ToColor(style.Stroke.Color);
            sp.Transparency = new Transparency(ToTransparency(style.Stroke.Color));
            sp.LineWeight = ToLineweight(style.Stroke.Thickness);
            ApplyLineType(doc, sp, style);
            AddEntity(doc, sp);
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

        

        if (quadraticBezier.IsFilled && style.Fill?.Color is { })
        {
            var fill = ToColor(style.Fill.Color);
            var fillTransparency = ToTransparency(style.Fill.Color);
            var bp = new Hatch.BoundaryPath();
            var edge = CreateHatchSplineQuadratic(
                quadraticBezier.Point1.X, quadraticBezier.Point1.Y,
                quadraticBezier.Point2.X, quadraticBezier.Point2.Y,
                quadraticBezier.Point3.X, quadraticBezier.Point3.Y);
            bp.Edges.Add(edge);
            var hatch = new Hatch { Pattern = HatchPattern.Solid };
            hatch.Paths.Add(bp);
            hatch.Layer = _currentLayer;
            hatch.Color = fill;
            hatch.Transparency = new Transparency(fillTransparency);
            AddEntity(doc, hatch);
        }

        if (quadraticBezier.IsStroked && style.Stroke?.Color is { })
        {
            var sp = CreateEntitySplineQuadratic(
                quadraticBezier.Point1.X, quadraticBezier.Point1.Y,
                quadraticBezier.Point2.X, quadraticBezier.Point2.Y,
                quadraticBezier.Point3.X, quadraticBezier.Point3.Y);
            sp.Layer = _currentLayer;
            sp.Color = ToColor(style.Stroke.Color);
            sp.Transparency = new Transparency(ToTransparency(style.Stroke.Color));
            sp.LineWeight = ToLineweight(style.Stroke.Thickness);
            ApplyLineType(doc, sp, style);
            AddEntity(doc, sp);
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
        // Map Core2D font style to ACadSharp TrueType flags for better font handling
        var tf = ACadSharp.Tables.FontFlags.Regular;
        var fs = style.TextStyle.FontStyle;
        if (fs.HasFlag(FontStyleFlags.Bold)) tf |= ACadSharp.Tables.FontFlags.Bold;
        if (fs.HasFlag(FontStyleFlags.Italic)) tf |= ACadSharp.Tables.FontFlags.Italic;
        ts.TrueType = tf;

        string EscapeMText(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            s = s.Replace("\\", "\\\\");
            s = s.Replace("{", "\\{").Replace("}", "\\}");
            s = s.Replace("\r\n", "\\P").Replace("\n", "\\P").Replace("\r", "\\P");
            return s;
        }

        var escaped = EscapeMText(text.Text ?? string.Empty);
        var fontName = style.TextStyle.FontName ?? "Arial";
        var bold = fs.HasFlag(FontStyleFlags.Bold) ? 1 : 0;
        var italic = fs.HasFlag(FontStyleFlags.Italic) ? 1 : 0;
        var inlinePrefix = $"\\f{fontName}|b{bold}|i{italic};";
        string WrapIf(bool flag, string prefix, string suffix, string s) => flag ? prefix + s + suffix : s;

        bool getBooleanFlag(object obj, string name)
        {
            try
            {
                var prop = obj.GetType().GetProperty(name);
                if (prop is { } && prop.PropertyType == typeof(bool))
                {
                    return (bool)(prop.GetValue(obj) ?? false);
                }
            }
            catch { }
            return false;
        }

        var underline = getBooleanFlag(style.TextStyle, "Underline") || getBooleanFlag(style.TextStyle, "IsUnderline");
        var overline = getBooleanFlag(style.TextStyle, "Overline") || getBooleanFlag(style.TextStyle, "IsOverline");

        var inlineValue = $"{{{inlinePrefix}{escaped}}}";
        inlineValue = WrapIf(underline, "\\L", "\\l", inlineValue);
        inlineValue = WrapIf(overline, "\\O", "\\o", inlineValue);

        // Convert CSS px font size to drawing units (mm) using 96 DPI
        var mmPerPx = 25.4 / _sourceDpi;
        var textHeight = style.TextStyle.FontSize * mmPerPx;
        var mtext = new MText
        {
            InsertPoint = new CSMath.XYZ(ToDwgX(x), ToDwgY(y), 0),
            Height = textHeight,
            RectangleWidth = rect.Width,
            Style = ts,
            AttachmentPoint = attachmentPoint,
            Value = inlineValue
        };

        bool useBg = getBooleanFlag(style.TextStyle, "UseTextBackground") || getBooleanFlag(style.TextStyle, "TextBackground");
        if (useBg)
        {
            mtext.BackgroundFillFlags = BackgroundFillFlags.UseBackgroundFillColor;
            if (style.Fill?.Color is { })
            {
                mtext.BackgroundColor = ToColor(style.Fill.Color);
            }
            mtext.BackgroundScale = 1.5;
        }

        mtext.Layer = _currentLayer;
        mtext.Color = stroke;
        mtext.Transparency = new Transparency(strokeTransparency);
        AddEntity(doc, mtext);
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
            AddEntity(doc, raster);
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
                AddEntity(doc, raster);
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

            var current = pf.StartPoint;
            var bp = new Hatch.BoundaryPath();
            var strokeSegs = new List<Entity>();
            var lwVertices = new List<(double x, double y)> { (pf.StartPoint.X, pf.StartPoint.Y) };
            bool onlyLines = true;
            foreach (var segment in pf.Segments)
            {
                switch (segment)
                {
                    case LineSegmentViewModel lineSegment when lineSegment.Point is { }:
                        bp.Edges.Add(new Hatch.BoundaryPath.Line
                        {
                            Start = new CSMath.XY(ToDwgX(current.X), ToDwgY(current.Y)),
                            End = new CSMath.XY(ToDwgX(lineSegment.Point.X), ToDwgY(lineSegment.Point.Y))
                        });
                        lwVertices.Add((lineSegment.Point.X, lineSegment.Point.Y));
                        current = lineSegment.Point;
                        break;
                    case CubicBezierSegmentViewModel cubic when cubic.Point1 is { } && cubic.Point2 is { } && cubic.Point3 is { }:
                        bp.Edges.Add(CreateHatchSplineCubic(
                            current.X, current.Y,
                            cubic.Point1.X, cubic.Point1.Y,
                            cubic.Point2.X, cubic.Point2.Y,
                            cubic.Point3.X, cubic.Point3.Y));
                        strokeSegs.Add(CreateEntitySplineCubic(
                            current.X, current.Y,
                            cubic.Point1.X, cubic.Point1.Y,
                            cubic.Point2.X, cubic.Point2.Y,
                            cubic.Point3.X, cubic.Point3.Y));
                        onlyLines = false;
                        current = cubic.Point3;
                        break;
                    case QuadraticBezierSegmentViewModel quad when quad.Point1 is { } && quad.Point2 is { }:
                        bp.Edges.Add(CreateHatchSplineQuadratic(
                            current.X, current.Y,
                            quad.Point1.X, quad.Point1.Y,
                            quad.Point2.X, quad.Point2.Y));
                        strokeSegs.Add(CreateEntitySplineQuadratic(
                            current.X, current.Y,
                            quad.Point1.X, quad.Point1.Y,
                            quad.Point2.X, quad.Point2.Y));
                        onlyLines = false;
                        current = quad.Point2;
                        break;
                    case ArcSegmentViewModel arcSeg when arcSeg.Point is { } && arcSeg.Size is { }:
                        // Convert WPF/SVG arc segment to ellipse parameters
                        var x1 = current.X; var y1 = current.Y;
                        var x2 = arcSeg.Point.X; var y2 = arcSeg.Point.Y;
                        var rx = Math.Abs(arcSeg.Size.Width);
                        var ry = Math.Abs(arcSeg.Size.Height);
                        if (rx <= 0 || ry <= 0)
                        {
                            bp.Edges.Add(new Hatch.BoundaryPath.Line
                            {
                                Start = new CSMath.XY(ToDwgX(x1), ToDwgY(y1)),
                                End = new CSMath.XY(ToDwgX(x2), ToDwgY(y2))
                            });
                            lwVertices.Add((x2, y2));
                            current = arcSeg.Point;
                            break;
                        }
                        double phi = arcSeg.RotationAngle * Math.PI / 180.0;
                        double cosphi = Math.Cos(phi), sinphi = Math.Sin(phi);
                        double dx2 = (x1 - x2) / 2.0;
                        double dy2 = (y1 - y2) / 2.0;
                        double x1p = cosphi * dx2 + sinphi * dy2;
                        double y1p = -sinphi * dx2 + cosphi * dy2;
                        double rx2 = rx * rx, ry2 = ry * ry;
                        double x1p2 = x1p * x1p, y1p2 = y1p * y1p;
                        double lambda = (x1p2 / rx2) + (y1p2 / ry2);
                        if (lambda > 1)
                        {
                            double s = Math.Sqrt(lambda);
                            rx *= s; ry *= s; rx2 = rx * rx; ry2 = ry * ry;
                        }
                        double sign = (arcSeg.IsLargeArc != (arcSeg.SweepDirection == Core2D.Model.Path.SweepDirection.Clockwise)) ? 1.0 : -1.0;
                        double num = rx2 * ry2 - rx2 * y1p2 - ry2 * x1p2;
                        double den = rx2 * y1p2 + ry2 * x1p2;
                        double coef = (den == 0) ? 0 : sign * Math.Sqrt(Math.Max(0, num / den));
                        double cxp = coef * (rx * y1p / ry);
                        double cyp = coef * (-ry * x1p / rx);
                        double cx = cosphi * cxp - sinphi * cyp + (x1 + x2) / 2.0;
                        double cy = sinphi * cxp + cosphi * cyp + (y1 + y2) / 2.0;
                        // unit vectors
                        double ux = (x1p - cxp) / rx; double uy = (y1p - cyp) / ry;
                        double vx = (-x1p - cxp) / rx; double vy = (-y1p - cyp) / ry;
                        double n1 = Math.Sqrt(ux * ux + uy * uy);
                        double n2 = Math.Sqrt(vx * vx + vy * vy);
                        double theta1 = Math.Acos(Math.Max(-1, Math.Min(1, ux / n1))); if (uy < 0) theta1 = -theta1;
                        double dtheta = Math.Acos(Math.Max(-1, Math.Min(1, (ux * vx + uy * vy) / (n1 * n2))));
                        double cross = ux * vy - uy * vx; if (cross < 0) dtheta = -dtheta;
                        if (arcSeg.SweepDirection == Core2D.Model.Path.SweepDirection.Clockwise && dtheta > 0) dtheta -= 2 * Math.PI;
                        if (arcSeg.SweepDirection == Core2D.Model.Path.SweepDirection.Counterclockwise && dtheta < 0) dtheta += 2 * Math.PI;

                        if (Math.Abs(rx - ry) < 1e-6)
                        {
                            var edgeArcCirc = new Hatch.BoundaryPath.Arc
                            {
                                Center = new CSMath.XY(ToDwgX(cx), ToDwgY(cy)),
                                Radius = rx,
                                StartAngle = theta1,
                                EndAngle = theta1 + dtheta,
                                CounterClockWise = arcSeg.SweepDirection == Core2D.Model.Path.SweepDirection.Counterclockwise
                            };
                            bp.Edges.Add(edgeArcCirc);
                        }
                        else
                        {
                            var edgeArc = new Hatch.BoundaryPath.Ellipse
                            {
                                Center = new CSMath.XY(ToDwgX(cx), ToDwgY(cy)),
                                MajorAxisEndPoint = new CSMath.XY(rx * Math.Cos(phi), -rx * Math.Sin(phi)),
                                MinorToMajorRatio = rx != 0 ? (ry / rx) : 1.0,
                                StartAngle = theta1,
                                EndAngle = theta1 + dtheta,
                                IsCounterclockwise = arcSeg.SweepDirection == Core2D.Model.Path.SweepDirection.Counterclockwise,
                                CounterClockWise = arcSeg.SweepDirection == Core2D.Model.Path.SweepDirection.Counterclockwise
                            };
                            bp.Edges.Add(edgeArc);
                        }

                        if (Math.Abs(rx - ry) < 1e-6)
                        {
                            // circular arc
                            var arcEnt = new Arc
                            {
                                Center = new CSMath.XYZ(ToDwgX(cx), ToDwgY(cy), 0),
                                Radius = rx,
                                StartAngle = theta1 + 0.0,
                                EndAngle = theta1 + dtheta
                            };
                            strokeSegs.Add(arcEnt);
                        }
                        else
                        {
                            var elSeg = new Ellipse
                            {
                                Center = new CSMath.XYZ(ToDwgX(cx), ToDwgY(cy), 0),
                                MajorAxisEndPoint = new CSMath.XYZ(rx * Math.Cos(phi), -rx * Math.Sin(phi), 0),
                                RadiusRatio = rx != 0 ? (ry / rx) : 1.0,
                                StartParameter = theta1,
                                EndParameter = theta1 + dtheta
                            };
                            strokeSegs.Add(elSeg);
                        }
                        onlyLines = false;
                        current = arcSeg.Point;
                        break;
                    default:
                        throw new NotSupportedException("Not supported segment type: " + segment.GetType());
                }
            }
            if (pf.IsClosed)
            {
                var sx = pf.StartPoint.X;
                var sy = pf.StartPoint.Y;
                if (Math.Abs(current.X - sx) > 1e-6 || Math.Abs(current.Y - sy) > 1e-6)
                {
                    bp.Edges.Add(new Hatch.BoundaryPath.Line
                    {
                        Start = new CSMath.XY(ToDwgX(current.X), ToDwgY(current.Y)),
                        End = new CSMath.XY(ToDwgX(sx), ToDwgY(sy))
                    });
                    lwVertices.Add((sx, sy));
                }
            }
            if (onlyLines && lwVertices.Count >= 2)
            {
                var polyBp = new Hatch.BoundaryPath();
                var polyEdge = new Hatch.BoundaryPath.Polyline { IsClosed = pf.IsClosed };
                foreach (var v in lwVertices)
                {
                    polyEdge.Vertices.Add(new CSMath.XYZ(ToDwgX(v.x), ToDwgY(v.y), 0));
                }
                polyBp.Edges.Add(polyEdge);
                allBounds.Add(polyBp);
            }
            else
            {
                if (bp.Edges.Count > 0)
                {
                    allBounds.Add(bp);
                }
            }
            if (onlyLines && pf.IsClosed && lwVertices.Count >= 3)
            {
                var lw = new LwPolyline { IsClosed = true };
                foreach (var v in lwVertices)
                {
                    lw.Vertices.Add(new LwPolyline.Vertex(new CSMath.XY(ToDwgX(v.x), ToDwgY(v.y))));
                }
                allEntities.Add(lw);
            }
            else
            {
                // fallback to explicit segments
                if (onlyLines)
                {
                    for (int i = 1; i < lwVertices.Count; i++)
                    {
                        var a = lwVertices[i - 1];
                        var b = lwVertices[i];
                        allEntities.Add(CreateLine(a.x, a.y, b.x, b.y));
                    }
                }
                allEntities.AddRange(strokeSegs);
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
            // Map FillRule to Hatch.Style (EvenOdd -> Normal; NonZero -> Ignore)
            if (path.FillRule == Core2D.Model.Path.FillRule.Nonzero)
            {
                hatch.Style = HatchStyleType.Ignore;
            }
            else
            {
                hatch.Style = HatchStyleType.Normal;
            }
            AddEntity(doc, hatch);
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
                ApplyLineType(doc, e, style);
                AddEntity(doc, e);
            }
        }
    }
}
