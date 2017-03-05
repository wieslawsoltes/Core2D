// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Spatial;
using Core2D.Spatial.Arc;
using Core2D.Style;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using N = System.Numerics;

namespace Renderer.Win2D
{
    /// <summary>
    /// Native Universal Windows Platform Win2D shape renderer.
    /// </summary>
    public class Win2dRenderer : ShapeRenderer
    {
        private IDictionary<string, CanvasBitmap> _biCache;
        private ShapeRendererState _state = new ShapeRendererState();

        /// <inheritdoc/>
        public override ShapeRendererState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Win2dRenderer"/> class.
        /// </summary>
        public Win2dRenderer()
        {
            ClearCache(isZooming: false);
        }

        /// <summary>
        /// Creates a new <see cref="Win2dRenderer"/> instance.
        /// </summary>
        /// <returns>The new instance of the <see cref="Win2dRenderer"/> class.</returns>
        public static ShapeRenderer Create() => new Win2dRenderer();

        private Color ToColor(ArgbColor c) => Color.FromArgb(c.A, c.R, c.G, c.B);

        private static Rect2 CreateRect(XPoint tl, XPoint br, double dx, double dy) => Rect2.FromPoints(tl.X, tl.Y, br.X, br.Y, dx, dy);

        private static CanvasStrokeStyle CreateStrokeStyle(BaseStyle style)
        {
            var ss = new CanvasStrokeStyle();
            switch (style.LineCap)
            {
                case LineCap.Flat:
                    ss.StartCap = CanvasCapStyle.Flat;
                    ss.EndCap = CanvasCapStyle.Flat;
                    ss.DashCap = CanvasCapStyle.Flat;
                    break;
                case LineCap.Square:
                    ss.StartCap = CanvasCapStyle.Square;
                    ss.EndCap = CanvasCapStyle.Square;
                    ss.DashCap = CanvasCapStyle.Square;
                    break;
                case LineCap.Round:
                    ss.StartCap = CanvasCapStyle.Round;
                    ss.EndCap = CanvasCapStyle.Round;
                    ss.DashCap = CanvasCapStyle.Round;
                    break;
            }
            if (style.Dashes != null)
            {
                // TODO: Convert to correct dash values.
                ss.CustomDashStyle = ShapeStyle.ConvertDashesToFloatArray(style.Dashes);
            }
            ss.DashOffset = (float)style.DashOffset;
            return ss;
        }

        private static void DrawLineInternal(CanvasDrawingSession ds, Color pen, CanvasStrokeStyle ss, bool isStroked, ref N.Vector2 p0, ref N.Vector2 p1, double strokeWidth)
        {
            if (isStroked)
            {
                ds.DrawLine(p0, p1, isStroked ? pen : Windows.UI.Colors.Transparent, (float)strokeWidth, ss);
            }
        }

        private static void DrawLineCurveInternal(CanvasDrawingSession ds, Color pen, bool isStroked, ref N.Vector2 pt1, ref N.Vector2 pt2, double curvature, CurveOrientation orientation, PointAlignment pt1a, PointAlignment pt2a, CanvasStrokeStyle stroke, double thickness)
        {
            if (isStroked)
            {
                CanvasGeometry g;
                using (var builder = new CanvasPathBuilder(ds))
                {
                    builder.BeginFigure(pt1.X, pt1.Y);
                    double p1x = pt1.X;
                    double p1y = pt1.Y;
                    double p2x = pt2.X;
                    double p2y = pt2.Y;
                    XLineExtensions.GetCurvedLineBezierControlPoints(orientation, curvature, pt1a, pt2a, ref p1x, ref p1y, ref p2x, ref p2y);
                    builder.AddCubicBezier(
                        new N.Vector2(
                            (float)p1x,
                            (float)p1y),
                        new N.Vector2(
                            (float)p2x,
                            (float)p2y),
                        new N.Vector2(
                            pt2.X,
                            pt2.Y));
                    builder.EndFigure(CanvasFigureLoop.Open);
                    g = CanvasGeometry.CreatePath(builder);
                }
                ds.DrawGeometry(g, pen, (float)thickness, stroke);
                g.Dispose();
            }
        }

