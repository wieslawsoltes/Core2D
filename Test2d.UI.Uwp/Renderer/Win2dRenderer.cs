// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using N = System.Numerics;
using Test2d;

namespace Test.Uwp
{
    public class Win2dRenderer : ObservableObject, IRenderer
    {
        private bool _enableImageCache = true;
        private IDictionary<Uri, CanvasBitmap> _biCache;
        private RendererState _state = new RendererState();

        public RendererState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        public Win2dRenderer()
        {
            ClearCache(isZooming: false);
        }

        public static IRenderer Create()
        {
            return new Win2dRenderer();
        }

        private Color ToColor(ArgbColor color)
        {
            return Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

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
                ss.CustomDashStyle = style.Dashes.Select(x => (float)x).ToArray();
            }
            ss.DashOffset = (float)style.DashOffset;
            return ss;
        }

        private static Rect2 CreateRect(XPoint tl, XPoint br, double dx, double dy)
        {
            return Rect2.Create(tl, br, dx, dy);
        }

        private static void DrawLineInternal(
            CanvasDrawingSession ds,
            Color pen,
            CanvasStrokeStyle ss,
            bool isStroked,
            ref N.Vector2 p0,
            ref N.Vector2 p1,
            double strokeWidth)
        {
            if (isStroked)
            {
                ds.DrawLine(p0, p1, isStroked ? pen : Colors.Transparent, (float)strokeWidth, ss);
            }
        }

        private static void DrawRectangleInternal(
            CanvasDrawingSession ds,
            Color brush,
            Color pen,
            CanvasStrokeStyle ss,
            bool isStroked,
            bool isFilled,
            ref Rect2 rect,
            double strokeWidth)
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

        private static void DrawEllipseInternal(
            CanvasDrawingSession ds,
            Color brush,
            Color pen,
            CanvasStrokeStyle ss,
            bool isStroked,
            bool isFilled,
            ref Rect2 rect,
            double strokeWidth)
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

        private void DrawGridInternal(
            CanvasDrawingSession ds,
            Color stroke,
            CanvasStrokeStyle ss,
            ref Rect2 rect,
            double offsetX, double offsetY,
            double cellWidth, double cellHeight,
            bool isStroked,
            double strokeWidth)
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

        public void CacheImage(Uri path, CanvasBitmap bi)
        {
            if (_enableImageCache
                && !_biCache.ContainsKey(path))
            {
                _biCache[path] = bi;
            }
        }

