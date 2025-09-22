// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Model.Style;
using Core2D.ViewModels;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Core2D.Modules.Renderer.PdfSharp;

public partial class PdfSharpRenderer : ViewModelBase, IShapeRenderer
{
    private readonly ICache<string, XImage>? _biCache;
    private Func<double, double> _scaleToPage;
    private readonly double _sourceDpi = 96.0;
    private readonly double _targetDpi = 72.0;

    [AutoNotify] private ShapeRendererStateViewModel? _state;

    public PdfSharpRenderer(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _state = serviceProvider.GetService<IViewModelFactory>()?.CreateShapeRendererState();
        _biCache = serviceProvider.GetService<IViewModelFactory>()?.CreateCache<string, XImage>(bi => bi.Dispose());
        _scaleToPage = value => (float)(value * 1.0);
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    private static XColor ToXColor(BaseColorViewModel colorViewModel)
    {
        return colorViewModel switch
        {
            ArgbColorViewModel argbColor => XColor.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B),
            _ => new XColor(),
        };
    }

    private static XPen? ToXPen(ShapeStyleViewModel style, Func<double, double> scale, double sourceDpi, double targetDpi)
    {
        if (style.Stroke?.Color is null)
        {
            return null;
        }

        var strokeWidth = scale(style.Stroke.Thickness * targetDpi / sourceDpi);
        var pen = new XPen(ToXColor(style.Stroke.Color), strokeWidth);
        switch (style.Stroke.LineCap)
        {
            case LineCap.Flat:
                pen.LineCap = XLineCap.Flat;
                break;

            case LineCap.Square:
                pen.LineCap = XLineCap.Square;
                break;

            case LineCap.Round:
                pen.LineCap = XLineCap.Round;
                break;
        }
        if (style.Stroke.Dashes is { })
        {
            // TODO: Convert to correct dash values.
            var dashPattern = StyleHelper.ConvertDashesToDoubleArray(style.Stroke.Dashes, strokeWidth);
            var dashOffset = style.Stroke.DashOffset * strokeWidth;
            if (dashPattern is { })
            {
                pen.DashPattern = dashPattern;
                pen.DashStyle = XDashStyle.Custom;
                pen.DashOffset = dashOffset;
            }
        }
        return pen;
    }

    private static XPen ToXPen(BaseColorViewModel colorViewModel, double thickness, Func<double, double> scale, double sourceDpi, double targetDpi)
    {
        var strokeWidth = scale(thickness * targetDpi / sourceDpi);
        return new XPen(ToXColor(colorViewModel), strokeWidth)
        {
            LineCap = XLineCap.Flat
        };
    }

    private static XBrush? ToXBrush(BaseColorViewModel colorViewModel)
    {
        return colorViewModel switch
        {
            ArgbColorViewModel argbColor => new XSolidBrush(ToXColor(argbColor)),
            _ => null,
        };
    }

    private static void DrawLineInternal(XGraphics gfx, XPen pen, bool isStroked, ref XPoint p0, ref XPoint p1)
    {
        if (isStroked)
        {
            gfx.DrawLine(pen, p0, p1);
        }
    }

