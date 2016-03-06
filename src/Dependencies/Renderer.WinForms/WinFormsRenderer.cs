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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Renderer.WinForms
{
    /// <summary>
    /// Native Windows Forms shape renderer.
    /// </summary>
    public class WinFormsRenderer : ShapeRenderer
    {
        private bool _enableImageCache = true;
        private IDictionary<string, Image> _biCache;

        /// <summary>
        /// 
        /// </summary>
        private Func<double, float> _scaleToPage;

        /// <summary>
        /// 
        /// </summary>
        private double _textScaleFactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormsRenderer"/> class.
        /// </summary>
        /// <param name="textScaleFactor"></param>
        public WinFormsRenderer(double textScaleFactor = 1.0)
        {
            ClearCache(isZooming: false);

            _textScaleFactor = textScaleFactor;
            _scaleToPage = (value) => (float)(value);
        }

        /// <summary>
        /// Creates a new <see cref="WinFormsRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="WinFormsRenderer"/> class.</returns>
        public static ShapeRenderer Create()
        {
            return new WinFormsRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Color ToColor(ArgbColor color)
        {
            return Color.FromArgb(
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
        private Pen ToPen(BaseStyle style, Func<double, float> scale)
        {
            var pen = new Pen(ToColor(style.Stroke), (float)(style.Thickness / State.ZoomX));
            switch (style.LineCap)
            {
                case Core2D.Style.LineCap.Flat:
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                    pen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
                    break;
                case Core2D.Style.LineCap.Square:
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Square;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                    pen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
                    break;
                case Core2D.Style.LineCap.Round:
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
                    break;
            }
            if (style.Dashes != null)
            {
                // TODO: Convert to correct dash values.
                pen.DashPattern = ShapeStyle.ConvertDashesToFloatArray(style.Dashes);
            }
            pen.DashOffset = (float)style.DashOffset;
            return pen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private SolidBrush ToSolidBrush(ArgbColor color)
        {
            return new SolidBrush(ToColor(color));
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
        /// <param name="gfx"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        private static void DrawLineInternal(
            Graphics gfx,
            Pen pen,
            bool isStroked,
            ref PointF p0,
            ref PointF p1)
        {
            if (isStroked)
            {
                gfx.DrawLine(pen, p0, p1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawRectangleInternal(
            Graphics gfx,
            Brush brush,
            Pen pen,
            bool isStroked,
            bool isFilled,
            ref Rect2 rect)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawEllipseInternal(
            Graphics gfx,
            Brush brush,
            Pen pen,
            bool isStroked,
            bool isFilled,
            ref Rect2 rect)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="stroke"></param>
        /// <param name="rect"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="cellWidth"></param>
        /// <param name="cellHeight"></param>
        /// <param name="isStroked"></param>
        private void DrawGridInternal(
            Graphics gfx,
            Pen stroke,
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
                var p0 = new PointF(
                    _scaleToPage(x),
                    _scaleToPage(oy));
                var p1 = new PointF(
                    _scaleToPage(x),
                    _scaleToPage(ey));
                DrawLineInternal(gfx, stroke, isStroked, ref p0, ref p1);
            }

            for (double y = sy; y < ey; y += cellHeight)
            {
                var p0 = new PointF(
                    _scaleToPage(ox),
                    _scaleToPage(y));
                var p1 = new PointF(
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
                _biCache = new Dictionary<string, Image>();
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _gfx = dc as Graphics;

            Brush fillLine = ToSolidBrush(line.Style.Fill);
            Pen strokeLine = ToPen(line.Style, _scaleToPage);

            Brush fillStartArrow = ToSolidBrush(line.Style.StartArrowStyle.Fill);
            Pen strokeStartArrow = ToPen(line.Style.StartArrowStyle, _scaleToPage);

            Brush fillEndArrow = ToSolidBrush(line.Style.EndArrowStyle.Fill);
            Pen strokeEndArrow = ToPen(line.Style.EndArrowStyle, _scaleToPage);

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
            float a1 = (float)(Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI);
            float a2 = (float)(Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI);

            var t1 = new Matrix();
            var c1 = new PointF(x1, y1);
            t1.RotateAt(a1, c1);

            var t2 = new Matrix();
            var c2 = new PointF(x2, y2);
            t2.RotateAt(a2, c2);

            PointF pt1;
            PointF pt2;

            double radiusX1 = sas.RadiusX;
            double radiusY1 = sas.RadiusY;
            double sizeX1 = 2.0 * radiusX1;
            double sizeY1 = 2.0 * radiusY1;

            switch (sas.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt1 = new PointF(x1, y1);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        var pts = new PointF[] { new PointF(x1 - (float)sizeX1, y1) };
                        t1.TransformPoints(pts);
                        pt1 = pts[0];
                        var rect = new Rect2(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        var gs = _gfx.Save();
                        _gfx.MultiplyTransform(t1);
                        DrawRectangleInternal(_gfx, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        _gfx.Restore(gs);
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        var pts = new PointF[] { new PointF(x1 - (float)sizeX1, y1) };
                        t1.TransformPoints(pts);
                        pt1 = pts[0];
                        var gs = _gfx.Save();
                        _gfx.MultiplyTransform(t1);
                        var rect = new Rect2(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        DrawEllipseInternal(_gfx, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        _gfx.Restore(gs);
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        var pts = new PointF[]
                        {
                            new PointF(x1, y1),
                            new PointF(x1 - (float)sizeX1, y1 + (float)sizeY1),
                            new PointF(x1, y1),
                            new PointF(x1 - (float)sizeX1, y1 - (float)sizeY1),
                            new PointF(x1, y1)
                        };
                        t1.TransformPoints(pts);
                        pt1 = pts[0];
                        var p11 = pts[1];
                        var p21 = pts[2];
                        var p12 = pts[3];
                        var p22 = pts[4];
                        DrawLineInternal(_gfx, strokeStartArrow, sas.IsStroked, ref p11, ref p21);
                        DrawLineInternal(_gfx, strokeStartArrow, sas.IsStroked, ref p12, ref p22);
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
                        pt2 = new PointF(x2, y2);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        var pts = new PointF[] { new PointF(x2 - (float)sizeX2, y2) };
                        t2.TransformPoints(pts);
                        pt2 = pts[0];
                        var rect = new Rect2(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        var gs = _gfx.Save();
                        _gfx.MultiplyTransform(t2);
                        DrawRectangleInternal(_gfx, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        _gfx.Restore(gs);
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        var pts = new PointF[] { new PointF(x2 - (float)sizeX2, y2) };
                        t2.TransformPoints(pts);
                        pt2 = pts[0];
                        var gs = _gfx.Save();
                        _gfx.MultiplyTransform(t2);
                        var rect = new Rect2(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        DrawEllipseInternal(_gfx, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        _gfx.Restore(gs);
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        var pts = new PointF[]
                        {
                            new PointF(x2, y2),
                            new PointF(x2 - (float)sizeX2, y2 + (float)sizeY2),
                            new PointF(x2, y2),
                            new PointF(x2 - (float)sizeX2, y2 - (float)sizeY2),
                            new PointF(x2, y2)
                        };
                        t2.TransformPoints(pts);
                        pt2 = pts[0];
                        var p11 = pts[1];
                        var p21 = pts[2];
                        var p12 = pts[3];
                        var p22 = pts[4];
                        DrawLineInternal(_gfx, strokeEndArrow, eas.IsStroked, ref p11, ref p21);
                        DrawLineInternal(_gfx, strokeEndArrow, eas.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            _gfx.DrawLine(strokeLine, pt1, pt2);

            fillLine.Dispose();
            strokeLine.Dispose();

            fillStartArrow.Dispose();
            strokeStartArrow.Dispose();

            fillEndArrow.Dispose();
            strokeEndArrow.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _gfx = dc as Graphics;

            Brush brush = ToSolidBrush(rectangle.Style.Fill);
            Pen pen = ToPen(rectangle.Style, _scaleToPage);

            var rect = CreateRect(
                rectangle.TopLeft,
                rectangle.BottomRight,
                dx, dy);

            if (rectangle.IsFilled)
            {
                _gfx.FillRectangle(
                    brush,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            if (rectangle.IsStroked)
            {
                _gfx.DrawRectangle(
                    pen,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    _gfx,
                    pen,
                    ref rect,
                    rectangle.OffsetX, rectangle.OffsetY,
                    rectangle.CellWidth, rectangle.CellHeight,
                    true);
            }

            brush.Dispose();
            pen.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _gfx = dc as Graphics;

            Brush brush = ToSolidBrush(ellipse.Style.Fill);
            Pen pen = ToPen(ellipse.Style, _scaleToPage);

            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy);

            if (ellipse.IsFilled)
            {
                _gfx.FillEllipse(
                    brush,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            if (ellipse.IsStroked)
            {
                _gfx.DrawEllipse(
                    pen,
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
            }

            brush.Dispose();
            pen.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var a = GdiArc.FromXArc(arc, dx, dy);
            if (a.Width <= 0.0 || a.Height <= 0.0)
                return;

            var _gfx = dc as Graphics;

            Brush brush = ToSolidBrush(arc.Style.Fill);
            Pen pen = ToPen(arc.Style, _scaleToPage);

            if (arc.IsFilled)
            {
                var path = new GraphicsPath();
                path.AddArc(
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    (float)a.StartAngle,
                    (float)a.SweepAngle);
                _gfx.FillPath(brush, path);
            }

            if (arc.IsStroked)
            {
                _gfx.DrawArc(
                    pen,
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    (float)a.StartAngle,
                    (float)a.SweepAngle);
            }

            brush.Dispose();
            pen.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XCubicBezier cubicBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _gfx = dc as Graphics;

            Brush brush = ToSolidBrush(cubicBezier.Style.Fill);
            Pen pen = ToPen(cubicBezier.Style, _scaleToPage);

            if (cubicBezier.IsFilled)
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
                _gfx.FillPath(brush, path);
            }

            if (cubicBezier.IsStroked)
            {
                _gfx.DrawBezier(
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

            brush.Dispose();
            pen.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XQuadraticBezier quadraticBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _gfx = dc as Graphics;

            Brush brush = ToSolidBrush(quadraticBezier.Style.Fill);
            Pen pen = ToPen(quadraticBezier.Style, _scaleToPage);

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
                var path = new GraphicsPath();
                path.AddBezier(
                    _scaleToPage(x1 + dx),
                    _scaleToPage(y1 + dy),
                    _scaleToPage(x2 + dx),
                    _scaleToPage(y2 + dy),
                    _scaleToPage(x3 + dx),
                    _scaleToPage(y3 + dy),
                    _scaleToPage(x4 + dx),
                    _scaleToPage(y4 + dy));
                _gfx.FillPath(brush, path);
            }

            if (quadraticBezier.IsStroked)
            {
                _gfx.DrawBezier(
                    pen,
                    _scaleToPage(x1 + dx),
                    _scaleToPage(y1 + dy),
                    _scaleToPage(x2 + dx),
                    _scaleToPage(y2 + dy),
                    _scaleToPage(x3 + dx),
                    _scaleToPage(y3 + dy),
                    _scaleToPage(x4 + dx),
                    _scaleToPage(y4 + dy));
            }

            brush.Dispose();
            pen.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XText text, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _gfx = dc as Graphics;

            var tbind = text.BindText(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            Brush brush = ToSolidBrush(text.Style.Stroke);

            var fontStyle = System.Drawing.FontStyle.Regular;
            if (text.Style.TextStyle.FontStyle != null)
            {
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Bold))
                {
                    fontStyle |= System.Drawing.FontStyle.Bold;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Italic))
                {
                    fontStyle |= System.Drawing.FontStyle.Italic;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Underline))
                {
                    fontStyle |= System.Drawing.FontStyle.Underline;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(Core2D.Style.FontStyleFlags.Strikeout))
                {
                    fontStyle |= System.Drawing.FontStyle.Strikeout;
                }
            }

            Font font = new Font(
                text.Style.TextStyle.FontName,
                (float)(text.Style.TextStyle.FontSize * _textScaleFactor),
                fontStyle);

            var rect = CreateRect(
                text.TopLeft,
                text.BottomRight,
                dx, dy);

            var srect = new RectangleF(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            var format = new StringFormat();
            switch (text.Style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    format.Alignment = StringAlignment.Near;
                    break;
                case TextHAlignment.Center:
                    format.Alignment = StringAlignment.Center;
                    break;
                case TextHAlignment.Right:
                    format.Alignment = StringAlignment.Far;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case TextVAlignment.Center:
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case TextVAlignment.Bottom:
                    format.LineAlignment = StringAlignment.Far;
                    break;
            }

            format.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
            format.Trimming = StringTrimming.None;

            _gfx.DrawString(
                tbind,
                font,
                ToSolidBrush(text.Style.Stroke),
                srect,
                format);

            brush.Dispose();
            font.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _gfx = dc as Graphics;

            Brush brush = ToSolidBrush(image.Style.Stroke);

            var rect = CreateRect(
                image.TopLeft,
                image.BottomRight,
                dx, dy);

            var srect = new RectangleF(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            if (image.IsFilled)
            {
                _gfx.FillRectangle(
                    ToSolidBrush(image.Style.Fill),
                    srect);
            }

            if (image.IsStroked)
            {
                _gfx.DrawRectangle(
                    ToPen(image.Style, _scaleToPage),
                    srect.X,
                    srect.Y,
                    srect.Width,
                    srect.Height);
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
                    var bi = Image.FromStream(ms);
                    ms.Dispose();

                    if (_enableImageCache)
                        _biCache[image.Key] = bi;

                    _gfx.DrawImage(bi, srect);

                    if (!_enableImageCache)
                        bi.Dispose();
                }
            }

            brush.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _gfx = dc as Graphics;

            var gp = path.Geometry.ToGraphicsPath(dx, dy, _scaleToPage);

            if (path.IsFilled && path.IsStroked)
            {
                var brush = ToSolidBrush(path.Style.Fill);
                var pen = ToPen(path.Style, _scaleToPage);
                _gfx.FillPath(
                    brush,
                    gp);
                _gfx.DrawPath(
                    pen,
                    gp);
                brush.Dispose();
                pen.Dispose();
            }
            else if (path.IsFilled && !path.IsStroked)
            {
                var brush = ToSolidBrush(path.Style.Fill);
                _gfx.FillPath(
                    brush,
                    gp);
                brush.Dispose();
            }
            else if (!path.IsFilled && path.IsStroked)
            {
                var pen = ToPen(path.Style, _scaleToPage);
                _gfx.DrawPath(
                    pen,
                    gp);
                pen.Dispose();
            }
        }
    }
}