        private void DrawLineArrowsInternal(CanvasDrawingSession ds, XLine line, double dx, double dy, out N.Vector2 pt1, out N.Vector2 pt2)
        {
            double thicknessStartArrow = line.Style.StartArrowStyle.Thickness / _state.ZoomX;
            double thicknessEndArrow = line.Style.EndArrowStyle.Thickness / _state.ZoomX;

            var fillStartArrow = ToColor(line.Style.StartArrowStyle.Fill);
            var strokeStartArrow = ToColor(line.Style.StartArrowStyle.Stroke);

            var fillEndArrow = ToColor(line.Style.EndArrowStyle.Fill);
            var strokeEndArrow = ToColor(line.Style.EndArrowStyle.Stroke);

            var ssStartArrow = CreateStrokeStyle(line.Style.StartArrowStyle);
            var ssEndArrow = CreateStrokeStyle(line.Style.EndArrowStyle);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            line.GetMaxLength(ref _x1, ref _y1, ref _x2, ref _y2);

            float x1 = (float)_x1;
            float y1 = (float)_y1;
            float x2 = (float)_x2;
            float y2 = (float)_y2;

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            float a1 = (float)Math.Atan2(y1 - y2, x1 - x2);
            float a2 = (float)Math.Atan2(y2 - y1, x2 - x1);

            // Draw start arrow.
            pt1 = DrawLineArrowInternal(ds, strokeStartArrow, fillStartArrow, ssStartArrow, thicknessStartArrow, x1, y1, a1, sas);

            // Draw end arrow.
            pt2 = DrawLineArrowInternal(ds, strokeEndArrow, fillEndArrow, ssEndArrow, thicknessEndArrow, x2, y2, a2, eas);

            ssEndArrow.Dispose();
            ssStartArrow.Dispose();
        }

        private static N.Vector2 DrawLineArrowInternal(CanvasDrawingSession ds, Color pen, Color brush, CanvasStrokeStyle stroke, double thickness, float x, float y, float angle, ArrowStyle style)
        {
            N.Vector2 pt = default(N.Vector2);
            var rt = N.Matrix3x2.CreateRotation(angle, new N.Vector2(x, y));
            double rx = style.RadiusX;
            double ry = style.RadiusY;
            double sx = 2.0 * rx;
            double sy = 2.0 * ry;

            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt = new N.Vector2(x, y);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt = N.Vector2.Transform(new N.Vector2(x - (float)sx, y), rt);
                        var rect = new Rect2(x - sx, y - ry, sx, sy);
                        var old = ds.Transform;
                        ds.Transform = rt * ds.Transform;
                        DrawRectangleInternal(ds, brush, pen, stroke, style.IsStroked, style.IsFilled, ref rect, thickness);
                        ds.Transform = old;
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt = N.Vector2.Transform(new N.Vector2(x - (float)sx, y), rt);
                        var old = ds.Transform;
                        ds.Transform = rt * ds.Transform;
                        var rect = new Rect2(x - sx, y - ry, sx, sy);
                        DrawEllipseInternal(ds, brush, pen, stroke, style.IsStroked, style.IsFilled, ref rect, thickness);
                        ds.Transform = old;
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        pt = N.Vector2.Transform(new N.Vector2(x, y), rt);
                        var p11 = N.Vector2.Transform(new N.Vector2(x - (float)sx, y + (float)sy), rt);
                        var p21 = N.Vector2.Transform(new N.Vector2(x, y), rt);
                        var p12 = N.Vector2.Transform(new N.Vector2(x - (float)sx, y - (float)sy), rt);
                        var p22 = N.Vector2.Transform(new N.Vector2(x, y), rt);
                        DrawLineInternal(ds, pen, stroke, style.IsStroked, ref p11, ref p21, thickness);
                        DrawLineInternal(ds, pen, stroke, style.IsStroked, ref p12, ref p22, thickness);
                    }
                    break;
            }

