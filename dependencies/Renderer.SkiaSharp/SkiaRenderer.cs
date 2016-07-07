// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Math;
using Core2D.Project;
using Core2D.Renderer;
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
            using (SKCanvas gfx = pdf.BeginPage(width, height))
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
                        gfx,
                        container.Template.Background,
                        Rect2.Create(0, 0, width / scale, height / scale));
                }

                // Draw template contents to pdf graphics.
                Draw(gfx, container.Template, container.Data.Properties, container.Data.Record);

                // Draw page contents to pdf graphics.
                Draw(gfx, container, container.Data.Properties, container.Data.Record);
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
            paint.StrokeWidth = (float)style.Thickness; // TODO: Convert from presentation units.
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

            // TODO: Add dashes lines support: https://github.com/mono/SkiaSharp/issues/47
            /*
            if (style.Dashes != null)
            {
                paint.DashPattern = ShapeStyle.ConvertDashesToDoubleArray(style.Dashes);
            }
            paint.DashOffset = style.DashOffset;
            */

            return paint;
        }

        // TODO: Dispose SKPaint after used.
        private static SKPaint ToSKPaintBrush(ArgbColor color)
        {
            var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.IsStroke = false;
            paint.Color = ToSKColor(color);
            return paint;
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

        private void DrawGridInternal(SKCanvas gfx, SKPaint stroke, ref SKRect rect, double offsetX, double offsetY, double cellWidth, double cellHeight, bool isStroked)
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
                DrawLineInternal(gfx, stroke, isStroked, ref p0, ref p1);
            }

            for (float y = sy; y < ey; y += (float)cellHeight)
            {
                var p0 = new SKPoint(ox, y);
                var p1 = new SKPoint(ex, y);
                DrawLineInternal(gfx, stroke, isStroked, ref p0, ref p1);
            }
        }

        private void DrawBackgroundInternal(SKCanvas canvas, ArgbColor color, Rect2 rect)
        {
            SKPaint brush = ToSKPaintBrush(color);
            SKRect srect = SKRect.Create(
                _scaleToPage(rect.X),
                _scaleToPage(rect.Y),
                _scaleToPage(rect.Width),
                _scaleToPage(rect.Height));
            canvas.DrawRect(srect, brush);
            brush.Dispose();
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
            // TODO: Add support for line arrows and curved lines.

            if (!line.IsStroked)
                return;

            var canvas = dc as SKCanvas;

            SKPaint strokeLine = ToSKPaintPen(line.Style, _scaleToPage);

            SKPaint fillStartArrow = ToSKPaintBrush(line.Style.StartArrowStyle.Fill);
            SKPaint strokeStartArrow = ToSKPaintPen(line.Style.StartArrowStyle, _scaleToPage);

            SKPaint fillEndArrow = ToSKPaintBrush(line.Style.EndArrowStyle.Fill);
            SKPaint strokeEndArrow = ToSKPaintPen(line.Style.EndArrowStyle, _scaleToPage);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            XLineExtensions.GetMaxLength(line, ref _x1, ref _y1, ref _x2, ref _y2);

            float x1 = _scaleToPage(_x1);
            float y1 = _scaleToPage(_y1);
            float x2 = _scaleToPage(_x2);
            float y2 = _scaleToPage(_y2);

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            //double a1 = Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI;
            //double a2 = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;

            //var t1 = new SKMatrix();
            //var c1 = new SKPoint(x1, y1);
            //t1.RotateAtPrepend(a1, c1);

            //var t2 = new SKMatrix();
            //var c2 = new SKPoint(x2, y2);
            //t2.RotateAtPrepend(a2, c2);

            SKPoint pt1 = default(SKPoint);
            SKPoint pt2 = default(SKPoint);

            //double radiusX1 = sas.RadiusX;
            //double radiusY1 = sas.RadiusY;
            //double sizeX1 = 2.0 * radiusX1;
            //double sizeY1 = 2.0 * radiusY1;

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
                        //pt1 = t1.Transform(new SKPoint(x1 - sizeX1, y1));
                        //var rect = new XRect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        //canvas.Save();
                        //canvas.RotateAtTransform(a1, c1);
                        //DrawRectangleInternal(canvas, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        //canvas.Restore();
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        //pt1 = t1.Transform(new SKPoint(x1 - sizeX1, y1));
                        //canvas.Save();
                        //canvas.RotateAtTransform(a1, c1);
                        //var rect = new XRect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        //DrawEllipseInternal(canvas, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                        //canvas.Restore();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        //pt1 = t1.Transform(new SKPoint(x1, y1));
                        //var p11 = t1.Transform(new SKPoint(x1 - sizeX1, y1 + sizeY1));
                        //var p21 = t1.Transform(new SKPoint(x1, y1));
                        //var p12 = t1.Transform(new SKPoint(x1 - sizeX1, y1 - sizeY1));
                        //var p22 = t1.Transform(new SKPoint(x1, y1));
                        //DrawLineInternal(canvas, strokeStartArrow, sas.IsStroked, ref p11, ref p21);
                        //DrawLineInternal(canvas, strokeStartArrow, sas.IsStroked, ref p12, ref p22);
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
                        //pt2 = t2.Transform(new SKPoint(x2 - sizeX2, y2));
                        //var rect = new XRect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        //canvas.Save();
                        //canvas.RotateAtTransform(a2, c2);
                        //DrawRectangleInternal(canvas, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        //canvas.Restore();
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        //pt2 = t2.Transform(new SKPoint(x2 - sizeX2, y2));
                        //canvas.Save();
                        //canvas.RotateAtTransform(a2, c2);
                        //var rect = new XRect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        //DrawEllipseInternal(canvas, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                        //canvas.Restore();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        //pt2 = t2.Transform(new SKPoint(x2, y2));
                        //var p11 = t2.Transform(new SKPoint(x2 - sizeX2, y2 + sizeY2));
                        //var p21 = t2.Transform(new SKPoint(x2, y2));
                        //var p12 = t2.Transform(new SKPoint(x2 - sizeX2, y2 - sizeY2));
                        //var p22 = t2.Transform(new SKPoint(x2, y2));
                        //DrawLineInternal(canvas, strokeEndArrow, eas.IsStroked, ref p11, ref p21);
                        //DrawLineInternal(canvas, strokeEndArrow, eas.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            DrawLineInternal(canvas, strokeLine, line.IsStroked, ref pt1, ref pt2);

            strokeLine.Dispose();

            fillStartArrow.Dispose();
            strokeStartArrow.Dispose();

            fillEndArrow.Dispose();
            strokeEndArrow.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var canvas = dc as SKCanvas;

            SKPaint brush = ToSKPaintBrush(rectangle.Style.Fill);
            SKPaint pen = ToSKPaintPen(rectangle.Style, _scaleToPage);

            var rect = CreateRect(
                rectangle.TopLeft,
                rectangle.BottomRight,
                dx, dy, 
                _scaleToPage);

            DrawRectangleInternal(canvas, brush, pen, rectangle.IsStroked, rectangle.IsFilled, ref rect);

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    canvas,
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
            var canvas = dc as SKCanvas;

            SKPaint brush = ToSKPaintBrush(ellipse.Style.Fill);
            SKPaint pen = ToSKPaintPen(ellipse.Style, _scaleToPage);

            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy,
                _scaleToPage);

            DrawEllipseInternal(canvas, brush, pen, ellipse.IsStroked, ellipse.IsFilled, ref rect);

            brush.Dispose();
            pen.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            /*
            var canvas = dc as SKCanvas;

            var a = Arc.GdiArc.FromXArc(arc, dx, dy);

            if (arc.IsFilled)
            {
                var path = new SKCanvasPath();
                // NOTE: Not implemented in PdfSharp Core version.
                path.AddArc(
                    _scaleToPage(a.X),
                    _scaleToPage(a.Y),
                    _scaleToPage(a.Width),
                    _scaleToPage(a.Height),
                    a.StartAngle,
                    a.SweepAngle);

                if (arc.IsStroked)
                {
                    canvas.DrawPath(
                        ToSKPaintPen(arc.Style, _scaleToPage),
                        ToSKPaintBrush(arc.Style.Fill),
                        path);
                }
                else
                {
                    canvas.DrawPath(
                        ToSKPaintBrush(arc.Style.Fill),
                        path);
                }
            }
            else
            {
                if (arc.IsStroked)
                {
                    canvas.DrawArc(
                        ToSKPaintPen(arc.Style, _scaleToPage),
                        _scaleToPage(a.X),
                        _scaleToPage(a.Y),
                        _scaleToPage(a.Width),
                        _scaleToPage(a.Height),
                        a.StartAngle,
                        a.SweepAngle);
                }
            }
            */
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XCubicBezier cubicBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            /*
            var canvas = dc as SKCanvas;

            if (cubicBezier.IsFilled)
            {
                var path = new SKCanvasPath();
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
                    canvas.DrawPath(
                        ToSKPaintPen(cubicBezier.Style, _scaleToPage),
                        ToSKPaintBrush(cubicBezier.Style.Fill),
                        path);
                }
                else
                {
                    canvas.DrawPath(
                        ToSKPaintBrush(cubicBezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (cubicBezier.IsStroked)
                {
                    canvas.DrawBezier(
                        ToSKPaintPen(cubicBezier.Style, _scaleToPage),
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
            */
        }

        /// <inheritdoc/>
        public override void Draw(object dc, XQuadraticBezier quadraticBezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            /*
            var canvas = dc as SKCanvas;

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
                var path = new SKCanvasPath();
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
                    canvas.DrawPath(
                        ToSKPaintPen(quadraticBezier.Style, _scaleToPage),
                        ToSKPaintBrush(quadraticBezier.Style.Fill),
                        path);
                }
                else
                {
                    canvas.DrawPath(
                        ToSKPaintBrush(quadraticBezier.Style.Fill),
                        path);
                }
            }
            else
            {
                if (quadraticBezier.IsStroked)
                {
                    canvas.DrawBezier(
                        ToSKPaintPen(quadraticBezier.Style, _scaleToPage),
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
            */
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
            /*
            var canvas = dc as SKCanvas;

            var gp = path.Geometry.ToSKCanvasPath(dx, dy, _scaleToPage);

            if (path.IsFilled && path.IsStroked)
            {
                canvas.DrawPath(
                    ToSKPaintPen(path.Style, _scaleToPage),
                    ToSKPaintBrush(path.Style.Fill),
                    gp);
            }
            else if (path.IsFilled && !path.IsStroked)
            {
                canvas.DrawPath(
                    ToSKPaintBrush(path.Style.Fill),
                    gp);
            }
            else if (!path.IsFilled && path.IsStroked)
            {
                canvas.DrawPath(
                    ToSKPaintPen(path.Style, _scaleToPage),
                    gp);
            }
            */
        }
    }
}
