// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Math;
using Core2D.Math.Arc;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using P = Perspex;
using PM = Perspex.Media;
using PMI = Perspex.Media.Imaging;

namespace Renderer.Perspex
{
    /// <summary>
    /// Native Perspex shape renderer.
    /// </summary>
    public class PerspexRenderer : ShapeRenderer
    {
        private bool _enableImageCache = true;
        private IDictionary<string, PMI.Bitmap> _biCache;

        /// <summary>
        /// 
        /// </summary>
        private Func<double, float> _scaleToPage;

        /// <summary>
        /// 
        /// </summary>
        private double _textScaleFactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerspexRenderer"/> class.
        /// </summary>
        /// <param name="textScaleFactor"></param>
        public PerspexRenderer(double textScaleFactor = 1.0)
        {
            ClearCache(isZooming: false);

            _textScaleFactor = textScaleFactor;
            _scaleToPage = (value) => (float)(value);
        }

        /// <summary>
        /// Creates a new <see cref="PerspexRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="PerspexRenderer"/> class.</returns>
        public static ShapeRenderer Create()
        {
            return new PerspexRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private P.Point GetTextOrigin(ShapeStyle style, ref Rect2 rect, ref P.Size size)
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

            return new P.Point(ox, oy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private PM.Color ToColor(ArgbColor color)
        {
            return PM.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private PM.Pen ToPen(BaseStyle style, Func<double, float> scale)
        {
            var lineCap = default(PM.PenLineCap);
            var dashStyle = default(PM.DashStyle);

            switch (style.LineCap)
            {
                case LineCap.Flat:
                    lineCap = PM.PenLineCap.Flat;
                    break;
                case LineCap.Square:
                    lineCap = PM.PenLineCap.Square;
                    break;
                case LineCap.Round:
                    lineCap = PM.PenLineCap.Round;
                    break;
            }

            if (style.Dashes != null)
            {
                dashStyle = new PM.DashStyle(
                    ShapeStyle.ConvertDashesToDoubleArray(style.Dashes),
                    style.DashOffset);
            }

            var pen = new PM.Pen(
                ToBrush(style.Stroke),
                scale(style.Thickness / State.ZoomX),
                dashStyle, lineCap,
                lineCap, lineCap);

            return pen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private PM.IBrush ToBrush(ArgbColor color)
        {
            return new PM.SolidColorBrush(ToColor(color));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="br"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private static Rect2 CreateRect(XPoint tl, XPoint br, double dx, double dy)
        {
            return Rect2.Create(tl, br, dx, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        private static void DrawLineInternal(
            PM.DrawingContext dc,
            PM.Pen pen,
            bool isStroked,
            ref P.Point p0,
            ref P.Point p1)
        {
            if (isStroked)
            {
                dc.DrawLine(pen, p0, p1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dc"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="offset"></param>
        private static void DrawLineCurve(
            PM.DrawingContext _dc,
            PM.Pen pen,
            bool isStroked,
            ref P.Point pt1,
            ref P.Point pt2,
            double offset,
            CurveOrientation orientation)
        {
            if (isStroked)
            {
                var sg = new PM.StreamGeometry();
                using (var sgc = sg.Open())
                {
                    sgc.BeginFigure(new P.Point(pt1.X, pt1.Y), false);
                    if (orientation == CurveOrientation.Horizontal)
                    {
                        sgc.CubicBezierTo(
                            new P.Point(pt1.X + offset, pt1.Y),
                            new P.Point(pt2.X - offset, pt2.Y),
                            new P.Point(pt2.X, pt2.Y));
                    }
                    else if(orientation == CurveOrientation.Vertical)
                    {
                        sgc.CubicBezierTo(
                            new P.Point(pt1.X, pt1.Y + offset),
                            new P.Point(pt2.X, pt2.Y - offset),
                            new P.Point(pt2.X, pt2.Y));
                    }
                    else
                    {
                        throw new NotSupportedException($"Not supported curve orientation: {orientation}");
                    }
                    sgc.EndFigure(false);
                }
                _dc.DrawGeometry(null, pen, sg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="line"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        private void DrawLineArrowsInternal(
            PM.DrawingContext dc,
            XLine line,
            double dx,
            double dy,
            out P.Point pt1,
            out P.Point pt2)
        {
            PM.IBrush fillStartArrow = ToBrush(line.Style.StartArrowStyle.Fill);
            PM.Pen strokeStartArrow = ToPen(line.Style.StartArrowStyle, _scaleToPage);

            PM.IBrush fillEndArrow = ToBrush(line.Style.EndArrowStyle.Fill);
            PM.Pen strokeEndArrow = ToPen(line.Style.EndArrowStyle, _scaleToPage);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            XLine.SetMaxLength(line, ref _x1, ref _y1, ref _x2, ref _y2);

            float x1 = _scaleToPage(_x1);
            float y1 = _scaleToPage(_y1);
            float x2 = _scaleToPage(_x2);
            float y2 = _scaleToPage(_y2);

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2);
            double a2 = Math.Atan2(y2 - y1, x2 - x1);

            var t1 = MatrixHelper.Rotation(a1, new P.Vector(x1, y1));
            var t2 = MatrixHelper.Rotation(a2, new P.Vector(x2, y2));

            pt1 = default(P.Point);
            pt2 = default(P.Point);
            double radiusX1 = sas.RadiusX;
            double radiusY1 = sas.RadiusY;
            double sizeX1 = 2.0 * radiusX1;
            double sizeY1 = 2.0 * radiusY1;

            switch (sas.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt1 = new P.Point(x1, y1);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt1 = MatrixHelper.TransformPoint(t1, new P.Point(x1 - (float)sizeX1, y1));
                        var rect = new Rect2(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        var d = dc.PushPreTransform(t1);
                        DrawRectangleInternal(dc, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        d.Dispose();
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt1 = MatrixHelper.TransformPoint(t1, new P.Point(x1 - (float)sizeX1, y1));
                        var d = dc.PushPreTransform(t1);
                        var rect = new Rect2(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        DrawEllipseInternal(dc, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        d.Dispose();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        var pts = new P.Point[]
                        {
                            new P.Point(x1, y1),
                            new P.Point(x1 - (float)sizeX1, y1 + (float)sizeY1),
                            new P.Point(x1, y1),
                            new P.Point(x1 - (float)sizeX1, y1 - (float)sizeY1),
                            new P.Point(x1, y1)
                        };
                        pt1 = MatrixHelper.TransformPoint(t1, pts[0]);
                        var p11 = MatrixHelper.TransformPoint(t1, pts[1]);
                        var p21 = MatrixHelper.TransformPoint(t1, pts[2]);
                        var p12 = MatrixHelper.TransformPoint(t1, pts[3]);
                        var p22 = MatrixHelper.TransformPoint(t1, pts[4]);
                        DrawLineInternal(dc, strokeStartArrow, sas.IsStroked, ref p11, ref p21);
                        DrawLineInternal(dc, strokeStartArrow, sas.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            double radiusX2 = eas.RadiusX;
            double radiusY2 = eas.RadiusY;
            double sizeX2 = 2.0 * radiusX2;
            double sizeY2 = 2.0 * radiusY2;

            switch (eas.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt2 = new P.Point(x2, y2);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt2 = MatrixHelper.TransformPoint(t2, new P.Point(x2 - (float)sizeX2, y2));
                        var rect = new Rect2(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        var d = dc.PushPreTransform(t2);
                        DrawRectangleInternal(dc, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        d.Dispose();
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt2 = MatrixHelper.TransformPoint(t2, new P.Point(x2 - (float)sizeX2, y2));
                        var d = dc.PushPreTransform(t2);
                        var rect = new Rect2(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        DrawEllipseInternal(dc, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        d.Dispose();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        var pts = new P.Point[]
                        {
                            new P.Point(x2, y2),
                            new P.Point(x2 - (float)sizeX2, y2 + (float)sizeY2),
                            new P.Point(x2, y2),
                            new P.Point(x2 - (float)sizeX2, y2 - (float)sizeY2),
                            new P.Point(x2, y2)
                        };
                        pt2 = MatrixHelper.TransformPoint(t2, pts[0]);
                        var p11 = MatrixHelper.TransformPoint(t2, pts[1]);
                        var p21 = MatrixHelper.TransformPoint(t2, pts[2]);
                        var p12 = MatrixHelper.TransformPoint(t2, pts[3]);
                        var p22 = MatrixHelper.TransformPoint(t2, pts[4]);
                        DrawLineInternal(dc, strokeEndArrow, eas.IsStroked, ref p11, ref p21);
                        DrawLineInternal(dc, strokeEndArrow, eas.IsStroked, ref p12, ref p22);
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawRectangleInternal(
            PM.DrawingContext dc,
            PM.IBrush brush,
            PM.Pen pen,
            bool isStroked,
            bool isFilled,
            ref Rect2 rect)
        {
            if (!isStroked && !isFilled)
                return;

            var r = new P.Rect(rect.X, rect.Y, rect.Width, rect.Height);

            if (isFilled)
            {
                dc.FillRectangle(brush, r);
            }

            if (isStroked)
            {
                dc.DrawRectangle(pen, r);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawEllipseInternal(
            PM.DrawingContext dc,
            PM.IBrush brush,
            PM.Pen pen,
            bool isStroked,
            bool isFilled,
            ref Rect2 rect)
        {
            if (!isFilled && !isStroked)
                return;

            var r = new P.Rect(rect.X, rect.Y, rect.Width, rect.Height);
            var g = new PM.EllipseGeometry(r);

            dc.DrawGeometry(
                isFilled ? brush : null,
                isStroked ? pen : null,
                g);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="stroke"></param>
        /// <param name="rect"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="cellWidth"></param>
        /// <param name="cellHeight"></param>
        /// <param name="isStroked"></param>
        private void DrawGridInternal(
            PM.DrawingContext dc,
            PM.Pen stroke,
            ref Rect2 rect,
            double offsetX, double offsetY,
            double cellWidth, double cellHeight,
            bool isStroked)
        {
            double ox = rect.X;
            double oy = rect.Y;
            double sx = ox + offsetX;
            double sy = oy + offsetY;
            double ex = ox + rect.Width;
            double ey = oy + rect.Height;

            for (double x = sx; x < ex; x += cellWidth)
            {
                var p0 = new P.Point(
                    _scaleToPage(x),
                    _scaleToPage(oy));
                var p1 = new P.Point(
                    _scaleToPage(x),
                    _scaleToPage(ey));
                DrawLineInternal(dc, stroke, isStroked, ref p0, ref p1);
            }

            for (double y = sy; y < ey; y += cellHeight)
            {
                var p0 = new P.Point(
                    _scaleToPage(ox),
                    _scaleToPage(y));
                var p1 = new P.Point(
                    _scaleToPage(ex),
                    _scaleToPage(y));
                DrawLineInternal(dc, stroke, isStroked, ref p0, ref p1);
            }
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
                _biCache = new Dictionary<string, PMI.Bitmap>();
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _dc = dc as PM.DrawingContext;

            PM.Pen strokeLine = ToPen(line.Style, _scaleToPage);
            P.Point pt1, pt2;

            DrawLineArrowsInternal(_dc, line, dx, dy, out pt1, out pt2);

            if (line.Style.LineStyle.IsCurved)
            {
                DrawLineCurve(_dc, strokeLine, line.IsStroked, ref pt1, ref pt2, line.Style.LineStyle.Curvature, line.Style.LineStyle.CurveOrientation);
            }
            else
            {
                DrawLineInternal(_dc, strokeLine, line.IsStroked, ref pt1, ref pt2);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _dc = dc as PM.DrawingContext;

            PM.IBrush brush = ToBrush(rectangle.Style.Fill);
            PM.Pen pen = ToPen(rectangle.Style, _scaleToPage);

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
        public override void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _dc = dc as PM.DrawingContext;

            PM.IBrush brush = ToBrush(ellipse.Style.Fill);
            PM.Pen pen = ToPen(ellipse.Style, _scaleToPage);

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
        public override void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            if (!arc.IsFilled && !arc.IsStroked)
                return;

            var _dc = dc as PM.DrawingContext;

            PM.IBrush brush = ToBrush(arc.Style.Fill);
            PM.Pen pen = ToPen(arc.Style, _scaleToPage);

            var sg = new PM.StreamGeometry();
            using (var sgc = sg.Open())
            {
                var a = WpfArc.FromXArc(arc, dx, dy);

                sgc.BeginFigure(
                    new P.Point(a.Start.X, a.Start.Y),
                    arc.IsFilled);

                sgc.ArcTo(
                    new P.Point(a.End.X, a.End.Y),
                    new P.Size(a.Radius.Width, a.Radius.Height),
                    0.0,
                    a.IsLargeArc,
                    PM.SweepDirection.Clockwise);

                sgc.EndFigure(false);
            }

            _dc.DrawGeometry(
                arc.IsFilled ? brush : null,
                arc.IsStroked ? pen : null,
                sg);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XCubicBezier cubicBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            if (!cubicBezier.IsFilled && !cubicBezier.IsStroked)
                return;

            var _dc = dc as PM.DrawingContext;

            PM.IBrush brush = ToBrush(cubicBezier.Style.Fill);
            PM.Pen pen = ToPen(cubicBezier.Style, _scaleToPage);

            var sg = new PM.StreamGeometry();
            using (var sgc = sg.Open())
            {
                sgc.BeginFigure(
                    new P.Point(cubicBezier.Point1.X, cubicBezier.Point1.Y),
                    cubicBezier.IsFilled);

                sgc.CubicBezierTo(
                    new P.Point(cubicBezier.Point2.X, cubicBezier.Point2.Y),
                    new P.Point(cubicBezier.Point3.X, cubicBezier.Point3.Y),
                    new P.Point(cubicBezier.Point4.X, cubicBezier.Point4.Y));

                sgc.EndFigure(false);
            }

            _dc.DrawGeometry(
                cubicBezier.IsFilled ? brush : null,
                cubicBezier.IsStroked ? pen : null,
                sg);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XQuadraticBezier quadraticBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            if (!quadraticBezier.IsFilled && !quadraticBezier.IsStroked)
                return;

            var _dc = dc as PM.DrawingContext;

            PM.IBrush brush = ToBrush(quadraticBezier.Style.Fill);
            PM.Pen pen = ToPen(quadraticBezier.Style, _scaleToPage);

            var sg = new PM.StreamGeometry();
            using (var sgc = sg.Open())
            {
                sgc.BeginFigure(
                    new P.Point(quadraticBezier.Point1.X, quadraticBezier.Point1.Y),
                    quadraticBezier.IsFilled);

                sgc.QuadraticBezierTo(
                    new P.Point(quadraticBezier.Point2.X, quadraticBezier.Point2.Y),
                    new P.Point(quadraticBezier.Point3.X, quadraticBezier.Point3.Y));

                sgc.EndFigure(false);
            }

            _dc.DrawGeometry(
                quadraticBezier.IsFilled ? brush : null,
                quadraticBezier.IsStroked ? pen : null,
                sg);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XText text, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _gfx = dc as PM.DrawingContext;

            var tbind = text.BindText(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            PM.IBrush brush = ToBrush(text.Style.Stroke);

            var fontStyle = PM.FontStyle.Normal;
            var fontWeight = PM.FontWeight.Normal;
            //var fontDecoration = PM.FontDecoration.None;

            if (text.Style.TextStyle.FontStyle != null)
            {
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
                {
                    fontStyle |= PM.FontStyle.Italic;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
                {
                    fontWeight |= PM.FontWeight.Bold;
                }

                // TODO: Implement font decoration after Perspex adds support.
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
                var ft = new PM.FormattedText(
                    tbind,
                    text.Style.TextStyle.FontName,
                    text.Style.TextStyle.FontSize * _textScaleFactor,
                    fontStyle,
                    PM.TextAlignment.Left,
                    fontWeight);

                var rect = CreateRect(text.TopLeft, text.BottomRight, dx, dy);
                var size = ft.Measure();
                var origin = GetTextOrigin(text.Style, ref rect, ref size);

                _gfx.DrawText(brush, origin, ft);

                ft.Dispose();
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _dc = dc as PM.DrawingContext;

            var rect = CreateRect(image.TopLeft, image.BottomRight, dx, dy);

            if (image.IsStroked || image.IsFilled)
            {
                PM.IBrush brush = ToBrush(image.Style.Fill);
                PM.Pen pen = ToPen(image.Style, _scaleToPage);

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
                        new P.Rect(0, 0, bi.PixelWidth, bi.PixelHeight),
                        new P.Rect(rect.X, rect.Y, rect.Width, rect.Height));
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
                            var bi = new PMI.Bitmap(ms);

                            if (_enableImageCache)
                                _biCache[image.Key] = bi;

                            _dc.DrawImage(
                                bi,
                                1.0,
                                new P.Rect(0, 0, bi.PixelWidth, bi.PixelHeight),
                                new P.Rect(rect.X, rect.Y, rect.Width, rect.Height));
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
        public override void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            if (!path.IsFilled && !path.IsStroked)
                return;

            var _dc = dc as PM.DrawingContext;

            var g = path.Geometry.ToGeometry();

            var brush = ToBrush(path.Style.Fill);
            var pen = ToPen(path.Style, _scaleToPage);
            _dc.DrawGeometry(
                path.IsFilled ? brush : null,
                path.IsStroked ? pen : null,
                g);
        }
    }
}