            return pt;
        }

        private static void DrawRectangleInternal(CanvasDrawingSession ds, Color brush, Color pen, CanvasStrokeStyle ss, bool isStroked, bool isFilled, ref Rect2 rect, double strokeWidth)
        {
            if (isFilled)
            {
                ds.FillRectangle(
                    (float)rect.X,
                    (float)rect.Y,
                    (float)rect.Width,
                    (float)rect.Height,
                    brush);
            }

            if (isStroked)
            {
                ds.DrawRectangle(
                    (float)rect.X,
                    (float)rect.Y,
                    (float)rect.Width,
                    (float)rect.Height,
                    pen,
                    (float)strokeWidth,
                    ss);
            }
        }

        private static void DrawEllipseInternal(CanvasDrawingSession ds, Color brush, Color pen, CanvasStrokeStyle ss, bool isStroked, bool isFilled, ref Rect2 rect, double strokeWidth)
        {
            double radiusX = rect.Width / 2.0;
            double radiusY = rect.Height / 2.0;
            double x = rect.X + radiusX;
            double y = rect.Y + radiusY;

            if (isFilled)
            {
                ds.FillEllipse(
                    (float)x,
                    (float)y,
                    (float)radiusX,
                    (float)radiusY,
                    brush);
            }

            if (isStroked)
            {
                ds.DrawEllipse(
                    (float)x,
                    (float)y,
                    (float)radiusX,
                    (float)radiusY,
                    pen,
                    (float)strokeWidth,
                    ss);
            }
        }

        private void DrawGridInternal(CanvasDrawingSession ds, Color stroke, CanvasStrokeStyle ss, ref Rect2 rect, double offsetX, double offsetY, double cellWidth, double cellHeight, bool isStroked, double strokeWidth)
        {
            double ox = rect.X;
            double oy = rect.Y;
            double sx = ox + offsetX;
            double sy = oy + offsetY;
            double ex = ox + rect.Width;
            double ey = oy + rect.Height;

            for (double x = sx; x < ex; x += cellWidth)
            {
                var p0 = new N.Vector2((float)x, (float)oy);
                var p1 = new N.Vector2((float)x, (float)ey);
                DrawLineInternal(ds, stroke, ss, isStroked, ref p0, ref p1, strokeWidth);
            }

            for (double y = sy; y < ey; y += cellHeight)
            {
                var p0 = new N.Vector2((float)ox, (float)y);
                var p1 = new N.Vector2((float)ex, (float)y);
                DrawLineInternal(ds, stroke, ss, isStroked, ref p0, ref p1, strokeWidth);
            }
        }

        /// <summary>
        /// Caches the image bitmap.
        /// </summary>
        /// <param name="key">The image key.</param>
        /// <param name="bi">The image bitmap.</param>
        public void CacheImage(string key, CanvasBitmap bi)
        {
            if (!_biCache.ContainsKey(key))
            {
                _biCache[key] = bi;
            }
        }

        /// <summary>
        /// Caches the image.
        /// </summary>
        /// <param name="key">The image key.</param>
        /// <param name="canvas">The canvas control.</param>
        /// <returns>The empty task instance.</returns>
        public async Task CacheImage(string key, CanvasControl canvas)
        {
            var bytes = _state.ImageCache.GetImage(key);
            if (bytes != null)
            {
                using (var ms = new MemoryStream(bytes))
                {
                    using (var ras = ms.AsRandomAccessStream())
                    {
                        var bi = await CanvasBitmap.LoadAsync(canvas, ras);
                        CacheImage(key, bi);
                        ras.Dispose();
                    }
                }
            }
        }

        private N.Matrix3x2 ToMatrix3x2(MatrixObject matrix)
        {
            return new N.Matrix3x2(
                (float)matrix.M11, (float)matrix.M12,
                (float)matrix.M21, (float)matrix.M22,
                (float)matrix.OffsetX, (float)matrix.OffsetY);
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
                _biCache = new Dictionary<string, CanvasBitmap>();
            }
        }

        /// <inheritdoc/>
        public override void Fill(object ds, double x, double y, double width, double height, ArgbColor color)
        {
            var _ds = ds as CanvasDrawingSession;
            var brush = ToColor(color);
            _ds.FillRectangle(
                (float)x,
                (float)y,
                (float)width,
                (float)height,
                brush);
        }

        /// <inheritdoc/>
        public override object PushMatrix(object ds, MatrixObject matrix)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void PopMatrix(object dc, object state)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Draw(object ds, XLine line, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thicknessLine = line.Style.Thickness / _state.ZoomX;

            var fillLine = ToColor(line.Style.Fill);
            var strokeLine = ToColor(line.Style.Stroke);
            var ssLine = CreateStrokeStyle(line.Style);

            N.Vector2 pt1, pt2;

            DrawLineArrowsInternal(_ds, line, dx, dy, out pt1, out pt2);

            if (line.Style.LineStyle.IsCurved)
            {
                DrawLineCurveInternal(
                    _ds,
                    strokeLine, line.IsStroked,
                    ref pt1, ref pt2,
                    line.Style.LineStyle.Curvature,
                    line.Style.LineStyle.CurveOrientation,
                    line.Start.Alignment,
                    line.End.Alignment,
                    ssLine, thicknessLine);
            }
            else
            {
                DrawLineInternal(_ds, strokeLine, ssLine, line.IsStroked, ref pt1, ref pt2, thicknessLine);
            }

            ssLine.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object ds, XRectangle rectangle, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = rectangle.Style.Thickness / _state.ZoomX;
            var brush = ToColor(rectangle.Style.Fill);
            var pen = ToColor(rectangle.Style.Stroke);
            var ss = CreateStrokeStyle(rectangle.Style);
            var rect = CreateRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);

            DrawRectangleInternal(
                _ds,
                brush,
                pen,
                ss,
                rectangle.IsStroked,
                rectangle.IsFilled,
                ref rect,
                thickness);

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    _ds,
                    pen,
                    ss,
                    ref rect,
                    rectangle.OffsetX, rectangle.OffsetY,
                    rectangle.CellWidth, rectangle.CellHeight,
                    true,
                    thickness);
            }

            ss.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object ds, XEllipse ellipse, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = ellipse.Style.Thickness / _state.ZoomX;
            var brush = ToColor(ellipse.Style.Fill);
            var pen = ToColor(ellipse.Style.Stroke);
            var ss = CreateStrokeStyle(ellipse.Style);
            var rect = CreateRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);

            DrawEllipseInternal(
                _ds,
                brush,
                pen,
                ss,
                ellipse.IsStroked,
                ellipse.IsFilled,
                ref rect,
                thickness);

            ss.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object ds, XArc arc, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var a = new WpfArc(
                Point2.FromXY(arc.Point1.X, arc.Point1.Y),
                Point2.FromXY(arc.Point2.X, arc.Point2.Y),
                Point2.FromXY(arc.Point3.X, arc.Point3.Y),
                Point2.FromXY(arc.Point4.X, arc.Point4.Y));

            var _ds = ds as CanvasDrawingSession;

            double thickness = arc.Style.Thickness / _state.ZoomX;
            var brush = ToColor(arc.Style.Fill);
            var pen = ToColor(arc.Style.Stroke);
            var ss = CreateStrokeStyle(arc.Style);

            CanvasGeometry g;
            using (var builder = new CanvasPathBuilder(_ds))
            {
                builder.BeginFigure((float)(a.Start.X + dx), (float)(a.Start.Y + dy));
                builder.AddArc(
                    new N.Vector2(
                        (float)(a.End.X + dx),
                        (float)(a.End.Y + dy)),
                    (float)a.Radius.Width,
                    (float)a.Radius.Height,
                    0f,
                    CanvasSweepDirection.Clockwise,
                    a.IsLargeArc ? CanvasArcSize.Large : CanvasArcSize.Small);
                builder.EndFigure(CanvasFigureLoop.Open);
                g = CanvasGeometry.CreatePath(builder);
            }

            if (arc.IsFilled)
            {
                _ds.FillGeometry(g, brush);
            }

            if (arc.IsStroked)
            {
                _ds.DrawGeometry(g, pen, (float)thickness, ss);
            }

            g.Dispose();
            ss.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object ds, XCubicBezier bezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = bezier.Style.Thickness / _state.ZoomX;
            var brush = ToColor(bezier.Style.Fill);
            var pen = ToColor(bezier.Style.Stroke);
            var ss = CreateStrokeStyle(bezier.Style);

            CanvasGeometry g;
            using (var builder = new CanvasPathBuilder(_ds))
            {
                builder.BeginFigure((float)(bezier.Point1.X + dx), (float)(bezier.Point1.Y + dy));
                builder.AddCubicBezier(
                    new N.Vector2(
                        (float)(bezier.Point2.X + dx),
                        (float)(bezier.Point2.Y + dy)),
                    new N.Vector2(
                        (float)(bezier.Point3.X + dx),
                        (float)(bezier.Point3.Y + dy)),
                    new N.Vector2(
                        (float)(bezier.Point4.X + dx),
                        (float)(bezier.Point4.Y + dy)));
                builder.EndFigure(CanvasFigureLoop.Open);
                g = CanvasGeometry.CreatePath(builder);
            }

            if (bezier.IsFilled)
            {
                _ds.FillGeometry(g, brush);
            }

            if (bezier.IsStroked)
            {
                _ds.DrawGeometry(g, pen, (float)thickness, ss);
            }

            g.Dispose();
            ss.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object ds, XQuadraticBezier qbezier, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = qbezier.Style.Thickness / _state.ZoomX;
            var brush = ToColor(qbezier.Style.Fill);
            var pen = ToColor(qbezier.Style.Stroke);
            var ss = CreateStrokeStyle(qbezier.Style);

            CanvasGeometry g;
            using (var builder = new CanvasPathBuilder(_ds))
            {
                builder.BeginFigure((float)(qbezier.Point1.X + dx), (float)(qbezier.Point1.Y + dy));
                builder.AddQuadraticBezier(
                    new N.Vector2(
                        (float)(qbezier.Point2.X + dx),
                        (float)(qbezier.Point2.Y + dy)),
                    new N.Vector2(
                        (float)(qbezier.Point3.X + dx),
                        (float)(qbezier.Point3.Y + dy)));
                builder.EndFigure(CanvasFigureLoop.Open);
                g = CanvasGeometry.CreatePath(builder);
            }

            if (qbezier.IsFilled)
            {
                _ds.FillGeometry(g, brush);
            }

            if (qbezier.IsStroked)
            {
                _ds.DrawGeometry(g, pen, (float)thickness, ss);
            }

            g.Dispose();
            ss.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object ds, XText text, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _ds = ds as CanvasDrawingSession;

            var tbind = text.BindText(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            var brush = ToColor(text.Style.Stroke);

            var fontWeight = FontWeights.Normal;
            if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Bold))
            {
                fontWeight = FontWeights.Bold;
            }

            var fontStyle = Windows.UI.Text.FontStyle.Normal;
            if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Italic))
            {
                fontStyle = Windows.UI.Text.FontStyle.Italic;
            }

            var format = new CanvasTextFormat()
            {
                FontFamily = text.Style.TextStyle.FontName,
                FontWeight = fontWeight,
                FontStyle = fontStyle,
                FontSize = (float)text.Style.TextStyle.FontSize,
                WordWrapping = CanvasWordWrapping.NoWrap
            };

            var rect = Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y,
                dx, dy);

            var layout = new CanvasTextLayout(_ds, tbind, format, (float)rect.Width, (float)rect.Height);

            if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Underline))
            {
                layout.SetUnderline(0, tbind.Length, true);
            }

            if (text.Style.TextStyle.FontStyle.Flags.HasFlag(FontStyleFlags.Strikeout))
            {
                layout.SetStrikethrough(0, tbind.Length, true);
            }

            switch (text.Style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    layout.HorizontalAlignment = CanvasHorizontalAlignment.Left;
                    break;
                case TextHAlignment.Center:
                    layout.HorizontalAlignment = CanvasHorizontalAlignment.Center;
                    break;
                case TextHAlignment.Right:
                    layout.HorizontalAlignment = CanvasHorizontalAlignment.Right;
                    break;
            }

            switch (text.Style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    layout.VerticalAlignment = CanvasVerticalAlignment.Top;
                    break;
                case TextVAlignment.Center:
                    layout.VerticalAlignment = CanvasVerticalAlignment.Center;
                    break;
                case TextVAlignment.Bottom:
                    layout.VerticalAlignment = CanvasVerticalAlignment.Bottom;
                    break;
            }

            _ds.DrawTextLayout(
                layout,
                new N.Vector2(
                    (float)rect.X,
                    (float)rect.Y),
                brush);

            layout.Dispose();
            format.Dispose();
        }

        /// <inheritdoc/>
        public override void Draw(object ds, XImage image, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _ds = ds as CanvasDrawingSession;

            var rect = CreateRect(image.TopLeft, image.BottomRight, dx, dy);

            if (image.IsFilled || image.IsStroked)
            {
                double thickness = image.Style.Thickness / _state.ZoomX;
                var brush = ToColor(image.Style.Fill);
                var pen = ToColor(image.Style.Stroke);
                var ss = CreateStrokeStyle(image.Style);

                DrawRectangleInternal(_ds, brush, pen, ss, image.IsStroked, image.IsFilled, ref rect, thickness);

                ss.Dispose();
            }

            var srect = new Rect(rect.X, rect.Y, rect.Width, rect.Height);

            if (_biCache.ContainsKey(image.Key))
            {
                _ds.DrawImage(_biCache[image.Key], srect);
            }
            else
            {
                // Image caching is done in MainPage because calls to GetResults() will throw exception.
                //Debug.WriteLine($"Bitmap cache does not contain key: {image.Key}");
            }
        }

        /// <inheritdoc/>
        public override void Draw(object ds, XPath path, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = path.Style.Thickness / _state.ZoomX;
            var brush = ToColor(path.Style.Fill);
            var pen = ToColor(path.Style.Stroke);
            var ss = CreateStrokeStyle(path.Style);

            var g = path.Geometry.ToCanvasGeometry(_ds);

            if (path.IsFilled)
            {
                _ds.FillGeometry(g, (float)dx, (float)dy, brush);
            }

            if (path.IsStroked)
            {
                _ds.DrawGeometry(g, (float)dx, (float)dy, pen, (float)thickness, ss);
            }

            g.Dispose();
            ss.Dispose();
        }
    }
}
