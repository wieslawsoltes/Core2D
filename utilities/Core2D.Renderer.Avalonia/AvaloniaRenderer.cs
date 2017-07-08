// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Core2D.Data;
using Spatial;
using Spatial.Arc;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AMI = Avalonia.Media.Imaging;
using APAZ = Avalonia.Controls.PanAndZoom;

namespace Core2D.Renderer.Avalonia
{
    /// <summary>
    /// Native Avalonia shape renderer.
    /// </summary>
    public class AvaloniaRenderer : ShapeRenderer
    {
        private bool _enableImageCache = true;
        private IDictionary<string, AMI.Bitmap> _biCache;
        private Func<double, float> _scaleToPage;
        private double _textScaleFactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaRenderer"/> class.
        /// </summary>
        /// <param name="textScaleFactor"></param>
        public AvaloniaRenderer(double textScaleFactor = 1.0)
        {
            ClearCache(isZooming: false);
            _textScaleFactor = textScaleFactor;
            _scaleToPage = (value) => (float)(value);
        }

        /// <summary>
        /// Creates a new <see cref="AvaloniaRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="AvaloniaRenderer"/> class.</returns>
        public static ShapeRenderer Create() => new AvaloniaRenderer();

        private A.Point GetTextOrigin(ShapeStyle style, ref Rect2 rect, ref A.Size size)
        {
            double ox, oy;

            switch (style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    ox = rect.X;
                    break;
                case TextHAlignment.Right:
                    ox = rect.Right - size.Width;
                    break;
                case TextHAlignment.Center:
                default:
                    ox = (rect.Left + rect.Width / 2.0) - (size.Width / 2.0);
                    break;
            }

            switch (style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    oy = rect.Y;
                    break;
                case TextVAlignment.Bottom:
                    oy = rect.Bottom - size.Height;
                    break;
                case TextVAlignment.Center:
                default:
                    oy = (rect.Bottom - rect.Height / 2f) - (size.Height / 2f);
                    break;
            }

            return new A.Point(ox, oy);
        }

        private static AM.Color ToColor(ArgbColor color) => AM.Color.FromArgb(color.A, color.R, color.G, color.B);

        private AM.Pen ToPen(BaseStyle style, Func<double, float> scale)
        {
            var lineCap = default(AM.PenLineCap);
            var dashStyle = default(AM.DashStyle);

            switch (style.LineCap)
            {
                case LineCap.Flat:
                    lineCap = AM.PenLineCap.Flat;
                    break;
                case LineCap.Square:
                    lineCap = AM.PenLineCap.Square;
                    break;
                case LineCap.Round:
                    lineCap = AM.PenLineCap.Round;
                    break;
            }

            if (style.Dashes != null)
            {
                dashStyle = new AM.DashStyle(
                    ShapeStyle.ConvertDashesToDoubleArray(style.Dashes),
                    style.DashOffset);
            }

            var pen = new AM.Pen(
                ToBrush(style.Stroke),
                scale(style.Thickness / State.ZoomX),
                dashStyle, lineCap,
                lineCap, lineCap);

            return pen;
        }

        private AM.IBrush ToBrush(ArgbColor color)
        {
            return new AM.SolidColorBrush(ToColor(color));
        }

        private static Rect2 CreateRect(PointShape tl, PointShape br, double dx, double dy)
        {
            return Rect2.FromPoints(tl.X, tl.Y, br.X, br.Y, dx, dy);
        }

        private static void DrawLineInternal(AM.DrawingContext dc, AM.Pen pen, bool isStroked, ref A.Point p0, ref A.Point p1)
        {
            if (isStroked)
            {
                dc.DrawLine(pen, p0, p1);
            }
        }