        public void ClearCache(bool isZooming)
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
                _biCache = new Dictionary<Uri, CanvasBitmap>();
            }
        }

        public void Draw(object ds, Container container, ImmutableArray<ShapeProperty> db, Record r)
        {
            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(ds, layer, db, r);
                }
            }
        }

        public void Draw(object ds, Layer layer, ImmutableArray<ShapeProperty> db, Record r)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(_state.DrawShapeState))
                {
                    shape.Draw(ds, this, 0, 0, db, r);
                }
            }
        }

        public void Draw(object ds, XLine line, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            // TODO:
            var _ds = ds as CanvasDrawingSession;

            double thicknessLine = line.Style.Thickness / _state.Zoom;
            double thicknessStartArrow = line.Style.StartArrowStyle.Thickness / _state.Zoom;
            double thicknessEndArrow = line.Style.EndArrowStyle.Thickness / _state.Zoom;

            var fillLine = ToColor(line.Style.Fill);
            var strokeLine = ToColor(line.Style.Stroke);

            var fillStartArrow = ToColor(line.Style.StartArrowStyle.Fill);
            var strokeStartArrow = ToColor(line.Style.StartArrowStyle.Stroke);

            var fillEndArrow = ToColor(line.Style.EndArrowStyle.Fill);
            var strokeEndArrow = ToColor(line.Style.EndArrowStyle.Stroke);

            var ssLine = CreateStrokeStyle(line.Style);
            var ssStartArrow = CreateStrokeStyle(line.Style.StartArrowStyle);
            var ssEndArrow = CreateStrokeStyle(line.Style.EndArrowStyle);

            double _x1 = line.Start.X + dx;
            double _y1 = line.Start.Y + dy;
            double _x2 = line.End.X + dx;
            double _y2 = line.End.Y + dy;

            XLine.SetMaxLength(line, ref _x1, ref _y1, ref _x2, ref _y2);

            float x1 = (float)_x1;
            float y1 = (float)_y1;
            float x2 = (float)_x2;
            float y2 = (float)_y2;

            var sas = line.Style.StartArrowStyle;
            var eas = line.Style.EndArrowStyle;
            float a1 = (float)Math.Atan2(y1 - y2, x1 - x2);
            float a2 = (float)Math.Atan2(y2 - y1, x2 - x1);

            var t1 = N.Matrix3x2.CreateRotation(a1, new N.Vector2(x1, y1));
            var t2 = N.Matrix3x2.CreateRotation(a2, new N.Vector2(x2, y2));

            N.Vector2 pt1;
            N.Vector2 pt2;

            double radiusX1 = sas.RadiusX;
            double radiusY1 = sas.RadiusY;
            double sizeX1 = 2.0 * radiusX1;
            double sizeY1 = 2.0 * radiusY1;

            switch (sas.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        pt1 = new N.Vector2(x1, y1);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt1 = N.Vector2.Transform(new N.Vector2(x1 - (float)sizeX1, y1), t1);
                        var rect = new Rect2(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        var old = _ds.Transform;
                        _ds.Transform = t1;
                        DrawRectangleInternal(_ds, fillStartArrow, strokeStartArrow, ssStartArrow, sas.IsStroked, sas.IsFilled, ref rect, thicknessStartArrow);
                        _ds.Transform = old;
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt1 = N.Vector2.Transform(new N.Vector2(x1 - (float)sizeX1, y1), t1);
                        var old = _ds.Transform;
                        _ds.Transform = t1;
                        var rect = new Rect2(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        DrawEllipseInternal(_ds, fillStartArrow, strokeStartArrow, ssStartArrow, sas.IsStroked, sas.IsFilled, ref rect, thicknessStartArrow);
                        _ds.Transform = old;
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        pt1 = N.Vector2.Transform(new N.Vector2(x1, y1), t1);
                        var p11 = N.Vector2.Transform(new N.Vector2(x1 - (float)sizeX1, y1 + (float)sizeY1), t1);
                        var p21 = N.Vector2.Transform(new N.Vector2(x1, y1), t1);
                        var p12 = N.Vector2.Transform(new N.Vector2(x1 - (float)sizeX1, y1 - (float)sizeY1), t1);
                        var p22 = N.Vector2.Transform(new N.Vector2(x1, y1), t1);
                        DrawLineInternal(_ds, strokeStartArrow, ssStartArrow, sas.IsStroked, ref p11, ref p21, thicknessStartArrow);
                        DrawLineInternal(_ds, strokeStartArrow, ssStartArrow, sas.IsStroked, ref p12, ref p22, thicknessStartArrow);
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
                        pt2 = new N.Vector2(x2, y2);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt2 = N.Vector2.Transform(new N.Vector2(x2 - (float)sizeX2, y2), t2);
                        var rect = new Rect2(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        var old = _ds.Transform;
                        _ds.Transform = t1;
                        DrawRectangleInternal(_ds, fillEndArrow, strokeEndArrow, ssEndArrow, eas.IsStroked, eas.IsFilled, ref rect, thicknessEndArrow);
                        _ds.Transform = old;
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt2 = N.Vector2.Transform(new N.Vector2(x2 - (float)sizeX2, y2), t2);
                        var old = _ds.Transform;
                        _ds.Transform = t1;
                        var rect = new Rect2(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        DrawEllipseInternal(_ds, fillEndArrow, strokeEndArrow, ssEndArrow, eas.IsStroked, eas.IsFilled, ref rect, thicknessEndArrow);
                        _ds.Transform = old;
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        pt2 = N.Vector2.Transform(new N.Vector2(x2, y2), t2);
                        var p11 = N.Vector2.Transform(new N.Vector2(x2 - (float)sizeX2, y2 + (float)sizeY2), t2);
                        var p21 = N.Vector2.Transform(new N.Vector2(x2, y2), t2);
                        var p12 = N.Vector2.Transform(new N.Vector2(x2 - (float)sizeX2, y2 - (float)sizeY2), t2);
                        var p22 = N.Vector2.Transform(new N.Vector2(x2, y2), t2);
                        DrawLineInternal(_ds, strokeEndArrow, ssEndArrow, eas.IsStroked, ref p11, ref p21, thicknessEndArrow);
                        DrawLineInternal(_ds, strokeEndArrow, ssEndArrow, eas.IsStroked, ref p12, ref p22, thicknessEndArrow);
                    }
                    break;
            }

            DrawLineInternal(_ds, strokeLine, ssLine, line.IsStroked, ref pt1, ref pt2, thicknessLine);

            ssEndArrow.Dispose();
            ssStartArrow.Dispose();
            ssLine.Dispose();
        }

        public void Draw(object ds, XRectangle rectangle, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = rectangle.Style.Thickness / _state.Zoom;
            var brush = ToColor(rectangle.Style.Fill);
            var pen = ToColor(rectangle.Style.Stroke);
            var ss = CreateStrokeStyle(rectangle.Style);

            var rect = CreateRect(
                rectangle.TopLeft,
                rectangle.BottomRight,
                dx, dy);

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

        public void Draw(object ds, XEllipse ellipse, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = ellipse.Style.Thickness / _state.Zoom;
            var brush = ToColor(ellipse.Style.Fill);
            var pen = ToColor(ellipse.Style.Stroke);
            var ss = CreateStrokeStyle(ellipse.Style);

            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy);

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

        public void Draw(object ds, XArc arc, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var a = WpfArc.FromXArc(arc, dx, dy);

            var _ds = ds as CanvasDrawingSession;

            double thickness = arc.Style.Thickness / _state.Zoom;
            var brush = ToColor(arc.Style.Fill);
            var pen = ToColor(arc.Style.Stroke);
            var ss = CreateStrokeStyle(arc.Style);

            CanvasGeometry g;
            using (var builder = new CanvasPathBuilder(_ds))
            {
                builder.BeginFigure((float)a.Start.X, (float)a.Start.Y);
                builder.AddArc(
                    new N.Vector2(
                        (float)a.End.X, 
                        (float)a.End.Y),
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

        public void Draw(object ds, XBezier bezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = bezier.Style.Thickness / _state.Zoom;
            var brush = ToColor(bezier.Style.Fill);
            var pen = ToColor(bezier.Style.Stroke);
            var ss = CreateStrokeStyle(bezier.Style);

            CanvasGeometry g;
            using (var builder = new CanvasPathBuilder(_ds))
            {
                builder.BeginFigure((float)bezier.Point1.X, (float)bezier.Point1.Y);
                builder.AddCubicBezier(
                    new N.Vector2(
                        (float)bezier.Point2.X,
                        (float)bezier.Point2.Y),
                    new N.Vector2(
                        (float)bezier.Point3.X,
                        (float)bezier.Point3.Y),
                    new N.Vector2(
                        (float)bezier.Point4.X,
                        (float)bezier.Point4.Y));
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

        public void Draw(object ds, XQBezier qbezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = qbezier.Style.Thickness / _state.Zoom;
            var brush = ToColor(qbezier.Style.Fill);
            var pen = ToColor(qbezier.Style.Stroke);
            var ss = CreateStrokeStyle(qbezier.Style);

            CanvasGeometry g;
            using (var builder = new CanvasPathBuilder(_ds))
            {
                builder.BeginFigure((float)qbezier.Point1.X, (float)qbezier.Point1.Y);
                builder.AddQuadraticBezier(
                    new N.Vector2(
                        (float)qbezier.Point2.X,
                        (float)qbezier.Point2.Y),
                    new N.Vector2(
                        (float)qbezier.Point3.X,
                        (float)qbezier.Point3.Y));
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

        public void Draw(object ds, XText text, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _ds = ds as CanvasDrawingSession;

            var tbind = text.BindToTextProperty(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            var brush = ToColor(text.Style.Stroke);

            var fontWeight = FontWeights.Normal;
            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Bold))
            {
                fontWeight = FontWeights.Bold;
            }

            var fontStyle = Windows.UI.Text.FontStyle.Normal;
            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Italic))
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

            var rect = Rect2.Create(text.TopLeft, text.BottomRight, dx, dy);

            var layout = new CanvasTextLayout(_ds, tbind, format, (float)rect.Width, (float)rect.Height);

            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Underline))
            {
                layout.SetUnderline(0, tbind.Length, true);
            }

            if (text.Style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Strikeout))
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

        public void Draw(object ds, XImage image, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _ds = ds as CanvasDrawingSession;

            var rect = CreateRect(
                image.TopLeft,
                image.BottomRight,
                dx, dy);

            if (image.IsFilled || image.IsStroked)
            {
                double thickness = image.Style.Thickness / _state.Zoom;
                var brush = ToColor(image.Style.Fill);
                var pen = ToColor(image.Style.Stroke);
                var ss = CreateStrokeStyle(image.Style);

                DrawRectangleInternal(_ds, brush, pen, ss, image.IsStroked, image.IsFilled, ref rect, thickness);

                ss.Dispose();
            }

            var srect = new Rect(rect.X, rect.Y, rect.Width, rect.Height);

            if (_enableImageCache
                && _biCache.ContainsKey(image.Path))
            {
                _ds.DrawImage(_biCache[image.Path], srect);
            }
            else
            {
                if (!image.Path.IsAbsoluteUri /*|| !System.IO.File.Exists(image.Path.LocalPath)*/)
                    return;

                // TODO: CanvasBitmap.LoadAsync throws access denied.
                var bi = CanvasBitmap.LoadAsync(_ds, image.Path.LocalPath).GetResults();

                if (_enableImageCache)
                    _biCache[image.Path] = bi;

                _ds.DrawImage(bi, srect);

                if (!_enableImageCache)
                    bi.Dispose();
            }
        }

        public void Draw(object ds, XPath path, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _ds = ds as CanvasDrawingSession;

            double thickness = path.Style.Thickness / _state.Zoom;
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
