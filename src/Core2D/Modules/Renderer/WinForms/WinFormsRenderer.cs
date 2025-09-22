// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Model.Style;
using Core2D.ViewModels;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Spatial;
using Core2D.Spatial.Arc;

namespace Core2D.Modules.Renderer.WinForms;

public partial class WinFormsRenderer : ViewModelBase, IShapeRenderer
{
    private readonly ICache<string, Image>? _biCache;
    private readonly Func<double, float> _scaleToPage;
    private readonly double _textScaleFactor;

    [AutoNotify] private ShapeRendererStateViewModel? _state;

    public WinFormsRenderer(IServiceProvider? serviceProvider, double textScaleFactor = 1.0) : base(serviceProvider)
    {
        _state = serviceProvider.GetService<IViewModelFactory>()?.CreateShapeRendererState();
        _biCache = serviceProvider.GetService<IViewModelFactory>()?.CreateCache<string, Image>(bi => bi.Dispose());
        _textScaleFactor = textScaleFactor;
        _scaleToPage = value => (float)value;
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    private static Color ToColor(BaseColorViewModel colorViewModel)
    {
        return colorViewModel switch
        {
            ArgbColorViewModel argbColor => Color.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B),
            _ => new Color(),
        };
    }

    private Brush? ToBrush(BaseColorViewModel colorViewModel)
    {
        return colorViewModel switch
        {
            ArgbColorViewModel argbColor => new SolidBrush(ToColor(argbColor)),
            _ => null,
        };
    }

    private Pen? ToPen(ShapeStyleViewModel style)
    {
        if (style.Stroke?.Color is null)
        {
            return null;
        }

        var zoomX = State?.ZoomX ?? 1d;
        var pen = new Pen(ToColor(style.Stroke.Color), (float)(style.Stroke.Thickness / zoomX));
        switch (style.Stroke.LineCap)
        {
            case Model.Style.LineCap.Flat:
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                pen.DashCap = DashCap.Flat;
                break;

            case Model.Style.LineCap.Square:
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Square;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                pen.DashCap = DashCap.Flat;
                break;

            case Model.Style.LineCap.Round:
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.DashCap = DashCap.Round;
                break;
        }
        if (style.Stroke.Dashes is { })
        {
            // TODO: Convert to correct dash values.
            var dashPattern = StyleHelper.ConvertDashesToFloatArray(style.Stroke.Dashes, 1);
            var dashOffset = (float)style.Stroke.DashOffset;
            if (dashPattern is { })
            {
                pen.DashPattern = dashPattern;
                pen.DashStyle = DashStyle.Custom;
                pen.DashOffset = dashOffset;
            }
        }
        return pen;
    }

    private Pen ToPen(BaseColorViewModel colorViewModel, double thickness)
    {
        var zoomX = State?.ZoomX ?? 1d;
        var pen = new Pen(ToColor(colorViewModel), (float)(thickness / zoomX));

        pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
        pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
        pen.DashCap = DashCap.Flat;

        return pen;
    }

    private static Rect2 CreateRect(PointShapeViewModel tl, PointShapeViewModel br, double dx, double dy) => Rect2.FromPoints(tl.X, tl.Y, br.X, br.Y, dx, dy);

    private static void DrawLineInternal(Graphics gfx, Pen pen, bool isStroked, ref PointF p0, ref PointF p1)
    {
        if (isStroked)
        {
            gfx.DrawLine(pen, p0, p1);
        }
    }

