// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Style;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
#if WPF
using System.Windows.Media.Imaging;
#endif

#if WPF
namespace Renderer.PdfSharp_wpf
#elif CORE
namespace Renderer.PdfSharp_core
#endif
{
    /// <summary>
    /// Native PdfSharp shape renderer.
    /// </summary>
    public partial class PdfRenderer : Core2D.Renderer.ShapeRenderer
    {
        private bool _enableImageCache = true;
        private IDictionary<string, XImage> _biCache;
        private Func<double, double> _scaleToPage;
        private double _sourceDpi = 96.0;
        private double _targetDpi = 72.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRenderer"/> class.
        /// </summary>
        public PdfRenderer()
        {
            ClearCache(isZooming: false);
            _scaleToPage = (value) => (float)(value * 1.0);
        }

        /// <summary>
        /// Creates a new <see cref="PdfRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="PdfRenderer"/> class.</returns>
        public static Core2D.Renderer.ShapeRenderer Create() => new PdfRenderer();

        private static XColor ToXColor(Core2D.Style.ArgbColor color) => XColor.FromArgb(color.A, color.R, color.G, color.B);

        private static XPen ToXPen(Core2D.Style.BaseStyle style, Func<double, double> scale, double sourceDpi, double targetDpi)
        {
            var pen = new XPen(ToXColor(style.Stroke), scale(style.Thickness * targetDpi / sourceDpi));
            switch (style.LineCap)
            {
                case Core2D.Style.LineCap.Flat:
                    pen.LineCap = XLineCap.Flat;
                    break;
                case Core2D.Style.LineCap.Square:
                    pen.LineCap = XLineCap.Square;
                    break;
                case Core2D.Style.LineCap.Round:
                    pen.LineCap = XLineCap.Round;
                    break;
            }
            if (style.Dashes != null)
            {
                // TODO: Convert to correct dash values.
                pen.DashPattern = Core2D.Style.ShapeStyle.ConvertDashesToDoubleArray(style.Dashes);
            }
            pen.DashOffset = style.DashOffset;
            return pen;
        }

        private static XSolidBrush ToXSolidBrush(Core2D.Style.ArgbColor color)
        {
            return new XSolidBrush(ToXColor(color));
        }

        private static void DrawLineInternal(XGraphics gfx, XPen pen, bool isStroked, ref XPoint p0, ref XPoint p1)
        {
            if (isStroked)
            {
                gfx.DrawLine(pen, p0, p1);
            }
        }

        private static void DrawLineCurveInternal(XGraphics gfx, XPen pen, bool isStroked, ref XPoint pt1, ref XPoint pt2, double curvature, Core2D.Style.CurveOrientation orientation, Core2D.Shape.PointAlignment pt1a, Core2D.Shape.PointAlignment pt2a)
        {
            if (isStroked)
            {
                var path = new XGraphicsPath();
                double p1x = pt1.X;
                double p1y = pt1.Y;
                double p2x = pt2.X;
                double p2y = pt2.Y;
                Core2D.Shapes.XLineExtensions.GetCurvedLineBezierControlPoints(orientation, curvature, pt1a, pt2a, ref p1x, ref p1y, ref p2x, ref p2y);
                path.AddBezier(
                    pt1.X, pt1.Y,
                    p1x, p1y,
                    p2x, p2y,
                    pt2.X, pt2.Y);
                gfx.DrawPath(pen, path);
            }
        }

        private void DrawLineArrowsInternal(XGraphics gfx, Core2D.Shapes.XLine line, double dx, double dy, out XPoint pt1, out XPoint pt2)
        {
            XSolidBrush fillStartArrow = ToXSolidBrush(line.Style.StartArrowStyle.Fill);
            XPen strokeStartArrow = ToXPen(line.Style.StartArrowStyle, _scaleToPage, _sourceDpi, _targetDpi);

            XSolidBrush fillEndArrow = ToXSolidBrush(line.Style.EndArrowStyle.Fill);
            XPen strokeEndArrow = ToXPen(line.Style.EndArrowStyle, _scaleToPage, _sourceDpi, _targetDpi);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            Core2D.Shapes.XLineExtensions.GetMaxLength(line, ref _x1, ref _y1, ref _x2, ref _y2);

            double x1 = _scaleToPage(_x1);
            double y1 = _scaleToPage(_y1);
            double x2 = _scaleToPage(_x2);
            double y2 = _scaleToPage(_y2);

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI;
            double a2 = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;

            // Draw start arrow.
            pt1 = DrawLineArrowInternal(gfx, strokeStartArrow, fillStartArrow, x1, y1, a1, sas);

            // Draw end arrow.
            pt2 = DrawLineArrowInternal(gfx, strokeEndArrow, fillEndArrow, x2, y2, a2, eas);
        }

        private static XPoint DrawLineArrowInternal(XGraphics gfx, XPen pen, XSolidBrush brush, double x, double y, double angle, Core2D.Style.ArrowStyle style)
        {
            XPoint pt;
            var rt = new XMatrix();
            var c = new XPoint(x, y);
            rt.RotateAtPrepend(angle, c);
            double rx = style.RadiusX;
            double ry = style.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;

            switch (style.ArrowType)
            {
                default:
                case Core2D.Style.ArrowType.None:
                    {
                        pt = new XPoint(x, y);
                    }
                    break;
                case Core2D.Style.ArrowType.Rectangle:
                    {
                        pt = rt.Transform(new XPoint(x - sx, y));
                        var rect = new XRect(x - sx, y - ry, sx, sy);
                        gfx.Save();
                        gfx.RotateAtTransform(angle, c);
                        DrawRectangleInternal(gfx, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                        gfx.Restore();
                    }
                    break;
                case Core2D.Style.ArrowType.Ellipse:
                    {
                        pt = rt.Transform(new XPoint(x - sx, y));
                        gfx.Save();
                        gfx.RotateAtTransform(angle, c);
                        var rect = new XRect(x - sx, y - ry, sx, sy);
                        DrawEllipseInternal(gfx, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                        gfx.Restore();
                    }
                    break;
                case Core2D.Style.ArrowType.Arrow:
                    {
                        pt = rt.Transform(new XPoint(x, y));
                        var p11 = rt.Transform(new XPoint(x - sx, y + sy));
                        var p21 = rt.Transform(new XPoint(x, y));
                        var p12 = rt.Transform(new XPoint(x - sx, y - sy));
                        var p22 = rt.Transform(new XPoint(x, y));
                        DrawLineInternal(gfx, pen, style.IsStroked, ref p11, ref p21);
                        DrawLineInternal(gfx, pen, style.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            return pt;
        }

        private static void DrawRectangleInternal(XGraphics gfx, XSolidBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
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

        private static void DrawEllipseInternal(XGraphics gfx, XSolidBrush brush, XPen pen, bool isStroked, bool isFilled, ref XRect rect)
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

        private void DrawGridInternal(XGraphics gfx, XPen stroke, ref Core2D.Spatial.Rect2 rect, double offsetX, double offsetY, double cellWidth, double cellHeight, bool isStroked)
        {
            double ox = rect.X;
            double oy = rect.Y;
            double sx = ox + offsetX;
            double sy = oy + offsetY;
            double ex = ox + rect.Width;
            double ey = oy + rect.Height;

            for (double x = sx; x < ex; x += cellWidth)
            {
                var p0 = new XPoint(
                    _scaleToPage(x),
                    _scaleToPage(oy));
                var p1 = new XPoint(
                    _scaleToPage(x),
                    _scaleToPage(ey));
                DrawLineInternal(gfx, stroke, isStroked, ref p0, ref p1);
            }

            for (double y = sy; y < ey; y += cellHeight)
            {
                var p0 = new XPoint(
                    _scaleToPage(ox),
                    _scaleToPage(y));
                var p1 = new XPoint(
                    _scaleToPage(ex),
                    _scaleToPage(y));
                DrawLineInternal(gfx, stroke, isStroked, ref p0, ref p1);
            }
        }

        /// <inheritdoc/>
        public override void ClearCache(bool isZooming)
        {
            if (!isZooming)
            {
                if (_biCache != null)
                {
                    foreach (var kvp in _biCache)
                    {
                        kvp.Value.Dispose();
                    }
                    _biCache.Clear();
                }
                _biCache = new Dictionary<string, XImage>();
            }
        }

        /// <inheritdoc/>
        public override void Fill(object dc, double x, double y, double width, double height, ArgbColor color)
        {
            var _gfx = dc as XGraphics;
            _gfx.DrawRectangle(
                ToXSolidBrush(color), 
                _scaleToPage(x),
                _scaleToPage(y),
                _scaleToPage(width),
                _scaleToPage(height));
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XLine line, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            if (!line.IsStroked)
                return;

            var _gfx = dc as XGraphics;

            XPen strokeLine = ToXPen(line.Style, _scaleToPage, _sourceDpi, _targetDpi);
            XPoint pt1, pt2;

            DrawLineArrowsInternal(_gfx, line, dx, dy, out pt1, out pt2);

            if (line.Style.LineStyle.IsCurved)
            {
                DrawLineCurveInternal(
                    _gfx,
                    strokeLine, line.IsStroked,
                    ref pt1, ref pt2,
                    line.Style.LineStyle.Curvature,
                    line.Style.LineStyle.CurveOrientation,
                    line.Start.Alignment,
                    line.End.Alignment);
            }
            else
            {
                DrawLineInternal(_gfx, strokeLine, line.IsStroked, ref pt1, ref pt2);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XRectangle rectangle, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var rect = Core2D.Spatial.Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y,
                dx, dy);

            if (rectangle.IsStroked && rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(rectangle.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    ToXSolidBrush(rectangle.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (rectangle.IsStroked && !rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXPen(rectangle.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (!rectangle.IsStroked && rectangle.IsFilled)
            {
                _gfx.DrawRectangle(
                    ToXSolidBrush(rectangle.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    _gfx,
                    ToXPen(rectangle.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    ref rect,
                    rectangle.OffsetX, rectangle.OffsetY,
                    rectangle.CellWidth, rectangle.CellHeight,
                    true);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XEllipse ellipse, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var rect = Core2D.Spatial.Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y,
                dx, dy);

            if (ellipse.IsStroked && ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXPen(ellipse.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    ToXSolidBrush(ellipse.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (ellipse.IsStroked && !ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXPen(ellipse.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
            else if (!ellipse.IsStroked && ellipse.IsFilled)
            {
                _gfx.DrawEllipse(
                    ToXSolidBrush(ellipse.Style.Fill),
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XArc arc, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var a = new Core2D.Spatial.Arc.GdiArc(
                Core2D.Spatial.Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Core2D.Spatial.Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Core2D.Spatial.Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Core2D.Spatial.Point2.FromXY(arc.Point4.X, arc.Point4.Y));

            if (arc.IsFilled)
            {
                var path = new XGraphicsPath();
                // NOTE: Not implemented in PdfSharp Core version.
                path.AddArc(
                    _scaleToPage(a.X + dx),
                    _scaleToPage(a.Y + dy),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    a.StartAngle,
                    a.SweepAngle);

                if (arc.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(arc.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        ToXSolidBrush(arc.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXSolidBrush(arc.Style.Fill),
                        path);
                }
            }
            else
            {
                if (arc.IsStroked)
                {
                    _gfx.DrawArc(
                        ToXPen(arc.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        _scaleToPage(a.X + dx),
                        _scaleToPage(a.Y + dy),
                        _scaleToPage(a.Width),
                        _scaleToPage(a.Height),
                        a.StartAngle,
                        a.SweepAngle);
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XCubicBezier cubicBezier, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            if (cubicBezier.IsFilled)
            {
                var path = new XGraphicsPath();
                path.AddBezier(
                    _scaleToPage(cubicBezier.Point1.X + dx),
                    _scaleToPage(cubicBezier.Point1.Y + dy),
                    _scaleToPage(cubicBezier.Point2.X + dx),
                    _scaleToPage(cubicBezier.Point2.Y + dy),
                    _scaleToPage(cubicBezier.Point3.X + dx),
                    _scaleToPage(cubicBezier.Point3.Y + dy),
                    _scaleToPage(cubicBezier.Point4.X + dx),
                    _scaleToPage(cubicBezier.Point4.Y + dy));

                if (cubicBezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(cubicBezier.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        ToXSolidBrush(cubicBezier.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXSolidBrush(cubicBezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (cubicBezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(cubicBezier.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        _scaleToPage(cubicBezier.Point1.X + dx),
                        _scaleToPage(cubicBezier.Point1.Y + dy),
                        _scaleToPage(cubicBezier.Point2.X + dx),
                        _scaleToPage(cubicBezier.Point2.Y + dy),
                        _scaleToPage(cubicBezier.Point3.X + dx),
                        _scaleToPage(cubicBezier.Point3.Y + dy),
                        _scaleToPage(cubicBezier.Point4.X + dx),
                        _scaleToPage(cubicBezier.Point4.Y + dy));
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XQuadraticBezier quadraticBezier, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            double x1 = quadraticBezier.Point1.X;
            double y1 = quadraticBezier.Point1.Y;
            double x2 = quadraticBezier.Point1.X + (2.0 * (quadraticBezier.Point2.X - quadraticBezier.Point1.X)) / 3.0;
            double y2 = quadraticBezier.Point1.Y + (2.0 * (quadraticBezier.Point2.Y - quadraticBezier.Point1.Y)) / 3.0;
            double x3 = x2 + (quadraticBezier.Point3.X - quadraticBezier.Point1.X) / 3.0;
            double y3 = y2 + (quadraticBezier.Point3.Y - quadraticBezier.Point1.Y) / 3.0;
            double x4 = quadraticBezier.Point3.X;
            double y4 = quadraticBezier.Point3.Y;

            if (quadraticBezier.IsFilled)
            {
                var path = new XGraphicsPath();
                path.AddBezier(
                    _scaleToPage(x1 + dx),
                    _scaleToPage(y1 + dy),
                    _scaleToPage(x2 + dx),
                    _scaleToPage(y2 + dy),
                    _scaleToPage(x3 + dx),
                    _scaleToPage(y3 + dy),
                    _scaleToPage(x4 + dx),
                    _scaleToPage(y4 + dy));

                if (quadraticBezier.IsStroked)
                {
                    _gfx.DrawPath(
                        ToXPen(quadraticBezier.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        ToXSolidBrush(quadraticBezier.Style.Fill),
                        path);
                }
                else
                {
                    _gfx.DrawPath(
                        ToXSolidBrush(quadraticBezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (quadraticBezier.IsStroked)
                {
                    _gfx.DrawBezier(
                        ToXPen(quadraticBezier.Style, _scaleToPage, _sourceDpi, _targetDpi),
                        _scaleToPage(x1 + dx),
                        _scaleToPage(y1 + dy),
                        _scaleToPage(x2 + dx),
                        _scaleToPage(y2 + dy),
                        _scaleToPage(x3 + dx),
                        _scaleToPage(y3 + dy),
                        _scaleToPage(x4 + dx),
                        _scaleToPage(y4 + dy));
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XText text, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var tbind = text.BindText(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            var options = new XPdfFontOptions(PdfFontEncoding.Unicode);

            var fontStyle = XFontStyle.Regular;
            if (text.Style.TextStyle.FontStyle != null)
            {
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Bold))
                {
                    fontStyle |= XFontStyle.Bold;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Italic))
                {
                    fontStyle |= XFontStyle.Italic;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Underline))
                {
                    fontStyle |= XFontStyle.Underline;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Strikeout))
                {
                    fontStyle |= XFontStyle.Strikeout;
                }
            }

            var font = new XFont(
                text.Style.TextStyle.FontName,
                _scaleToPage(text.Style.TextStyle.FontSize),
                fontStyle,
                options);

            var rect = Core2D.Spatial.Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y,
                dx, dy);

            var srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            var format = new XStringFormat();
            switch (text.Style.TextStyle.TextHAlignment)
            {
                case Core2D.Style.TextHAlignment.Left:
                    format.Alignment = XStringAlignment.Near;
                    break;
                case Core2D.Style.TextHAlignment.Center:
                    format.Alignment = XStringAlignment.Center;
                    break;
                case Core2D.Style.TextHAlignment.Right:
                    format.Alignment = XStringAlignment.Far;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                case Core2D.Style.TextVAlignment.Top:
                    format.LineAlignment = XLineAlignment.Near;
                    break;
                case Core2D.Style.TextVAlignment.Center:
                    format.LineAlignment = XLineAlignment.Center;
                    break;
                case Core2D.Style.TextVAlignment.Bottom:
                    format.LineAlignment = XLineAlignment.Far;
                    break;
            }

            _gfx.DrawString(
                tbind,
                font,
                ToXSolidBrush(text.Style.Stroke),
                srect,
                format);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XImage image, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var rect = Core2D.Spatial.Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y,
                dx, dy);

            var srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            if (image.IsStroked || image.IsFilled)
            {
                DrawRectangleInternal(
                    _gfx,
                    ToXSolidBrush(image.Style.Fill),
                    ToXPen(image.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    image.IsStroked,
                    image.IsFilled,
                    ref srect);
            }

            if (_enableImageCache
                && _biCache.ContainsKey(image.Key))
            {
                _gfx.DrawImage(_biCache[image.Key], srect);
            }
            else
            {
                if (State.ImageCache == null || string.IsNullOrEmpty(image.Key))
                    return;

                var bytes = State.ImageCache.GetImage(image.Key);
                if (bytes != null)
                {
                    var ms = new System.IO.MemoryStream(bytes);
#if WPF
                    var bs = new BitmapImage();
                    bs.BeginInit();
                    bs.StreamSource = ms;
                    bs.EndInit();
                    bs.Freeze();
                    var bi = XImage.FromBitmapSource(bs);
#else
                    var bi = XImage.FromStream(ms);
#endif
                    if (_enableImageCache)
                        _biCache[image.Key] = bi;

                    _gfx.DrawImage(bi, srect);

                    if (!_enableImageCache)
                        bi.Dispose();
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, Core2D.Shapes.XPath path, double dx, double dy, ImmutableArray<Core2D.Data.XProperty> db, Core2D.Data.Database.XRecord r)
        {
            var _gfx = dc as XGraphics;

            var gp = path.Geometry.ToXGraphicsPath(dx, dy, _scaleToPage);

            if (path.IsFilled && path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXPen(path.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    ToXSolidBrush(path.Style.Fill),
                    gp);
            }
            else if (path.IsFilled && !path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXSolidBrush(path.Style.Fill),
                    gp);
            }
            else if (!path.IsFilled && path.IsStroked)
            {
                _gfx.DrawPath(
                    ToXPen(path.Style, _scaleToPage, _sourceDpi, _targetDpi),
                    gp);
            }
        }
    }
}