    private void DrawLineArrowsInternal(XGraphics gfx, LineShapeViewModel line, ShapeStyleViewModel style, out XPoint pt1, out XPoint pt2)
    {
        if (line.Start is null || line.End is null || style.Fill?.Color is null || style.Stroke?.StartArrow is null || style.Stroke?.EndArrow is null)
        {
            pt1 = new XPoint();
            pt2 = new XPoint();
            return;
        }
        
        var fillStartArrow = ToXBrush(style.Fill.Color);
        var strokeStartArrow = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
        if (fillStartArrow is null || strokeStartArrow is null)
        {
            pt1 = new XPoint();
            pt2 = new XPoint();
            return;  
        }

        var fillEndArrow = ToXBrush(style.Fill.Color);
        var strokeEndArrow = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
        if (fillEndArrow is null || strokeEndArrow is null)
        {
            pt1 = new XPoint();
            pt2 = new XPoint();
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
        var a1 = Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI;
        var a2 = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;

        // Draw start arrow.
        pt1 = DrawLineArrowInternal(gfx, strokeStartArrow, fillStartArrow, x1, y1, a1, sas, line);

        // Draw end arrow.
        pt2 = DrawLineArrowInternal(gfx, strokeEndArrow, fillEndArrow, x2, y2, a2, eas, line);
    }

    private static XPoint DrawLineArrowInternal(XGraphics gfx, XPen pen, XBrush brush, double x, double y, double angle, ArrowStyleViewModel style, BaseShapeViewModel shape)
    {
        XPoint pt;
        var rt = new XMatrix();
        var c = new XPoint(x, y);
        rt.RotateAtPrepend(angle, c);
        var rx = style.RadiusX;
        var ry = style.RadiusY;
        var sx = 2.0 * rx;
        var sy = 2.0 * ry;

        switch (style.ArrowType)
        {
            default:
            case ArrowType.None:
            {
                pt = new XPoint(x, y);
                break;
            }
            case ArrowType.Rectangle:
            {
                pt = rt.Transform(new XPoint(x - sx, y));
                var rect = new XRect(x - sx, y - ry, sx, sy);
                gfx.Save();
                gfx.RotateAtTransform(angle, c);
                DrawRectangleInternal(gfx, brush, pen, shape.IsStroked, shape.IsFilled, ref rect);
                gfx.Restore();
                break;
            }
            case ArrowType.Ellipse:
            {
                pt = rt.Transform(new XPoint(x - sx, y));
                gfx.Save();
                gfx.RotateAtTransform(angle, c);
                var rect = new XRect(x - sx, y - ry, sx, sy);
                DrawEllipseInternal(gfx, brush, pen, shape.IsStroked, shape.IsFilled, ref rect);
                gfx.Restore();
                break;
            }
            case ArrowType.Arrow:
            {
                pt = rt.Transform(new XPoint(x, y));
                var p11 = rt.Transform(new XPoint(x - sx, y + sy));
                var p21 = rt.Transform(new XPoint(x, y));
                var p12 = rt.Transform(new XPoint(x - sx, y - sy));
                var p22 = rt.Transform(new XPoint(x, y));
                DrawLineInternal(gfx, pen, shape.IsStroked, ref p11, ref p21);
                DrawLineInternal(gfx, pen, shape.IsStroked, ref p12, ref p22);
                break;
            }
        }

        return pt;
    }

    private static void DrawRectangleInternal(XGraphics gfx, XBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
    {
        if (isStroked && isFilled)
        {
            gfx.DrawRectangle(pen, brush, rect);
        }
        else if (isStroked && !isFilled)
        {
            gfx.DrawRectangle(pen, rect);
        }
        else if (!isStroked && isFilled)
        {
            gfx.DrawRectangle(brush, rect);
        }
    }

    private static void DrawEllipseInternal(XGraphics gfx, XBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
    {
        if (isStroked && isFilled)
        {
            gfx.DrawEllipse(pen, brush, rect);
        }
        else if (isStroked && !isFilled)
        {
            gfx.DrawEllipse(pen, rect);
        }
        else if (!isStroked && isFilled)
        {
            gfx.DrawEllipse(brush, rect);
        }
    }

    private void DrawGridInternal(XGraphics gfx, IGrid grid, XPen stroke, ref Spatial.Rect2 rect)
    {
        var ox = rect.X;
        var ex = rect.X + rect.Width;
        var oy = rect.Y;
        var ey = rect.Y + rect.Height;
        var cw = grid.GridCellWidth;
        var ch = grid.GridCellHeight;

        for (var x = ox + cw; x < ex; x += cw)
        {
            var p0 = new XPoint(
                _scaleToPage(x),
                _scaleToPage(oy));
            var p1 = new XPoint(
                _scaleToPage(x),
                _scaleToPage(ey));
            DrawLineInternal(gfx, stroke, true, ref p0, ref p1);
        }

        for (var y = oy + ch; y < ey; y += ch)
        {
            var p0 = new XPoint(
                _scaleToPage(ox),
                _scaleToPage(y));
            var p1 = new XPoint(
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
        if (dc is not XGraphics gfx)
        {
            return;
        }

        if (colorViewModel is null)
        {
            return;
        }

        var brush = ToXBrush(colorViewModel);
        if (brush is { })
        {
            gfx.DrawRectangle(
                brush,
                _scaleToPage(x),
                _scaleToPage(y),
                _scaleToPage(width),
                _scaleToPage(height));
        }
    }

    public void Grid(object? dc, IGrid grid, double x, double y, double width, double height)
    {
        if (dc is not XGraphics gfx)
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

        var pen = ToXPen(grid.GridStrokeColor, grid.GridStrokeThickness, _scaleToPage, _sourceDpi, _targetDpi);

        var rect = Spatial.Rect2.FromPoints(
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
    }

    public void DrawPoint(object? dc, PointShapeViewModel point, ShapeStyleViewModel? style)
    {
        // TODO:
    }

    public void DrawLine(object? dc, LineShapeViewModel line, ShapeStyleViewModel? style)
    {
        if (dc is not XGraphics gfx)
        {
            return;
        }

        if (style is null)
        {
            return;
        }

        DrawLineArrowsInternal(gfx, line, style, out var pt1, out var pt2);

        var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
        if (pen is { })
        {
            DrawLineInternal(gfx, pen, line.IsStroked, ref pt1, ref pt2);
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
        }
    }

    public void DrawRectangle(object? dc, RectangleShapeViewModel rectangle, ShapeStyleViewModel? style)
    {
        if (dc is not XGraphics gfx)
        {
            return;
        }

        if (rectangle.TopLeft is null || rectangle.BottomRight is null)
        {
            return;
        }

        var rect = Spatial.Rect2.FromPoints(
            rectangle.TopLeft.X,
            rectangle.TopLeft.Y,
            rectangle.BottomRight.X,
            rectangle.BottomRight.Y);

        if (rectangle.IsStroked && rectangle.IsFilled)
        {
            if (style?.Fill?.Color is { })
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
                var brush = ToXBrush(style.Fill.Color);
                if (pen is { } && brush is { })
                {
                    gfx.DrawRectangle(
                        pen,
                        brush,
                        _scaleToPage(rect.X),
                        _scaleToPage(rect.Y),
                        _scaleToPage(rect.Width),
                        _scaleToPage(rect.Height));
                }
            }
        }
        else if (rectangle.IsStroked && !rectangle.IsFilled)
        {
            if (style is { })
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
                if (pen is { })
                {
                    gfx.DrawRectangle(
                        pen,
                        _scaleToPage(rect.X),
                        _scaleToPage(rect.Y),
                        _scaleToPage(rect.Width),
                        _scaleToPage(rect.Height));
                }
            }
        }
        else if (!rectangle.IsStroked && rectangle.IsFilled)
        {
            if (style?.Fill?.Color is { })
            {
                var brush = ToXBrush(style.Fill.Color);
                if (brush is { })
                {
                    gfx.DrawRectangle(
                        brush,
                        _scaleToPage(rect.X),
                        _scaleToPage(rect.Y),
                        _scaleToPage(rect.Width),
                        _scaleToPage(rect.Height));
                }
            }
        }
    }

    public void DrawEllipse(object? dc, EllipseShapeViewModel ellipse, ShapeStyleViewModel? style)
    {
        if (dc is not XGraphics gfx)
        {
            return;
        }

        if (ellipse.TopLeft is null || ellipse.BottomRight is null)
        {
            return;
        }

        var rect = Spatial.Rect2.FromPoints(
            ellipse.TopLeft.X,
            ellipse.TopLeft.Y,
            ellipse.BottomRight.X,
            ellipse.BottomRight.Y);

        if (ellipse.IsStroked && ellipse.IsFilled)
        {
            if (style?.Fill?.Color is { })
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
                var brush = ToXBrush(style.Fill.Color);
                if (pen is { } && brush is { })
                {
                    gfx.DrawEllipse(
                        pen,
                        brush,
                        _scaleToPage(rect.X),
                        _scaleToPage(rect.Y),
                        _scaleToPage(rect.Width),
                        _scaleToPage(rect.Height));
                }
            }
        }
        else if (ellipse.IsStroked && !ellipse.IsFilled)
        {
            if (style is { })
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
                if (pen is { })
                {
                    gfx.DrawEllipse(
                        pen,
                        _scaleToPage(rect.X),
                        _scaleToPage(rect.Y),
                        _scaleToPage(rect.Width),
                        _scaleToPage(rect.Height));
                }
            }
        }
        else if (!ellipse.IsStroked && ellipse.IsFilled)
        {
            if (style?.Fill?.Color is { })
            {
                var brush = ToXBrush(style.Fill.Color);
                if (brush is { })
                {
                    gfx.DrawEllipse(
                        brush,
                        _scaleToPage(rect.X),
                        _scaleToPage(rect.Y),
                        _scaleToPage(rect.Width),
                        _scaleToPage(rect.Height));
                }
            }
        }
    }

    public void DrawArc(object? dc, ArcShapeViewModel arc, ShapeStyleViewModel? style)
    {
        if (dc is not XGraphics gfx)
        {
            return;
        }

        if (arc.Point1 is null || arc.Point2 is null || arc.Point3 is null || arc.Point4 is null)
        {
            return;
        }

        var a = new Spatial.Arc.GdiArc(
            Spatial.Point2.FromXY(arc.Point1.X, arc.Point1.Y),
            Spatial.Point2.FromXY(arc.Point2.X, arc.Point2.Y),
            Spatial.Point2.FromXY(arc.Point3.X, arc.Point3.Y),
            Spatial.Point2.FromXY(arc.Point4.X, arc.Point4.Y));

        if (!(a.Width > 0.0) || !(a.Height > 0.0))
        {
            return;
        }

        if (arc.IsFilled && style?.Fill?.Color is { })
        {
            var path = new XGraphicsPath();
            path.AddArc(
                _scaleToPage(a.X),
                _scaleToPage(a.Y),
                _scaleToPage(a.Width),
                _scaleToPage(a.Height),
                a.StartAngle,
                a.SweepAngle);

            if (arc.IsStroked)
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
                var brush = ToXBrush(style.Fill.Color);
                if (pen is { } && brush is { })
                {
                    gfx.DrawPath(pen, brush, path);
                }
            }
            else
            {
                var brush = ToXBrush(style.Fill.Color);
                if (brush is { })
                {
                    gfx.DrawPath(brush, path);
                }
            }
        }
        else
        {
            if (arc.IsStroked && style is { })
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
                if (pen is { })
                {
                    gfx.DrawArc(
                        pen,
                        _scaleToPage(a.X),
                        _scaleToPage(a.Y),
                        _scaleToPage(a.Width),
                        _scaleToPage(a.Height),
                        a.StartAngle,
                        a.SweepAngle);
                }
            }
        }
    }