    private void DrawLineArrowsInternal(LineShapeViewModel line, ShapeStyleViewModel style, Graphics gfx, out PointF pt1, out PointF pt2)
    {
        if (line.Start is null || line.End is null || style.Fill?.Color is null || style.Stroke?.StartArrow is null || style.Stroke?.EndArrow is null)
        {
            pt1 = PointF.Empty;
            pt2 = PointF.Empty;
            return;
        }
        
        var fillStartArrow = ToBrush(style.Fill.Color);
        var strokeStartArrow = ToPen(style);
        if (fillStartArrow is null || strokeStartArrow is null)
        {
            pt1 = PointF.Empty;
            pt2 = PointF.Empty;
            return;  
        }

        var fillEndArrow = ToBrush(style.Fill.Color);
        var strokeEndArrow = ToPen(style);
        if (fillEndArrow is null || strokeEndArrow is null)
        {
            pt1 = PointF.Empty;
            pt2 = PointF.Empty;
            return;  
        }

        var sx1 = line.Start.X;
        var sy1 = line.Start.Y;
        var ex2 = line.End.X;
        var ey2 = line.End.Y;

        var x1 = _scaleToPage(sx1);
        var y1 = _scaleToPage(sy1);
        var x2 = _scaleToPage(ex2);
        var y2 = _scaleToPage(ey2);

        var sas = style.Stroke.StartArrow;
        var eas = style.Stroke.EndArrow;
        var a1 = (float)(Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI);
        var a2 = (float)(Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI);

        // Draw start arrow.
        pt1 = DrawLineArrowInternal(gfx, strokeStartArrow, fillStartArrow, x1, y1, a1, sas, line);

        // Draw end arrow.
        pt2 = DrawLineArrowInternal(gfx, strokeEndArrow, fillEndArrow, x2, y2, a2, eas, line);

        fillStartArrow.Dispose();
        strokeStartArrow.Dispose();

        fillEndArrow.Dispose();
        strokeEndArrow.Dispose();
    }

    private static PointF DrawLineArrowInternal(Graphics gfx, Pen pen, Brush brush, float x, float y, float angle, ArrowStyleViewModel style, BaseShapeViewModel shape)
    {
        PointF pt;
        var rt = new Matrix();
        rt.RotateAt(angle, new PointF(x, y));
        var rx = style.RadiusX;
        var ry = style.RadiusY;
        var sx = 2.0 * rx;
        var sy = 2.0 * ry;

        switch (style.ArrowType)
        {
            default:
            case ArrowType.None:
            {
                pt = new PointF(x, y);
                break;
            }
            case ArrowType.Rectangle:
            {
                var pts = new[] { new PointF(x - (float)sx, y) };
                rt.TransformPoints(pts);
                pt = pts[0];
                var rect = new Rect2(x - sx, y - ry, sx, sy);
                var gs = gfx.Save();
                gfx.MultiplyTransform(rt);
                DrawRectangleInternal(gfx, brush, pen, shape.IsStroked, shape.IsFilled, ref rect);
                gfx.Restore(gs);
                break;
            }
            case ArrowType.Ellipse:
            {
                var pts = new[] { new PointF(x - (float)sx, y) };
                rt.TransformPoints(pts);
                pt = pts[0];
                var gs = gfx.Save();
                gfx.MultiplyTransform(rt);
                var rect = new Rect2(x - sx, y - ry, sx, sy);
                DrawEllipseInternal(gfx, brush, pen, shape.IsStroked, shape.IsFilled, ref rect);
                gfx.Restore(gs);
                break;
            }
            case ArrowType.Arrow:
            {
                var pts = new[]
                {
                    new PointF(x, y),
                    new PointF(x - (float)sx, y + (float)sy),
                    new PointF(x, y),
                    new PointF(x - (float)sx, y - (float)sy),
                    new PointF(x, y)
                };
                rt.TransformPoints(pts);
                pt = pts[0];
                var p11 = pts[1];
                var p21 = pts[2];
                var p12 = pts[3];
                var p22 = pts[4];
                DrawLineInternal(gfx, pen, shape.IsStroked, ref p11, ref p21);
                DrawLineInternal(gfx, pen, shape.IsStroked, ref p12, ref p22);
                break;
            }
        }

        return pt;
    }