        private static void DrawLineCurveInternal(AM.DrawingContext _dc, AM.Pen pen, bool isStroked, ref A.Point pt1, ref A.Point pt2, double curvature, CurveOrientation orientation, PointAlignment pt1a, PointAlignment pt2a)
        {
            if (isStroked)
            {
                var sg = new AM.StreamGeometry();
                using (var sgc = sg.Open())
                {
                    sgc.BeginFigure(new A.Point(pt1.X, pt1.Y), false);
                    double p1x = pt1.X;
                    double p1y = pt1.Y;
                    double p2x = pt2.X;
                    double p2y = pt2.Y;
                    LineShapeExtensions.GetCurvedLineBezierControlPoints(orientation, curvature, pt1a, pt2a, ref p1x, ref p1y, ref p2x, ref p2y);
                    sgc.CubicBezierTo(
                        new A.Point(p1x, p1y),
                        new A.Point(p2x, p2y),
                        new A.Point(pt2.X, pt2.Y));
                    sgc.EndFigure(false);
                }
                _dc.DrawGeometry(null, pen, sg);
            }
        }

        private void DrawLineArrowsInternal(AM.DrawingContext dc, LineShape line, double dx, double dy, out A.Point pt1, out A.Point pt2)
        {
            AM.IBrush fillStartArrow = ToBrush(line.Style.StartArrowStyle.Fill);
            AM.Pen strokeStartArrow = ToPen(line.Style.StartArrowStyle, _scaleToPage);

            AM.IBrush fillEndArrow = ToBrush(line.Style.EndArrowStyle.Fill);
            AM.Pen strokeEndArrow = ToPen(line.Style.EndArrowStyle, _scaleToPage);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            line.GetMaxLength(ref _x1, ref _y1, ref _x2, ref _y2);

            float x1 = _scaleToPage(_x1);
            float y1 = _scaleToPage(_y1);
            float x2 = _scaleToPage(_x2);
            float y2 = _scaleToPage(_y2);

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2);
            double a2 = Math.Atan2(y2 - y1, x2 - x1);

            // Draw start arrow.
            pt1 = DrawLineArrowInternal(dc, strokeStartArrow, fillStartArrow, x1, y1, a1, sas);

            // Draw end arrow.
            pt2 = DrawLineArrowInternal(dc, strokeEndArrow, fillEndArrow, x2, y2, a2, eas);
        }