    public void DrawCubicBezier(object? dc, CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel? style)
    {
        if (dc is not XGraphics gfx)
        {
            return;
        }

        if (cubicBezier.Point1 is null || cubicBezier.Point2 is null || cubicBezier.Point3 is null || cubicBezier.Point4 is null)
        {
            return;
        }

        if (cubicBezier.IsFilled && style?.Fill?.Color is { })
        {
            var path = new XGraphicsPath();
            path.AddBezier(
                _scaleToPage(cubicBezier.Point1.X),
                _scaleToPage(cubicBezier.Point1.Y),
                _scaleToPage(cubicBezier.Point2.X),
                _scaleToPage(cubicBezier.Point2.Y),
                _scaleToPage(cubicBezier.Point3.X),
                _scaleToPage(cubicBezier.Point3.Y),
                _scaleToPage(cubicBezier.Point4.X),
                _scaleToPage(cubicBezier.Point4.Y));

            if (cubicBezier.IsStroked)
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
                var brush = ToXBrush(style.Fill.Color);
                if (pen is { } && brush is { })
                {
                    gfx.DrawPath(pen, brush, path);
                }
            }
            else
            {
                var brush = ToXBrush(style.Fill.Color);
                if (brush is { })
                {
                    gfx.DrawPath(brush, path);
                }
            }
        }
        else
        {
            if (cubicBezier.IsStroked && style is { })
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
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
                }
            }
        }
    }

    public void DrawQuadraticBezier(object? dc, QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel? style)
    {
        if (dc is not XGraphics gfx)
        {
            return;
        }

        if (quadraticBezier.Point1 is null || quadraticBezier.Point2 is null || quadraticBezier.Point3 is null)
        {
            return;
        }

        var x1 = quadraticBezier.Point1.X;
        var y1 = quadraticBezier.Point1.Y;
        var x2 = quadraticBezier.Point1.X + (2.0 * (quadraticBezier.Point2.X - quadraticBezier.Point1.X)) / 3.0;
        var y2 = quadraticBezier.Point1.Y + (2.0 * (quadraticBezier.Point2.Y - quadraticBezier.Point1.Y)) / 3.0;
        var x3 = x2 + (quadraticBezier.Point3.X - quadraticBezier.Point1.X) / 3.0;
        var y3 = y2 + (quadraticBezier.Point3.Y - quadraticBezier.Point1.Y) / 3.0;
        var x4 = quadraticBezier.Point3.X;
        var y4 = quadraticBezier.Point3.Y;

        if (quadraticBezier.IsFilled && style?.Fill?.Color is { })
        {
            var path = new XGraphicsPath();
            path.AddBezier(
                _scaleToPage(x1),
                _scaleToPage(y1),
                _scaleToPage(x2),
                _scaleToPage(y2),
                _scaleToPage(x3),
                _scaleToPage(y3),
                _scaleToPage(x4),
                _scaleToPage(y4));

            if (quadraticBezier.IsStroked)
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
                var brush = ToXBrush(style.Fill.Color);
                if (pen is { } && brush is { })
                {
                    gfx.DrawPath(pen, brush, path);
                }
            }
            else
            {
                var brush = ToXBrush(style.Fill.Color);
                if (brush is { })
                {
                    gfx.DrawPath(brush, path);
                }
            }
        }
        else
        {
            if (quadraticBezier.IsStroked && style is { })
            {
                var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
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
                }
            }
        }
    }

    public void DrawText(object? dc, TextShapeViewModel text, ShapeStyleViewModel? style)
    {
        if (dc is not XGraphics gfx)
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

        var brush = ToXBrush(style.Stroke.Color);
        if (brush is null)
        {
            return;
        }
        
        var options = new XPdfFontOptions(PdfFontEncoding.Unicode);

        var fontStyle = XFontStyleEx.Regular;

        if (style.TextStyle.FontStyle.HasFlag(FontStyleFlags.Bold))
        {
            fontStyle |= XFontStyleEx.Bold;
        }

        if (style.TextStyle.FontStyle.HasFlag(FontStyleFlags.Italic))
        {
            fontStyle |= XFontStyleEx.Italic;
        }

        var font = new XFont(
            style.TextStyle.FontName,
            _scaleToPage(style.TextStyle.FontSize),
            fontStyle,
            options);

        var rect = Spatial.Rect2.FromPoints(
            text.TopLeft.X,
            text.TopLeft.Y,
            text.BottomRight.X,
            text.BottomRight.Y);

        var scaledRect = new XRect(
            _scaleToPage(rect.X),
            _scaleToPage(rect.Y),
            _scaleToPage(rect.Width),
            _scaleToPage(rect.Height));

        var format = new XStringFormat();

        format.Alignment = style.TextStyle.TextHAlignment switch
        {
            TextHAlignment.Left => XStringAlignment.Near,
            TextHAlignment.Center => XStringAlignment.Center,
            TextHAlignment.Right => XStringAlignment.Far,
            _ => format.Alignment
        };

        format.LineAlignment = style.TextStyle.TextVAlignment switch
        {
            TextVAlignment.Top => XLineAlignment.Near,
            TextVAlignment.Center => XLineAlignment.Center,
            TextVAlignment.Bottom => XLineAlignment.Far,
            _ => format.LineAlignment
        };

        gfx.DrawString(
            boundText,
            font,
            brush,
            scaledRect,
            format);
    }

    public void DrawImage(object? dc, ImageShapeViewModel image, ShapeStyleViewModel? style)
    {
        if (dc is not XGraphics gfx)
        {
            return;
        }

        if (image.TopLeft is null || image.BottomRight is null)
        {
            return;
        }

        var rect = Spatial.Rect2.FromPoints(
            image.TopLeft.X,
            image.TopLeft.Y,
            image.BottomRight.X,
            image.BottomRight.Y);

        var srect = new XRect(
            _scaleToPage(rect.X),
            _scaleToPage(rect.Y),
            _scaleToPage(rect.Width),
            _scaleToPage(rect.Height));

        if ((image.IsStroked || image.IsFilled) && style?.Fill?.Color is { })
        {
            var brush = ToXBrush(style.Fill.Color);
            var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
            if (brush is null || pen is null)
            {
                return;
            }
            DrawRectangleInternal(
                gfx,
                brush,
                pen,
                image.IsStroked,
                image.IsFilled,
                ref srect);
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

                    var bi = XImage.FromStream(ms);

                    _biCache?.Set(image.Key, bi);

                    gfx.DrawImage(bi, srect);
                }
            }
        }
    }

    public void DrawPath(object? dc, PathShapeViewModel path, ShapeStyleViewModel? style)
    {
        if (dc is not XGraphics gfx)
        {
            return;
        }

        var graphicsPath = path.ToXGraphicsPath(_scaleToPage);
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
            var brush = ToXBrush(style.Fill.Color);
            var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
            if (brush is null || pen is null)
            {
                return;
            }
            gfx.DrawPath(pen, brush, graphicsPath);
        }
        else if (path.IsFilled && !path.IsStroked)
        {
            if (style?.Fill?.Color is null)
            {
                return;
            }
            var brush = ToXBrush(style.Fill.Color);
            if (brush is null)
            {
                return;
            }
            gfx.DrawPath(brush, graphicsPath);
        }
        else if (!path.IsFilled && path.IsStroked)
        {
            if (style is null)
            {
                return;
            }
            var pen = ToXPen(style, _scaleToPage, _sourceDpi, _targetDpi);
            if (pen is null)
            {
                return;
            }
            gfx.DrawPath(pen, graphicsPath);
        }
    }
}
