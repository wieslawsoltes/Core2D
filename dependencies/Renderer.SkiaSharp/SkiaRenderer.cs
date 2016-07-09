// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Math;
using Core2D.Math.Arc;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Renderer.SkiaSharp
{
    /// <summary>
    /// Native SkiaSharp shape renderer.
    /// </summary>
    public class SkiaRenderer : ShapeRenderer
    {
        //private bool _enableImageCache = true;
        private IDictionary<string, SKImage> _biCache;
        private Func<double, float> _scaleToPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaRenderer"/> class.
        /// </summary>
        public SkiaRenderer()
        {
            ClearCache(isZooming: false);

            _scaleToPage = (value) => (float)(value * 1.0);
        }

        /// <summary>
        /// Creates a new <see cref="SkiaRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="SkiaRenderer"/> class.</returns>
        public static ShapeRenderer Create()
        {
            return new SkiaRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="container"></param>
        public void Save(string path, XContainer container)
        {
            using (var stream = new SKFileWStream(path))
            {
                using (var pdf = SKDocument.CreatePdf(stream))
                {
                    Add(pdf, container);
                    pdf.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="document"></param>
        public void Save(string path, XDocument document)
        {
            using (var stream = new SKFileWStream(path))
            {
                using (var pdf = SKDocument.CreatePdf(stream))
                {
                    foreach (var container in document.Pages)
                    {
                        Add(pdf, container);
                    }

                    pdf.Close();
                    ClearCache(isZooming: false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="project"></param>
        public void Save(string path, XProject project)
        {
            using (var stream = new SKFileWStream(path))
            {
                using (var pdf = SKDocument.CreatePdf(stream))
                {
                    foreach (var document in project.Documents)
                    {
                        foreach (var container in document.Pages)
                        {
                            Add(pdf, container);
                        }
                    }

                    pdf.Close();
                    ClearCache(isZooming: false);
                }
            }
        }

        private void Add(SKDocument pdf, XContainer container)
        {
            float width = (float)container.Template.Width;
            float height = (float)container.Template.Height;
            using (SKCanvas canvas = pdf.BeginPage(width, height))
            {
                // Calculate x and y page scale factors.
                double scaleX = width / container.Template.Width;
                double scaleY = height / container.Template.Height;
                double scale = Math.Min(scaleX, scaleY);

                // Set scaling function.
                _scaleToPage = (value) => (float)(value * scale);

                // Draw container template contents to pdf graphics.
                if (container.Template.Background.A > 0)
                {
                    DrawBackgroundInternal(
                        canvas,
                        container.Template.Background,
                        Rect2.Create(0, 0, width / scale, height / scale));
                }

                // Draw template contents to pdf graphics.
                Draw(canvas, container.Template, container.Data.Properties, container.Data.Record);

                // Draw page contents to pdf graphics.
                Draw(canvas, container, container.Data.Properties, container.Data.Record);
            }
        }

        private static SKColor ToSKColor(ArgbColor color)
        {
            return new SKColor(
                color.R,
                color.G,
                color.B,
                color.A);
        }

        private static SKPaint ToSKPaintPen(BaseStyle style, Func<double, float> scale)
        {
            var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.IsStroke = true;
            paint.StrokeWidth = (float)(style.Thickness * 72.0 / 96.0);
            paint.Color = ToSKColor(style.Stroke);

            switch (style.LineCap)
            {
                case LineCap.Flat:
                    paint.StrokeCap = SKStrokeCap.Butt;
                    break;
                case LineCap.Square:
                    paint.StrokeCap = SKStrokeCap.Square;
                    break;
                case LineCap.Round:
                    paint.StrokeCap = SKStrokeCap.Round;
                    break;
            }

            // TODO: Add dashed line support: https://github.com/mono/SkiaSharp/issues/47
            /*
            if (style.Dashes != null)
            {
                paint.DashPattern = ShapeStyle.ConvertDashesToDoubleArray(style.Dashes);
            }
            paint.DashOffset = style.DashOffset;
            */

            return paint;
        }

        private static SKPaint ToSKPaintBrush(ArgbColor color)
        {
            var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.IsStroke = false;
            paint.Color = ToSKColor(color);
            return paint;
        }

        private static SKRect ToSKRect(double x, double y, double width, double height)
        {
            float left = (float)x;
            float top = (float)y;
            float right = (float)(x + width);
            float bottom = (float)(y + height);
            return new SKRect(left, top, right, bottom);
        }

        private static SKRect CreateRect(XPoint tl, XPoint br, double dx, double dy, Func<double, float> scale)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            return new SKRect(
                scale(tlx + dx),
                scale(tly + dy),
                scale(brx + dx),
                scale(bry + dy));
        }

        private static void DrawLineInternal(SKCanvas canvas, SKPaint pen, bool isStroked, ref SKPoint p0, ref SKPoint p1)
        {
            if (isStroked)
            {
                canvas.DrawLine(p0.X, p0.Y, p1.X, p1.Y, pen);
            }
        }

        private static void DrawLineCurveInternal(SKCanvas canvas, SKPaint pen, bool isStroked, ref SKPoint pt1, ref SKPoint pt2, double curvature, CurveOrientation orientation, PointAlignment pt1a, PointAlignment pt2a)
        {
            if (isStroked)
            {
                using (var path = new SKPath())
                {
                    path.MoveTo(pt1.X, pt1.Y);
                    double p1x = pt1.X;
                    double p1y = pt1.Y;
                    double p2x = pt2.X;
                    double p2y = pt2.Y;
                    XLineExtensions.GetCurvedLineBezierControlPoints(orientation, curvature, pt1a, pt2a, ref p1x, ref p1y, ref p2x, ref p2y);
                    path.CubicTo(
                        (float)p1x, 
                        (float)p1y,
                        (float)p2x, 
                        (float)p2y,
                        pt2.X, pt2.Y);
                    canvas.DrawPath(path, pen);
                }
            }
        }

        private void DrawLineArrowsInternal(SKCanvas canvas, XLine line, double dx, double dy, out SKPoint pt1, out SKPoint pt2)
        {
            using (SKPaint fillStartArrow = ToSKPaintBrush(line.Style.StartArrowStyle.Fill))
            using (SKPaint strokeStartArrow = ToSKPaintPen(line.Style.StartArrowStyle, _scaleToPage))
            using (SKPaint fillEndArrow = ToSKPaintBrush(line.Style.EndArrowStyle.Fill))
            using (SKPaint strokeEndArrow = ToSKPaintPen(line.Style.EndArrowStyle, _scaleToPage))
            {
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

                var t1 = MatrixHelper.Rotation(a1, new SKPoint(x1, y1));
                var t2 = MatrixHelper.Rotation(a2, new SKPoint(x2, y2));

                pt1 = default(SKPoint);
                pt2 = default(SKPoint);
                double radiusX1 = sas.RadiusX;
                double radiusY1 = sas.RadiusY;
                double sizeX1 = 2.0 * radiusX1;
                double sizeY1 = 2.0 * radiusY1;

                switch (sas.ArrowType)
                {
                    default:
                    case ArrowType.None:
                        {
                            pt1 = new SKPoint(x1, y1);
                        }
                        break;
                    case ArrowType.Rectangle:
                        {
                            pt1 = MatrixHelper.TransformPoint(t1, new SKPoint(x1 - (float)sizeX1, y1));
                            var rect = ToSKRect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                            int count = canvas.Save();
                            canvas.SetMatrix(t1);
                            DrawRectangleInternal(canvas, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                            canvas.RestoreToCount(count);
                        }
                        break;
                    case ArrowType.Ellipse:
                        {
                            pt1 = MatrixHelper.TransformPoint(t1, new SKPoint(x1 - (float)sizeX1, y1));
                            int count = canvas.Save();
                            canvas.SetMatrix(t1);
                            var rect = ToSKRect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                            DrawEllipseInternal(canvas, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                            canvas.RestoreToCount(count);
                        }
                        break;
                    case ArrowType.Arrow:
                        {
                            var pts = new SKPoint[]
                            {
                            new SKPoint(x1, y1),
                            new SKPoint(x1 - (float)sizeX1, y1 + (float)sizeY1),
                            new SKPoint(x1, y1),
                            new SKPoint(x1 - (float)sizeX1, y1 - (float)sizeY1),
                            new SKPoint(x1, y1)
                            };
                            pt1 = MatrixHelper.TransformPoint(t1, pts[0]);
                            var p11 = MatrixHelper.TransformPoint(t1, pts[1]);
                            var p21 = MatrixHelper.TransformPoint(t1, pts[2]);
                            var p12 = MatrixHelper.TransformPoint(t1, pts[3]);
                            var p22 = MatrixHelper.TransformPoint(t1, pts[4]);
                            DrawLineInternal(canvas, strokeStartArrow, sas.IsStroked, ref p11, ref p21);
                            DrawLineInternal(canvas, strokeStartArrow, sas.IsStroked, ref p12, ref p22);
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
                            pt2 = new SKPoint(x2, y2);
                        }
                        break;
                    case ArrowType.Rectangle:
                        {
                            pt2 = MatrixHelper.TransformPoint(t2, new SKPoint(x2 - (float)sizeX2, y2));
                            var rect = ToSKRect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                            int count = canvas.Save();
                            canvas.SetMatrix(t2);
                            DrawRectangleInternal(canvas, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                            canvas.RestoreToCount(count);
                        }
                        break;
                    case ArrowType.Ellipse:
                        {
                            pt2 = MatrixHelper.TransformPoint(t2, new SKPoint(x2 - (float)sizeX2, y2));
                            int count = canvas.Save();
                            canvas.SetMatrix(t2);
                            var rect = ToSKRect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                            DrawEllipseInternal(canvas, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                            canvas.RestoreToCount(count);
                        }
                        break;
                    case ArrowType.Arrow:
                        {
                            var pts = new SKPoint[]
                            {
                            new SKPoint(x2, y2),
                            new SKPoint(x2 - (float)sizeX2, y2 + (float)sizeY2),
                            new SKPoint(x2, y2),
                            new SKPoint(x2 - (float)sizeX2, y2 - (float)sizeY2),
                            new SKPoint(x2, y2)
                            };
                            pt2 = MatrixHelper.TransformPoint(t2, pts[0]);
                            var p11 = MatrixHelper.TransformPoint(t2, pts[1]);
                            var p21 = MatrixHelper.TransformPoint(t2, pts[2]);
                            var p12 = MatrixHelper.TransformPoint(t2, pts[3]);
                            var p22 = MatrixHelper.TransformPoint(t2, pts[4]);
                            DrawLineInternal(canvas, strokeEndArrow, eas.IsStroked, ref p11, ref p21);
                            DrawLineInternal(canvas, strokeEndArrow, eas.IsStroked, ref p12, ref p22);
                        }
                        break;
                }
            }
        }

        private static void DrawRectangleInternal(SKCanvas canvas, SKPaint brush, SKPaint pen, bool isStroked, bool isFilled, ref SKRect rect)
        {
            if (isFilled)
            {
                canvas.DrawRect(rect, brush);
            }

            if (isStroked)
            {
                canvas.DrawRect(rect, pen);
            }
        }

        private static void DrawEllipseInternal(SKCanvas canvas, SKPaint brush, SKPaint pen, bool isStroked, bool isFilled, ref SKRect rect)
        {
            if (isFilled)
            {
                canvas.DrawOval(rect, brush);
            }

            if (isStroked)
            {
                canvas.DrawOval(rect, pen);
            }
        }

        private static void DrawPathInternal(SKCanvas canvas, SKPaint brush, SKPaint pen, bool isStroked, bool isFilled, SKPath path)
        {
            if (isFilled)
            {
                canvas.DrawPath(path, brush);
            }

            if (isStroked)
            {
                canvas.DrawPath(path, pen);
            }
        }

        private void DrawGridInternal(SKCanvas canvas, SKPaint stroke, ref SKRect rect, double offsetX, double offsetY, double cellWidth, double cellHeight, bool isStroked)
        {
            float ox = rect.Left;
            float oy = rect.Top;
            float sx = (float)(ox + offsetX);
            float sy = (float)(oy + offsetY);
            float ex = ox + (rect.Right - rect.Left);
            float ey = oy + (rect.Bottom - rect.Top);

            for (float x = sx; x < ex; x += (float)cellWidth)
            {
                var p0 = new SKPoint(x, oy);
                var p1 = new SKPoint(x, ey);
                DrawLineInternal(canvas, stroke, isStroked, ref p0, ref p1);
            }

            for (float y = sy; y < ey; y += (float)cellHeight)
            {
                var p0 = new SKPoint(ox, y);
                var p1 = new SKPoint(ex, y);
                DrawLineInternal(canvas, stroke, isStroked, ref p0, ref p1);
            }
        }

        private void DrawBackgroundInternal(SKCanvas canvas, ArgbColor color, Rect2 rect)
        {
            using (SKPaint brush = ToSKPaintBrush(color))
            {
                SKRect srect = SKRect.Create(
                    _scaleToPage(rect.X),
                    _scaleToPage(rect.Y),
                    _scaleToPage(rect.Width),
                    _scaleToPage(rect.Height));
                canvas.DrawRect(srect, brush);
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
                _biCache = new Dictionary<string, SKImage>();
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var canvas = dc as SKCanvas;

            using (SKPaint strokeLine = ToSKPaintPen(line.Style, _scaleToPage))
            {
                SKPoint pt1, pt2;

                DrawLineArrowsInternal(canvas, line, dx, dy, out pt1, out pt2);

                if (line.Style.LineStyle.IsCurved)
                {
                    DrawLineCurveInternal(
                        canvas,
                        strokeLine, line.IsStroked,
                        ref pt1, ref pt2,
                        line.Style.LineStyle.Curvature,
                        line.Style.LineStyle.CurveOrientation,
                        line.Start.Alignment,
                        line.End.Alignment);
                }
                else
                {
                    DrawLineInternal(canvas, strokeLine, line.IsStroked, ref pt1, ref pt2);
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var canvas = dc as SKCanvas;

            using (SKPaint brush = ToSKPaintBrush(rectangle.Style.Fill))
            using (SKPaint pen = ToSKPaintPen(rectangle.Style, _scaleToPage))
            {
                var rect = CreateRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy, _scaleToPage);
                DrawRectangleInternal(canvas, brush, pen, rectangle.IsStroked, rectangle.IsFilled, ref rect);
                if (rectangle.IsGrid)
                {
                    DrawGridInternal(
                        canvas,
                        pen,
                        ref rect,
                        rectangle.OffsetX,
                        rectangle.OffsetY,
                        rectangle.CellWidth,
                        rectangle.CellHeight,
                        isStroked: true);
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var canvas = dc as SKCanvas;

            using (SKPaint brush = ToSKPaintBrush(ellipse.Style.Fill))
            using (SKPaint pen = ToSKPaintPen(ellipse.Style, _scaleToPage))
            {
                var rect = CreateRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy, _scaleToPage);
                DrawEllipseInternal(canvas, brush, pen, ellipse.IsStroked, ellipse.IsFilled, ref rect);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var canvas = dc as SKCanvas;

            using (SKPaint brush = ToSKPaintBrush(arc.Style.Fill))
            using (SKPaint pen = ToSKPaintPen(arc.Style, _scaleToPage))
            using (var path = new SKPath())
            {
                var a = GdiArc.FromXArc(arc, dx, dy);
                var rect = new SKRect(
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.X + a.Width),
                    _scaleToPage(a.Y + a.Height));
                path.AddArc(rect, (float)a.StartAngle, (float)a.SweepAngle);
                DrawPathInternal(canvas, brush, pen, arc.IsStroked, arc.IsFilled, path);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XCubicBezier cubicBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var canvas = dc as SKCanvas;

            using (SKPaint brush = ToSKPaintBrush(cubicBezier.Style.Fill))
            using (SKPaint pen = ToSKPaintPen(cubicBezier.Style, _scaleToPage))
            using (var path = new SKPath())
            {
                path.MoveTo(
                    _scaleToPage(cubicBezier.Point1.X + dx),
                    _scaleToPage(cubicBezier.Point1.Y + dy));
                path.CubicTo(
                    _scaleToPage(cubicBezier.Point2.X + dx),
                    _scaleToPage(cubicBezier.Point2.Y + dy),
                    _scaleToPage(cubicBezier.Point3.X + dx),
                    _scaleToPage(cubicBezier.Point3.Y + dy),
                    _scaleToPage(cubicBezier.Point4.X + dx),
                    _scaleToPage(cubicBezier.Point4.Y + dy));
                DrawPathInternal(canvas, brush, pen, cubicBezier.IsStroked, cubicBezier.IsFilled, path);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XQuadraticBezier quadraticBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var canvas = dc as SKCanvas;

            using (SKPaint brush = ToSKPaintBrush(quadraticBezier.Style.Fill))
            using (SKPaint pen = ToSKPaintPen(quadraticBezier.Style, _scaleToPage))
            using (var path = new SKPath())
            {
                path.MoveTo(
                    _scaleToPage(quadraticBezier.Point1.X + dx),
                    _scaleToPage(quadraticBezier.Point1.Y + dy));
                path.QuadTo(
                    _scaleToPage(quadraticBezier.Point2.X + dx),
                    _scaleToPage(quadraticBezier.Point2.Y + dy),
                    _scaleToPage(quadraticBezier.Point3.X + dx),
                    _scaleToPage(quadraticBezier.Point3.Y + dy));
                DrawPathInternal(canvas, brush, pen, quadraticBezier.IsStroked, quadraticBezier.IsFilled, path);
            }
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XText text, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            /*
            var canvas = dc as SKCanvas;

            var tbind = text.BindText(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            var options = new XPdfFontOptions(PdfFontEncoding.Unicode);

            var fontStyle = XFontStyle.Regular;
            if (text.Style.TextStyle.FontStyle != null)
            {
                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
                {
                    fontStyle |= XFontStyle.Bold;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
                {
                    fontStyle |= XFontStyle.Italic;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Underline))
                {
                    fontStyle |= XFontStyle.Underline;
                }

                if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Strikeout))
                {
                    fontStyle |= XFontStyle.Strikeout;
                }
            }

            var font = new XFont(
                text.Style.TextStyle.FontName,
                _scaleToPage(text.Style.TextStyle.FontSize),
                fontStyle,
                options);

            var rect = Rect2.Create(
                text.TopLeft,
                text.BottomRight,
                dx, dy);

            var srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            var format = new XStringFormat();
            switch (text.Style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    format.Alignment = XStringAlignment.Near;
                    break;
                case TextHAlignment.Center:
                    format.Alignment = XStringAlignment.Center;
                    break;
                case TextHAlignment.Right:
                    format.Alignment = XStringAlignment.Far;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    format.LineAlignment = XLineAlignment.Near;
                    break;
                case TextVAlignment.Center:
                    format.LineAlignment = XLineAlignment.Center;
                    break;
                case TextVAlignment.Bottom:
                    format.LineAlignment = XLineAlignment.Far;
                    break;
            }

            canvas.DrawString(
                tbind,
                font,
                ToSKPaintBrush(text.Style.Stroke),
                srect,
                format);
            */
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            /*
            var canvas = dc as SKCanvas;

            var rect = Rect2.Create(
                image.TopLeft,
                image.BottomRight,
                dx, dy);

            var srect = new XRect(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));

            if (image.IsStroked && image.IsFilled)
            {
                canvas.DrawRectangle(
                    ToSKPaintPen(image.Style, _scaleToPage),
                    ToSKPaintBrush(image.Style.Fill),
                    srect);
            }
            else if (image.IsStroked && !image.IsFilled)
            {
                canvas.DrawRectangle(
                    ToSKPaintPen(image.Style, _scaleToPage),
                    srect);
            }
            else if (!image.IsStroked && image.IsFilled)
            {
                canvas.DrawRectangle(
                    ToSKPaintBrush(image.Style.Fill),
                    srect);
            }

            if (_enableImageCache
                && _biCache.ContainsKey(image.Key))
            {
                canvas.DrawImage(_biCache[image.Key], srect);
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

                    canvas.DrawImage(bi, srect);

                    if (!_enableImageCache)
                        bi.Dispose();
                }
            }
            */
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var canvas = dc as SKCanvas;

            using (SKPaint brush = ToSKPaintBrush(path.Style.Fill))
            using (SKPaint pen = ToSKPaintPen(path.Style, _scaleToPage))
            using (var spath = path.Geometry.ToSKPath(dx, dy, _scaleToPage))
            {
                DrawPathInternal(canvas, brush, pen, path.IsStroked, path.IsFilled, spath);
            }
        }
    }
}