        private static A.Point DrawLineArrowInternal(AM.DrawingContext dc, AM.Pen pen, AM.IBrush brush, float x, float y, double angle, ArrowStyle style)
        {
            A.Point pt = default(A.Point);
            var rt = APAZ.MatrixHelper.Rotation(angle, new A.Vector(x, y));
            double rx = style.RadiusX;
            double ry = style.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;

            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt = new A.Point(x, y);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt = APAZ.MatrixHelper.TransformPoint(rt, new A.Point(x - (float)sx, y));
                        var rect = new Rect2(x - sx, y - ry, sx, sy);
                        using (var d = dc.PushPreTransform(rt))
                        {
                            DrawRectangleInternal(dc, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                        }
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt = APAZ.MatrixHelper.TransformPoint(rt, new A.Point(x - (float)sx, y));
                        using (var d = dc.PushPreTransform(rt))
                        {
                            var rect = new Rect2(x - sx, y - ry, sx, sy);
                            DrawEllipseInternal(dc, brush, pen, style.IsStroked, style.IsFilled, ref rect);
                        }
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        var pts = new A.Point[]
                        {
                            new A.Point(x, y),
                            new A.Point(x - (float)sx, y + (float)sy),
                            new A.Point(x, y),
                            new A.Point(x - (float)sx, y - (float)sy),
                            new A.Point(x, y)
                        };
                        pt = APAZ.MatrixHelper.TransformPoint(rt, pts[0]);
                        var p11 = APAZ.MatrixHelper.TransformPoint(rt, pts[1]);
                        var p21 = APAZ.MatrixHelper.TransformPoint(rt, pts[2]);
                        var p12 = APAZ.MatrixHelper.TransformPoint(rt, pts[3]);
                        var p22 = APAZ.MatrixHelper.TransformPoint(rt, pts[4]);
                        DrawLineInternal(dc, pen, style.IsStroked, ref p11, ref p21);
                        DrawLineInternal(dc, pen, style.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            return pt;
        }

        private static void DrawRectangleInternal(AM.DrawingContext dc, AM.IBrush brush, AM.Pen pen, bool isStroked, bool isFilled, ref Rect2 rect)
        {
            if (!isStroked && !isFilled)
                return;

            var r = new A.Rect(rect.X, rect.Y, rect.Width, rect.Height);

            if (isFilled)
            {
                dc.FillRectangle(brush, r);
            }

            if (isStroked)
            {
                dc.DrawRectangle(pen, r);
            }
        }

        private static void DrawEllipseInternal(AM.DrawingContext dc, AM.IBrush brush, AM.Pen pen, bool isStroked, bool isFilled, ref Rect2 rect)
        {
            if (!isFilled && !isStroked)
                return;

            var r = new A.Rect(rect.X, rect.Y, rect.Width, rect.Height);
            var g = new AM.EllipseGeometry(r);

            dc.DrawGeometry(
                isFilled ? brush : null,
                isStroked ? pen : null,
                g);
        }

        private void DrawGridInternal(AM.DrawingContext dc, AM.Pen stroke, ref Rect2 rect, double offsetX, double offsetY, double cellWidth, double cellHeight, bool isStroked)
        {
            double ox = rect.X;
            double oy = rect.Y;
            double sx = ox + offsetX;
            double sy = oy + offsetY;
            double ex = ox + rect.Width;
            double ey = oy + rect.Height;

            for (double x = sx; x < ex; x += cellWidth)
            {
                var p0 = new A.Point(
                    _scaleToPage(x),
                    _scaleToPage(oy));
                var p1 = new A.Point(
                    _scaleToPage(x),
                    _scaleToPage(ey));
                DrawLineInternal(dc, stroke, isStroked, ref p0, ref p1);
            }

            for (double y = sy; y < ey; y += cellHeight)
            {
                var p0 = new A.Point(
                    _scaleToPage(ox),
                    _scaleToPage(y));
                var p1 = new A.Point(
                    _scaleToPage(ex),
                    _scaleToPage(y));
                DrawLineInternal(dc, stroke, isStroked, ref p0, ref p1);
            }
        }

        private A.Matrix ToMatrix(MatrixObject matrix)
        {
            return new A.Matrix(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.OffsetX, matrix.OffsetY);
        }

        /// <inheritdoc/>
        public override void ClearCache(bool isZooming)
        {
            if (!isZooming)
            {
                if (_biCache != null)
                {
                    _biCache.Clear();
                }
                _biCache = new Dictionary<string, AMI.Bitmap>();
            }
        }

        /// <inheritdoc/>
        public override void Fill(object dc, double x, double y, double width, double height, ArgbColor color)
        {
            var _dc = dc as AM.DrawingContext;
            var brush = ToBrush(color);
            var rect = new A.Rect(x, y, width, height);
            _dc.FillRectangle(brush, rect);
        }

        /// <inheritdoc/>
        public override object PushMatrix(object dc, MatrixObject matrix)
        {
            var _dc = dc as AM.DrawingContext;
            return _dc.PushPreTransform(ToMatrix(matrix));
        }

        /// <inheritdoc/>
        public override void PopMatrix(object dc, object state)
        {
            var _state = (AM.DrawingContext.PushedState)state;
            _state.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, LineShape line, double dx, double dy, object db, object r)
        {
            var _dc = dc as AM.DrawingContext;

            AM.Pen strokeLine = ToPen(line.Style, _scaleToPage);
            DrawLineArrowsInternal(_dc, line, dx, dy, out A.Point pt1, out A.Point pt2);

            if (line.Style.LineStyle.IsCurved)
            {
                DrawLineCurveInternal(
                    _dc,
                    strokeLine, line.IsStroked,
                    ref pt1, ref pt2,
                    line.Style.LineStyle.Curvature,
                    line.Style.LineStyle.CurveOrientation,
                    line.Start.Alignment,
                    line.End.Alignment);
            }
            else
            {
                DrawLineInternal(_dc, strokeLine, line.IsStroked, ref pt1, ref pt2);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, RectangleShape rectangle, double dx, double dy, object db, object r)
        {
            var _dc = dc as AM.DrawingContext;

            AM.IBrush brush = ToBrush(rectangle.Style.Fill);
            AM.Pen pen = ToPen(rectangle.Style, _scaleToPage);

            var rect = CreateRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);

            DrawRectangleInternal(
                _dc,
                brush,
                pen,
                rectangle.IsStroked,
                rectangle.IsFilled,
                ref rect);

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    _dc,
                    pen,
                    ref rect,
                    rectangle.OffsetX, rectangle.OffsetY,
                    rectangle.CellWidth, rectangle.CellHeight,
                    true);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, EllipseShape ellipse, double dx, double dy, object db, object r)
        {
            var _dc = dc as AM.DrawingContext;

            AM.IBrush brush = ToBrush(ellipse.Style.Fill);
            AM.Pen pen = ToPen(ellipse.Style, _scaleToPage);

            var rect = CreateRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);

            DrawEllipseInternal(
                _dc,
                brush,
                pen,
                ellipse.IsStroked,
                ellipse.IsFilled,
                ref rect);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ArcShape arc, double dx, double dy, object db, object r)
        {
            if (!arc.IsFilled && !arc.IsStroked)
                return;

            var _dc = dc as AM.DrawingContext;

            AM.IBrush brush = ToBrush(arc.Style.Fill);
            AM.Pen pen = ToPen(arc.Style, _scaleToPage);

            var sg = new AM.StreamGeometry();
            using (var sgc = sg.Open())
            {
                var a = new WpfArc(
                    Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                    Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                    Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                    Point2.FromXY(arc.Point4.X, arc.Point4.Y));

                sgc.BeginFigure(
                    new A.Point(a.Start.X + dx, a.Start.Y),
                    arc.IsFilled);

                sgc.ArcTo(
                    new A.Point(a.End.X + dx, a.End.Y + dy),
                    new A.Size(a.Radius.Width, a.Radius.Height),
                    0.0,
                    a.IsLargeArc,
                    AM.SweepDirection.Clockwise);

                sgc.EndFigure(false);
            }

            _dc.DrawGeometry(
                arc.IsFilled ? brush : null,
                arc.IsStroked ? pen : null,
                sg);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, CubicBezierShape cubicBezier, double dx, double dy, object db, object r)
        {
            if (!cubicBezier.IsFilled && !cubicBezier.IsStroked)
                return;

            var _dc = dc as AM.DrawingContext;

            AM.IBrush brush = ToBrush(cubicBezier.Style.Fill);
            AM.Pen pen = ToPen(cubicBezier.Style, _scaleToPage);

            var sg = new AM.StreamGeometry();
            using (var sgc = sg.Open())
            {
                sgc.BeginFigure(
                    new A.Point(cubicBezier.Point1.X, cubicBezier.Point1.Y),
                    cubicBezier.IsFilled);

                sgc.CubicBezierTo(
                    new A.Point(cubicBezier.Point2.X, cubicBezier.Point2.Y),
                    new A.Point(cubicBezier.Point3.X, cubicBezier.Point3.Y),
                    new A.Point(cubicBezier.Point4.X, cubicBezier.Point4.Y));

                sgc.EndFigure(false);
            }

            _dc.DrawGeometry(
                cubicBezier.IsFilled ? brush : null,
                cubicBezier.IsStroked ? pen : null,
                sg);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, QuadraticBezierShape quadraticBezier, double dx, double dy, object db, object r)
        {
            if (!quadraticBezier.IsFilled && !quadraticBezier.IsStroked)
                return;

            var _dc = dc as AM.DrawingContext;

            AM.IBrush brush = ToBrush(quadraticBezier.Style.Fill);
            AM.Pen pen = ToPen(quadraticBezier.Style, _scaleToPage);

            var sg = new AM.StreamGeometry();
            using (var sgc = sg.Open())
            {
                sgc.BeginFigure(
                    new A.Point(quadraticBezier.Point1.X, quadraticBezier.Point1.Y),
                    quadraticBezier.IsFilled);

                sgc.QuadraticBezierTo(
                    new A.Point(quadraticBezier.Point2.X, quadraticBezier.Point2.Y),
                    new A.Point(quadraticBezier.Point3.X, quadraticBezier.Point3.Y));

                sgc.EndFigure(false);
            }

            _dc.DrawGeometry(
                quadraticBezier.IsFilled ? brush : null,
                quadraticBezier.IsStroked ? pen : null,
                sg);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, TextShape text, double dx, double dy, object db, object r)
        {
            var _gfx = dc as AM.DrawingContext;

            var properties = (ImmutableArray<Property>)db;
            var record = (Record)r;
            var tbind = text.BindText(properties, record);
            if (string.IsNullOrEmpty(tbind))
                return;

            AM.IBrush brush = ToBrush(text.Style.Stroke);

            var fontStyle = AM.FontStyle.Normal;
            var fontWeight = AM.FontWeight.Normal;
            //var fontDecoration = PM.FontDecoration.None;

            if (text.Style.TextStyle.FontStyle != null)
            {
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
                {
                    fontStyle |= AM.FontStyle.Italic;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
                {
                    fontWeight |= AM.FontWeight.Bold;
                }

                // TODO: Implement font decoration after Avalonia adds support.
                /*
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Underline))
                {
                    fontDecoration |= PM.FontDecoration.Underline;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Strikeout))
                {
                    fontDecoration |= PM.FontDecoration.Strikethrough;
                }
                */
            }

            if (text.Style.TextStyle.FontSize >= 0.0)
            {
                var tf = new AM.Typeface(
                    text.Style.TextStyle.FontName,
                    text.Style.TextStyle.FontSize * _textScaleFactor,
                    fontStyle,
                    fontWeight);

                var ft = new AM.FormattedText()
                {
                    Typeface = tf,
                    Text = tbind,
                    TextAlignment = AM.TextAlignment.Left,
                    Wrapping = AM.TextWrapping.NoWrap
                };

                var rect = CreateRect(text.TopLeft, text.BottomRight, dx, dy);
                var size = ft.Measure();
                var origin = GetTextOrigin(text.Style, ref rect, ref size);

                _gfx.DrawText(brush, origin, ft);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ImageShape image, double dx, double dy, object db, object r)
        {
            var _dc = dc as AM.DrawingContext;

            var rect = CreateRect(image.TopLeft, image.BottomRight, dx, dy);

            if (image.IsStroked || image.IsFilled)
            {
                AM.IBrush brush = ToBrush(image.Style.Fill);
                AM.Pen pen = ToPen(image.Style, _scaleToPage);

                DrawRectangleInternal(
                    _dc,
                    brush,
                    pen,
                    image.IsStroked,
                    image.IsFilled,
                    ref rect);
            }

            if (_enableImageCache
                && _biCache.ContainsKey(image.Key))
            {
                try
                {
                    var bi = _biCache[image.Key];
                    _dc.DrawImage(
                        bi,
                        1.0,
                        new A.Rect(0, 0, bi.PixelWidth, bi.PixelHeight),
                        new A.Rect(rect.X, rect.Y, rect.Width, rect.Height));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
            else
            {
                if (State.ImageCache == null || string.IsNullOrEmpty(image.Key))
                    return;

                try
                {
                    var bytes = State.ImageCache.GetImage(image.Key);
                    if (bytes != null)
                    {
                        using (var ms = new System.IO.MemoryStream(bytes))
                        {
                            var bi = new AMI.Bitmap(ms);

                            if (_enableImageCache)
                                _biCache[image.Key] = bi;

                            _dc.DrawImage(
                                bi,
                                1.0,
                                new A.Rect(0, 0, bi.PixelWidth, bi.PixelHeight),
                                new A.Rect(rect.X, rect.Y, rect.Width, rect.Height));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, PathShape path, double dx, double dy, object db, object r)
        {
            if (!path.IsFilled && !path.IsStroked)
                return;

            var _dc = dc as AM.DrawingContext;

            var g = path.Geometry.ToGeometry(dx, dy);

            var brush = ToBrush(path.Style.Fill);
            var pen = ToPen(path.Style, _scaleToPage);
            _dc.DrawGeometry(
                path.IsFilled ? brush : null,
                path.IsStroked ? pen : null,
                g);
        }
    }
}