    private static void DrawRectangleInternal(Graphics gfx, Brush brush, Pen pen, bool isStroked, bool isFilled, ref Rect2 rect)
    {
        if (isFilled)
        {
            gfx.FillRectangle(
                brush,
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
        }

        if (isStroked)
        {
            gfx.DrawRectangle(
                pen,
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
        }
    }

    private static void DrawEllipseInternal(Graphics gfx, Brush brush, Pen pen, bool isStroked, bool isFilled, ref Rect2 rect)
    {
        if (isFilled)
        {
            gfx.FillEllipse(
                brush,
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
        }

        if (isStroked)
        {
            gfx.DrawEllipse(
                pen,
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
        }
    }

    private void DrawGridInternal(Graphics gfx, IGrid grid, Pen stroke, ref Rect2 rect)
    {
        double ox = rect.X;
        double ex = rect.X + rect.Width;
        double oy = rect.Y;
        double ey = rect.Y + rect.Height;
        double cw = grid.GridCellWidth;
        double ch = grid.GridCellHeight;

        for (double x = ox + ch; x < ex; x += cw)
        {
            var p0 = new PointF(
                _scaleToPage(x),
                _scaleToPage(oy));
            var p1 = new PointF(
                _scaleToPage(x),
                _scaleToPage(ey));
            DrawLineInternal(gfx, stroke, true, ref p0, ref p1);
        }

        for (double y = oy + ch; y < ey; y += ch)
        {
            var p0 = new PointF(
                _scaleToPage(ox),
                _scaleToPage(y));
            var p1 = new PointF(
                _scaleToPage(ex),
                _scaleToPage(y));
            DrawLineInternal(gfx, stroke, true, ref p0, ref p1);
        }
    }

    public void ClearCache()
    {
        _biCache?.Reset();
    }

    public void Fill(object? dc, double x, double y, double width, double height, BaseColorViewModel? colorViewModel)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (colorViewModel is null)
        {
            return;
        }
        
        var brush = ToBrush(colorViewModel);
        if (brush is { })
        {
            gfx.FillRectangle(
                brush,
                (float)x,
                (float)y,
                (float)width,
                (float)height);
            brush.Dispose();
        }
    }

    public void Grid(object? dc, IGrid grid, double x, double y, double width, double height)
    {
        if (dc is not Graphics gfx)
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

        var pen = ToPen(grid.GridStrokeColor, grid.GridStrokeThickness);

        var rect = Rect2.FromPoints(
            x + grid.GridOffsetLeft,
            y + grid.GridOffsetTop,
            x + width - grid.GridOffsetLeft + grid.GridOffsetRight,
            y + height - grid.GridOffsetTop + grid.GridOffsetBottom);

        if (grid.IsGridEnabled)
        {
            DrawGridInternal(
                gfx,
                grid,
                pen,
                ref rect);
        }

        if (grid.IsBorderEnabled)
        {
            gfx.DrawRectangle(
                pen,
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));
        }

        pen.Dispose();
    }

    public void DrawPoint(object? dc, PointShapeViewModel point, ShapeStyleViewModel? style)
    {
        // TODO:
    }

    public void DrawLine(object? dc, LineShapeViewModel line, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (style is null)
        {
            return;
        }

        DrawLineArrowsInternal(line, style, gfx, out var pt1, out var pt2);

        var pen = ToPen(style);
        if (pen is { })
        {
            DrawLineInternal(gfx, pen, line.IsStroked, ref pt1, ref pt2);
            pen.Dispose();
        }
    }

    public void DrawWire(object? dc, WireShapeViewModel wire, ShapeStyleViewModel? style)
    {
        switch (wire.RendererKey)
        {
            default:
                DrawLine(dc, wire, style);
                break;
            case WireRendererKeys.Line:
                DrawLine(dc, wire, style);
                break;
            case WireRendererKeys.Bezier:
                DrawBezierWire(dc, wire, style);
                break;
        }
    }

    private void DrawBezierWire(object? dc, WireShapeViewModel wire, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (style is null)
        {
            return;
        }

        if (!WireGeometryHelper.TryCreateGeometry(wire, out var geometry))
        {
            return;
        }

        if (!geometry.IsBezier)
        {
            DrawLine(dc, wire, style);
            return;
        }

        var p1 = new PointF(_scaleToPage(geometry.Start.X), _scaleToPage(geometry.Start.Y));
        var c1 = new PointF(_scaleToPage(geometry.Control1.X), _scaleToPage(geometry.Control1.Y));
        var c2 = new PointF(_scaleToPage(geometry.Control2.X), _scaleToPage(geometry.Control2.Y));
        var p2 = new PointF(_scaleToPage(geometry.End.X), _scaleToPage(geometry.End.Y));

        using var path = new GraphicsPath();
        path.AddBezier(p1, c1, c2, p2);

        var pen = ToPen(style);
        if (pen is { })
        {
            if (wire.IsStroked)
            {
                gfx.DrawPath(pen, path);
            }
            pen.Dispose();
        }

        DrawBezierWireArrows(gfx, wire, style, geometry);
    }

    private void DrawBezierWireArrows(Graphics gfx, WireShapeViewModel wire, ShapeStyleViewModel style, WireGeometry geometry)
    {
        var startArrow = style.Stroke?.StartArrow;
        var endArrow = style.Stroke?.EndArrow;

        if (startArrow is { ArrowType: not ArrowType.None } && style.Fill?.Color is { })
        {
            var fill = ToBrush(style.Fill.Color);
            var pen = ToPen(style);
            if (fill is { } && pen is { })
            {
                var start = new PointF(_scaleToPage(geometry.Start.X), _scaleToPage(geometry.Start.Y));
                var ctrl = new PointF(_scaleToPage(geometry.Control1.X), _scaleToPage(geometry.Control1.Y));
                var angle = (float)(Math.Atan2(start.Y - ctrl.Y, start.X - ctrl.X) * 180.0 / Math.PI);
                DrawLineArrowInternal(gfx, pen, fill, start.X, start.Y, angle, startArrow, wire);
            }
            fill?.Dispose();
            pen?.Dispose();
        }

        if (endArrow is { ArrowType: not ArrowType.None } && style.Fill?.Color is { })
        {
            var fill = ToBrush(style.Fill.Color);
            var pen = ToPen(style);
            if (fill is { } && pen is { })
            {
                var end = new PointF(_scaleToPage(geometry.End.X), _scaleToPage(geometry.End.Y));
                var ctrl = new PointF(_scaleToPage(geometry.Control2.X), _scaleToPage(geometry.Control2.Y));
                var angle = (float)(Math.Atan2(end.Y - ctrl.Y, end.X - ctrl.X) * 180.0 / Math.PI);
                DrawLineArrowInternal(gfx, pen, fill, end.X, end.Y, angle, endArrow, wire);
            }
            fill?.Dispose();
            pen?.Dispose();
        }
    }

    public void DrawRectangle(object? dc, RectangleShapeViewModel rectangle, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (rectangle.TopLeft is null || rectangle.BottomRight is null)
        {
            return;
        }

        var rect = CreateRect(
            rectangle.TopLeft,
            rectangle.BottomRight,
            0, 0);

        if (rectangle.IsFilled && style?.Fill?.Color is { })
        {
            var brush = ToBrush(style.Fill.Color);
            if (brush is { })
            {
                gfx.FillRectangle(
                    brush,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
                brush.Dispose();
            }
        }

        if (rectangle.IsStroked && style is { })
        {
            var pen = ToPen(style);
            if (pen is { })
            {
                gfx.DrawRectangle(
                    pen,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
                pen.Dispose();
            }
        }
    }

    public void DrawEllipse(object? dc, EllipseShapeViewModel ellipse, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (ellipse.TopLeft is null || ellipse.BottomRight is null)
        {
            return;
        }
        
        var rect = CreateRect(
            ellipse.TopLeft,
            ellipse.BottomRight,
            0, 0);

        if (ellipse.IsFilled && style?.Fill?.Color is { })
        {
            var brush = ToBrush(style.Fill.Color);
            if (brush is { })
            {
                gfx.FillEllipse(
                    brush,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
                brush.Dispose();
            }
        }

        if (ellipse.IsStroked && style is { })
        {
            var pen = ToPen(style);
            if (pen is { })
            {
                gfx.DrawEllipse(
                    pen,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
                pen.Dispose();
            }
        }
    }

    public void DrawArc(object? dc, ArcShapeViewModel arc, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (arc.Point1 is null || arc.Point2 is null || arc.Point3 is null || arc.Point4 is null)
        {
            return;
        }
        
        var a = new GdiArc(
            Point2.FromXY(arc.Point1.X, arc.Point1.Y),
            Point2.FromXY(arc.Point2.X, arc.Point2.Y),
            Point2.FromXY(arc.Point3.X, arc.Point3.Y),
            Point2.FromXY(arc.Point4.X, arc.Point4.Y));

        if (!(a.Width > 0.0) || !(a.Height > 0.0))
        {
            return;
        }

        if (arc.IsFilled && style?.Fill?.Color is { })
        {
            var brush = ToBrush(style.Fill.Color);
            if (brush is { })
            {
                var path = new GraphicsPath();
                path.AddArc(
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    (float)a.StartAngle,
                    (float)a.SweepAngle);
                gfx.FillPath(brush, path);
                brush.Dispose();
            }
        }

        if (arc.IsStroked && style is { })
        {
            var pen = ToPen(style);
            if (pen is { })
            {
                gfx.DrawArc(
                    pen,
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    (float)a.StartAngle,
                    (float)a.SweepAngle);
                pen.Dispose();
            }
        }
    }

    public void DrawCubicBezier(object? dc, CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (cubicBezier.Point1 is null || cubicBezier.Point2 is null || cubicBezier.Point3 is null || cubicBezier.Point4 is null)
        {
            return;
        }

        if (cubicBezier.IsFilled && style?.Fill?.Color is { })
        {
            var brush = ToBrush(style.Fill.Color);
            if (brush is { })
            {
                var path = new GraphicsPath();
                path.AddBezier(
                    _scaleToPage(cubicBezier.Point1.X),
                    _scaleToPage(cubicBezier.Point1.Y),
                    _scaleToPage(cubicBezier.Point2.X),
                    _scaleToPage(cubicBezier.Point2.Y),
                    _scaleToPage(cubicBezier.Point3.X),
                    _scaleToPage(cubicBezier.Point3.Y),
                    _scaleToPage(cubicBezier.Point4.X),
                    _scaleToPage(cubicBezier.Point4.Y));
                gfx.FillPath(brush, path);
                brush.Dispose();
            }
        }

        if (cubicBezier.IsStroked && style is { })
        {
            var pen = ToPen(style);
            if (pen is { })
            {
                gfx.DrawBezier(
                    pen,
                    _scaleToPage(cubicBezier.Point1.X),
                    _scaleToPage(cubicBezier.Point1.Y),
                    _scaleToPage(cubicBezier.Point2.X),
                    _scaleToPage(cubicBezier.Point2.Y),
                    _scaleToPage(cubicBezier.Point3.X),
                    _scaleToPage(cubicBezier.Point3.Y),
                    _scaleToPage(cubicBezier.Point4.X),
                    _scaleToPage(cubicBezier.Point4.Y));
                pen.Dispose();
            }
        }
    }

    public void DrawQuadraticBezier(object? dc, QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (quadraticBezier.Point1 is null || quadraticBezier.Point2 is null || quadraticBezier.Point3 is null)
        {
            return;
        }

        var x1 = quadraticBezier.Point1.X;
        var y1 = quadraticBezier.Point1.Y;
        var x2 = quadraticBezier.Point1.X + 2.0 * (quadraticBezier.Point2.X - quadraticBezier.Point1.X) / 3.0;
        var y2 = quadraticBezier.Point1.Y + 2.0 * (quadraticBezier.Point2.Y - quadraticBezier.Point1.Y) / 3.0;
        var x3 = x2 + (quadraticBezier.Point3.X - quadraticBezier.Point1.X) / 3.0;
        var y3 = y2 + (quadraticBezier.Point3.Y - quadraticBezier.Point1.Y) / 3.0;
        var x4 = quadraticBezier.Point3.X;
        var y4 = quadraticBezier.Point3.Y;

        if (quadraticBezier.IsFilled && style?.Fill?.Color is { })
        {
            var brush = ToBrush(style.Fill.Color);
            if (brush is { })
            {
                var path = new GraphicsPath();
                path.AddBezier(
                    _scaleToPage(x1),
                    _scaleToPage(y1),
                    _scaleToPage(x2),
                    _scaleToPage(y2),
                    _scaleToPage(x3),
                    _scaleToPage(y3),
                    _scaleToPage(x4),
                    _scaleToPage(y4));
                gfx.FillPath(brush, path);
                brush.Dispose();
            }
        }

        if (quadraticBezier.IsStroked && style is { })
        {
            var pen = ToPen(style);
            if (pen is { })
            {
                gfx.DrawBezier(
                    pen,
                    _scaleToPage(x1),
                    _scaleToPage(y1),
                    _scaleToPage(x2),
                    _scaleToPage(y2),
                    _scaleToPage(x3),
                    _scaleToPage(y3),
                    _scaleToPage(x4),
                    _scaleToPage(y4));
                pen.Dispose();
            }
        }
    }

    public void DrawText(object? dc, TextShapeViewModel text, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (text.TopLeft is null || text.BottomRight is null)
        {
            return;
        }
        
        if (style?.Stroke?.Color is null)
        {
            return;
        }

        if (style.TextStyle is null || style.TextStyle.FontName is null)
        {
            return;
        }

        if (!(text.GetProperty(nameof(TextShapeViewModel.Text)) is string boundText))
        {
            boundText = text.Text ?? string.Empty;
        }

        var brush = ToBrush(style.Stroke.Color);
        if (brush is null)
        {
            return;
        }

        var fontStyle = FontStyle.Regular;

        if (style.TextStyle.FontStyle.HasFlag(FontStyleFlags.Bold))
        {
            fontStyle |= FontStyle.Bold;
        }

        if (style.TextStyle.FontStyle.HasFlag(FontStyleFlags.Italic))
        {
            fontStyle |= FontStyle.Italic;
        }

        var font = new Font(
            style.TextStyle.FontName,
            (float)(style.TextStyle.FontSize * _textScaleFactor),
            fontStyle);

        var rect = CreateRect(
            text.TopLeft,
            text.BottomRight,
            0, 0);

        var scaledRect = new RectangleF(
            _scaleToPage(rect.X),
            _scaleToPage(rect.Y),
            _scaleToPage(rect.Width),
            _scaleToPage(rect.Height));

        var format = new StringFormat();

        format.Alignment = style.TextStyle.TextHAlignment switch
        {
            TextHAlignment.Left => StringAlignment.Near,
            TextHAlignment.Center => StringAlignment.Center,
            TextHAlignment.Right => StringAlignment.Far,
            _ => format.Alignment
        };

        format.LineAlignment = style.TextStyle.TextVAlignment switch
        {
            TextVAlignment.Top => StringAlignment.Near,
            TextVAlignment.Center => StringAlignment.Center,
            TextVAlignment.Bottom => StringAlignment.Far,
            _ => format.LineAlignment
        };

        format.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
        format.Trimming = StringTrimming.None;

        gfx.DrawString(boundText, font, brush, scaledRect, format);

        brush.Dispose();
        font.Dispose();
    }

    public void DrawImage(object? dc, ImageShapeViewModel image, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        if (image.TopLeft is null || image.BottomRight is null)
        {
            return;
        }

        var rect = CreateRect(
            image.TopLeft,
            image.BottomRight,
            0, 0);

        var srect = new RectangleF(
            _scaleToPage(rect.X),
            _scaleToPage(rect.Y),
            _scaleToPage(rect.Width),
            _scaleToPage(rect.Height));

        if (image.IsFilled && style?.Fill?.Color is { })
        {
            var brush = ToBrush(style.Fill.Color);
            if (brush is { })
            {
                gfx.FillRectangle(brush, srect);
                brush.Dispose();
            }
        }

        if (image.IsStroked && style is { })
        {
            var pen = ToPen(style);
            if (pen is { })
            {
                gfx.DrawRectangle(
                    pen,
                    srect.X,
                    srect.Y,
                    srect.Width,
                    srect.Height);
                pen.Dispose();
            }
        }

        if (image.Key is null)
        {
            return;
        }
        
        var imageCached = _biCache?.Get(image.Key);
        if (imageCached is { })
        {
            gfx.DrawImage(imageCached, srect);
        }
        else
        {
            if (State?.ImageCache is { } && !string.IsNullOrEmpty(image.Key))
            {
                var bytes = State.ImageCache.GetImage(image.Key);
                if (bytes is { })
                {
                    var ms = new System.IO.MemoryStream(bytes);
                    var bi = Image.FromStream(ms);
                    ms.Dispose();

                    _biCache?.Set(image.Key, bi);

                    gfx.DrawImage(bi, srect);
                }
            }
        }
    }

    public void DrawPath(object? dc, PathShapeViewModel path, ShapeStyleViewModel? style)
    {
        if (dc is not Graphics gfx)
        {
            return;
        }

        var graphicsPath = path.ToGraphicsPath(_scaleToPage);
        if (graphicsPath is null)
        {
            return;
        }

        if (path.IsFilled && path.IsStroked)
        {
            if (style?.Fill?.Color is null)
            {
                return;
            }
            var brush = ToBrush(style.Fill.Color);
            var pen = ToPen(style);
            if (brush is null || pen is null)
            {
                return;
            }
            gfx.FillPath(brush, graphicsPath);
            gfx.DrawPath(pen, graphicsPath);
            brush.Dispose();
            pen.Dispose();
        }
        else if (path.IsFilled && !path.IsStroked)
        {
            if (style?.Fill?.Color is null)
            {
                return;
            }
            var brush = ToBrush(style.Fill.Color);
            if (brush is null )
            {
                return;
            }
            gfx.FillPath(brush, graphicsPath);
            brush.Dispose();
        }
        else if (!path.IsFilled && path.IsStroked)
        {
            if (style is null)
            {
                return;
            }
            var pen = ToPen(style);
            if (pen is null)
            {
                return;
            }
            gfx.DrawPath(pen, graphicsPath);
            pen.Dispose();
        }
    }
}
