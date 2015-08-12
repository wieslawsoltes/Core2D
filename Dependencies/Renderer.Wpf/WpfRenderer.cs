// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Test2d;

namespace Test
{
    /// <summary>
    /// 
    /// </summary>
    public class WpfRenderer : ObservableObject, IRenderer
    {
        private const bool _enableGuidelines = true;
        private bool _enableStyleCache = true;
        private bool _enableArrowStyleCache = true;
        private bool _enableArcCache = true;
        private bool _enableBezierCache = true;
        private bool _enableQBezierCache = true;
        private bool _enableTextCache = true;
        private bool _enableImageCache = true;
        // TODO: Enable XPath caching. Cache is disabled to enable PathHelper to work.
        private bool _enablePathCache = false;
        private IDictionary<ShapeStyle, Tuple<Brush, Pen>> _styleCache;
        private IDictionary<ArrowStyle, Tuple<Brush, Pen>> _arrowStyleCache;
        private IDictionary<XArc, PathGeometry> _arcCache;
        private IDictionary<XBezier, PathGeometry> _bezierCache;
        private IDictionary<XQBezier, PathGeometry> _qbezierCache;
        private IDictionary<XText, Tuple<string, FormattedText, ShapeStyle>> _textCache;
        private IDictionary<Uri, BitmapImage> _biCache;
        private IDictionary<XPath, Tuple<XPathGeometry, StreamGeometry, ShapeStyle>> _pathCache;
        private RendererState _state = new RendererState();

        /// <summary>
        /// 
        /// </summary>
        public RendererState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public WpfRenderer()
        {
            ClearCache(isZooming: false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IRenderer Create()
        {
            return new WpfRenderer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="rect"></param>
        /// <param name="ft"></param>
        /// <returns></returns>
        private static Point GetTextOrigin(ShapeStyle style, ref Rect rect, FormattedText ft)
        {
            double ox, oy;

            switch (style.TextStyle.TextHAlignment)
            {
                case TextHAlignment.Left:
                    ox = rect.TopLeft.X;
                    break;
                case TextHAlignment.Right:
                    ox = rect.Right - ft.Width;
                    break;
                case TextHAlignment.Center:
                default:
                    ox = (rect.Left + rect.Width / 2.0) - (ft.Width / 2.0);
                    break;
            }

            switch (style.TextStyle.TextVAlignment)
            {
                case TextVAlignment.Top:
                    oy = rect.TopLeft.Y;
                    break;
                case TextVAlignment.Bottom:
                    oy = rect.Bottom - ft.Height;
                    break;
                case TextVAlignment.Center:
                default:
                    oy = (rect.Bottom - rect.Height / 2.0) - (ft.Height / 2.0);
                    break;
            }

            return new Point(ox, oy);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Brush CreateBrush(ArgbColor color)
        {
            var brush = new SolidColorBrush(
                Color.FromArgb(
                    color.A,
                    color.R,
                    color.G,
                    color.B));
            brush.Freeze();
            return brush;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        private static Pen CreatePen(BaseStyle style, double thickness)
        {
            var brush = CreateBrush(style.Stroke);
            var pen = new Pen(brush, thickness);
            switch (style.LineCap)
            {
                case LineCap.Flat:
                    pen.StartLineCap = PenLineCap.Flat;
                    pen.EndLineCap = PenLineCap.Flat;
                    pen.DashCap = PenLineCap.Flat;
                    break;
                case LineCap.Square:
                    pen.StartLineCap = PenLineCap.Square;
                    pen.EndLineCap = PenLineCap.Square;
                    pen.DashCap = PenLineCap.Square;
                    break;
                case LineCap.Round:
                    pen.StartLineCap = PenLineCap.Round;
                    pen.EndLineCap = PenLineCap.Round;
                    pen.DashCap = PenLineCap.Round;
                    break;
            }
            pen.DashStyle = new DashStyle(style.Dashes, style.DashOffset);
            pen.DashStyle.Offset = style.DashOffset;
            pen.Freeze();
            return pen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="br"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private static Rect CreateRect(XPoint tl, XPoint br, double dx, double dy)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            return new Rect(
                new Point(tlx + dx, tly + dy),
                new Point(brx + dx, bry + dy));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="half"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        private static void DrawLineInternal(
            DrawingContext dc, 
            double half, 
            Pen pen, 
            bool isStroked,
            ref Point p0, 
            ref Point p1)
        {
            if (!isStroked)
                return;

            if (_enableGuidelines)
            {
                var gs = new GuidelineSet(
                    new double[] { p0.X + half, p1.X + half },
                    new double[] { p0.Y + half, p1.Y + half });
                dc.PushGuidelineSet(gs);
            }

            dc.DrawLine(isStroked ? pen : null, p0, p1);

            if (_enableGuidelines)
                dc.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="half"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="rect"></param>
        private static void DrawRectangleInternal(
            DrawingContext dc, 
            double half, 
            Brush brush, 
            Pen pen, 
            bool isStroked,
            bool isFilled, 
            ref Rect rect)
        {
            if (!isStroked && !isFilled)
                return;

            if (_enableGuidelines)
            {
                var gs = new GuidelineSet(
                    new double[] 
                        { 
                            rect.TopLeft.X + half, 
                            rect.BottomRight.X + half 
                        },
                    new double[] 
                        { 
                            rect.TopLeft.Y + half,
                            rect.BottomRight.Y + half
                        });
                dc.PushGuidelineSet(gs);
            }
            
            dc.DrawRectangle(
                isFilled ? brush : null, 
                isStroked ? pen : null, 
                rect);

            if (_enableGuidelines)
                dc.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="half"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="center"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        private static void DrawEllipseInternal(
            DrawingContext dc, 
            double half, 
            Brush brush, 
            Pen pen, 
            bool isStroked,
            bool isFilled, 
            ref Point center,
            double rx, double ry)
        {
            if (!isStroked && !isFilled)
                return;

            if (_enableGuidelines)
            {
                var gs = new GuidelineSet(
                    new double[] 
                        { 
                            center.X - rx + half, 
                            center.X + rx + half 
                        },
                    new double[] 
                        { 
                            center.Y - ry + half,
                            center.Y + ry + half
                        });
                dc.PushGuidelineSet(gs);
            }

            dc.DrawEllipse(
                isFilled ? brush : null,
                isStroked ? pen : null,
                center,
                rx, ry);

            if (_enableGuidelines)
                dc.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="half"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="pg"></param>
        private static void DrawPathGeometryInternal(
            DrawingContext dc,
            double half,
            Brush brush,
            Pen pen,
            bool isStroked,
            bool isFilled,
            PathGeometry pg)
        {
            if (!isStroked && !isFilled)
                return;

            if (_enableGuidelines)
            {
                var gs = new GuidelineSet(
                    new double[] 
                        { 
                            pg.Bounds.TopLeft.X + half, 
                            pg.Bounds.BottomRight.X + half 
                        },
                    new double[] 
                        { 
                            pg.Bounds.TopLeft.Y + half,
                            pg.Bounds.BottomRight.Y + half
                        });
                dc.PushGuidelineSet(gs);
            }

            dc.DrawGeometry(isFilled ? brush : null, isStroked ? pen : null, pg);

            if (_enableGuidelines)
                dc.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="half"></param>
        /// <param name="stroke"></param>
        /// <param name="rect"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="cellWidth"></param>
        /// <param name="cellHeight"></param>
        /// <param name="isStroked"></param>
        private static void DrawGridInternal(
            DrawingContext dc,
            double half,
            Pen stroke,
            ref Rect rect,
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
                var p0 = new Point(x, oy);
                var p1 = new Point(x, ey);
                DrawLineInternal(dc, half, stroke, isStroked, ref p0, ref p1);
            }

            for (double y = sy; y < ey; y += cellHeight)
            {
                var p0 = new Point(ox, y);
                var p1 = new Point(ex, y);
                DrawLineInternal(dc, half, stroke, isStroked, ref p0, ref p1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="container"></param>
        private static void DrawBackground(DrawingContext dc, Container container)
        {
            var brush = CreateBrush(container.Background);
            var rect = new Rect(
                0,
                0,
                container.Width,
                container.Height);

            DrawRectangleInternal(
                dc,
                0.5,
                brush,
                null,
                false,
                true,
                ref rect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isZooming"></param>
        public void ClearCache(bool isZooming)
        {
            _styleCache = new Dictionary<ShapeStyle, Tuple<Brush, Pen>>();
            _arrowStyleCache = new Dictionary<ArrowStyle, Tuple<Brush, Pen>>();

            if (!isZooming)
            {
                _arcCache = new Dictionary<XArc, PathGeometry>();
                _bezierCache = new Dictionary<XBezier, PathGeometry>();
                _qbezierCache = new Dictionary<XQBezier, PathGeometry>();
                _textCache = new Dictionary<XText, Tuple<string, FormattedText, ShapeStyle>>();

                if (_biCache != null)
                {
                    foreach (var kvp in _biCache)
                    {
                        kvp.Value.StreamSource.Dispose();
                    }
                    _biCache.Clear();
                }
                _biCache = new Dictionary<Uri, BitmapImage>();

                _pathCache = new Dictionary<XPath, Tuple<XPathGeometry, StreamGeometry, ShapeStyle>>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="container"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, Container container, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _dc = dc as DrawingContext;

            DrawBackground(_dc, container.Template);
            DrawBackground(_dc, container);

            foreach (var layer in container.Layers)
            {
                if (layer.IsVisible)
                {
                    Draw(dc, layer, db, r);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="layer"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, Layer layer, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _dc = dc as DrawingContext;

            foreach (var shape in layer.Shapes)
            {
                shape.Bind(r);
            }

            foreach (var shape in layer.Shapes)
            {
                if (shape.State.HasFlag(_state.DrawShapeState))
                {
                    shape.Draw(_dc, this, 0, 0, db, r);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="line"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, XLine line, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _dc = dc as DrawingContext;

            var style = line.Style;
            if (style == null)
                return;

            double zoom = _state.Zoom;
            double thicknessLine = style.Thickness / zoom;
            double halfLine = thicknessLine / 2.0;
            double thicknessStartArrow = style.StartArrowStyle.Thickness / zoom;
            double halfStartArrow = thicknessStartArrow / 2.0;
            double thicknessEndArrow = style.EndArrowStyle.Thickness / zoom;
            double halfEndArrow = thicknessEndArrow / 2.0;
            
            // line style
            Tuple<Brush, Pen> lineCache = null;
            Brush fillLine;
            Pen strokeLine;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(style, out lineCache))
            {
                fillLine = lineCache.Item1;
                strokeLine = lineCache.Item2;
            }
            else
            {
                fillLine = CreateBrush(style.Fill);
                strokeLine = CreatePen(style, thicknessLine);
                if (_enableStyleCache)
                    _styleCache.Add(style, Tuple.Create(fillLine, strokeLine));
            }
        
            // start arrow style
            Tuple<Brush, Pen> startArrowCache = null;
            Brush fillStartArrow;
            Pen strokeStartArrow;
            if (_enableArrowStyleCache 
                && _arrowStyleCache.TryGetValue(style.StartArrowStyle, out startArrowCache))
            {
                fillStartArrow = startArrowCache.Item1;
                strokeStartArrow = startArrowCache.Item2;
            }
            else
            {
                fillStartArrow = CreateBrush(style.StartArrowStyle.Fill);
                strokeStartArrow = CreatePen(style.StartArrowStyle, thicknessStartArrow);
                if (_enableArrowStyleCache)
                    _arrowStyleCache.Add(style.StartArrowStyle, Tuple.Create(fillStartArrow, strokeStartArrow));
            }
            
            // end arrow style
            Tuple<Brush, Pen> endArrowCache = null;
            Brush fillEndArrow;
            Pen strokeEndArrow;
            if (_enableArrowStyleCache 
                && _arrowStyleCache.TryGetValue(style.EndArrowStyle, out endArrowCache))
            {
                fillEndArrow = endArrowCache.Item1;
                strokeEndArrow = endArrowCache.Item2;
            }
            else
            {
                fillEndArrow = CreateBrush(style.EndArrowStyle.Fill);
                strokeEndArrow = CreatePen(style.EndArrowStyle, thicknessEndArrow);
                if (_enableArrowStyleCache)
                    _arrowStyleCache.Add(style.EndArrowStyle, Tuple.Create(fillEndArrow, strokeEndArrow));
            }

            // line max length
            double x1 = line.Start.X + dx;
            double y1 = line.Start.Y + dy;
            double x2 = line.End.X + dx;
            double y2 = line.End.Y + dy;

            XLine.SetMaxLength(line, ref x1, ref y1, ref x2, ref y2);

            // arrow transforms
            var sas = style.StartArrowStyle;
            var eas = style.EndArrowStyle;
            double a1 = Math.Atan2(y1 - y2, x1 - x2) * 180.0 / Math.PI;
            double a2 = Math.Atan2(y2 - y1, x2 - x1) * 180.0 / Math.PI;
            bool doRectTransform1 = a1 % 90.0 != 0.0;
            bool doRectTransform2 = a2 % 90.0 != 0.0;

            var t1 = new RotateTransform(a1, x1, y1);
            var t2 = new RotateTransform(a2, x2, y2);

            Point pt1;
            Point pt2;
   
            // draw start arrow
            double radiusX1 = sas.RadiusX;
            double radiusY1 = sas.RadiusY;
            double sizeX1 = 2.0 * radiusX1;
            double sizeY1 = 2.0 * radiusY1;

            switch (sas.ArrowType) 
            {
                default:
                case ArrowType.None:
                    {
                        pt1 = new Point(x1, y1);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt1 = t1.Transform(new Point(x1 - sizeX1, y1));
                        var rect = new Rect(x1 - sizeX1, y1 - radiusY1, sizeX1, sizeY1);
                        if (doRectTransform1)
                        {
                            _dc.PushTransform(t1);
                            DrawRectangleInternal(_dc, halfStartArrow, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref rect);
                            _dc.Pop();
                        }
                        else
                        {
                            var bounds = t1.TransformBounds(rect);
                            DrawRectangleInternal(_dc, halfStartArrow, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref bounds);
                        }
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt1 = t1.Transform(new Point(x1 - sizeX1, y1));
                        _dc.PushTransform(t1);
                        var c = new Point(x1 - radiusX1, y1);
                        DrawEllipseInternal(_dc, halfStartArrow, fillStartArrow, strokeStartArrow, sas.IsStroked, sas.IsFilled, ref c, radiusX1, radiusY1);
                        _dc.Pop();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                        pt1 = t1.Transform(new Point(x1, y1));
                        var p11 = t1.Transform(new Point(x1 - sizeX1, y1 + sizeY1));
                        var p21 = t1.Transform(new Point(x1, y1));
                        var p12 = t1.Transform(new Point(x1 - sizeX1, y1 - sizeY1));
                        var p22 = t1.Transform(new Point(x1, y1));
                        DrawLineInternal(_dc, halfStartArrow, strokeStartArrow, sas.IsStroked, ref p11, ref p21);
                        DrawLineInternal(_dc, halfStartArrow, strokeStartArrow, sas.IsStroked, ref p12, ref p22);
                    }
                    break;
            }
            
            // draw end arrow
            double radiusX2 = eas.RadiusX;
            double radiusY2 = eas.RadiusY;
            double sizeX2 = 2.0 * radiusX2;
            double sizeY2 = 2.0 * radiusY2;
            
            switch (eas.ArrowType) 
            {
                default:
                case ArrowType.None:
                    {
                        pt2 = new Point(x2, y2);
                    }
                    break;
                case ArrowType.Rectangle:
                    {
                        pt2 = t2.Transform(new Point(x2 - sizeX2, y2));
                        var rect = new Rect(x2 - sizeX2, y2 - radiusY2, sizeX2, sizeY2);
                        if (doRectTransform2)
                        {
                            _dc.PushTransform(t2);
                            DrawRectangleInternal(_dc, halfEndArrow, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref rect);
                            _dc.Pop();
                        }
                        else
                        {
                            var bounds = t2.TransformBounds(rect);
                            DrawRectangleInternal(_dc, halfEndArrow, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref bounds);
                        }
                    }
                    break;
                case ArrowType.Ellipse:
                    {
                        pt2 = t2.Transform(new Point(x2 - sizeX2, y2));
                        _dc.PushTransform(t2);
                        var c = new Point(x2 - radiusX2, y2);
                        DrawEllipseInternal(_dc, halfEndArrow, fillEndArrow, strokeEndArrow, eas.IsStroked, eas.IsFilled, ref c, radiusX2, radiusY2);
                        _dc.Pop();
                    }
                    break;
                case ArrowType.Arrow:
                    {
                         pt2 = t2.Transform(new Point(x2, y2));
                         var p11 = t2.Transform(new Point(x2 - sizeX2, y2 + sizeY2));
                         var p21 = t2.Transform(new Point(x2, y2));
                         var p12 = t2.Transform(new Point(x2 - sizeX2, y2 - sizeY2));
                         var p22 = t2.Transform(new Point(x2, y2));
                         DrawLineInternal(_dc, halfEndArrow, strokeEndArrow, eas.IsStroked, ref p11, ref p21);
                         DrawLineInternal(_dc, halfEndArrow, strokeEndArrow, eas.IsStroked, ref p12, ref p22);
                    }
                    break;
            }

            // draw line using points from arrow transforms
            DrawLineInternal(_dc, halfLine, strokeLine, line.IsStroked, ref pt1, ref pt2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="rectangle"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, XRectangle rectangle, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _dc = dc as DrawingContext;

            var style = rectangle.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / _state.Zoom;
            double half = thickness / 2.0;

            Tuple<Brush, Pen> cache = null;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache
                && _styleCache.TryGetValue(style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(style.Fill);
                stroke = CreatePen(style, thickness);
                if (_enableStyleCache)
                    _styleCache.Add(style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                rectangle.TopLeft, 
                rectangle.BottomRight, 
                dx, dy);

            DrawRectangleInternal(
                _dc, 
                half, 
                fill, stroke, 
                rectangle.IsStroked, rectangle.IsFilled, 
                ref rect);

            if (rectangle.IsGrid)
            {
                DrawGridInternal(
                    _dc,
                    half,
                    stroke,
                    ref rect,
                    rectangle.OffsetX, rectangle.OffsetY,
                    rectangle.CellWidth, rectangle.CellHeight,
                    true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="ellipse"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, XEllipse ellipse, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _dc = dc as DrawingContext;

            var style = ellipse.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / _state.Zoom;
            double half = thickness / 2.0;

            Tuple<Brush, Pen> cache = null;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(style.Fill);
                stroke = CreatePen(style, thickness);
                if (_enableStyleCache)
                    _styleCache.Add(style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy);
            double rx = rect.Width / 2.0;
            double ry = rect.Height / 2.0;
            var center = new Point(rect.X + rx, rect.Y + ry);

            DrawEllipseInternal(
                _dc, 
                half, 
                fill, stroke, 
                ellipse.IsStroked, ellipse.IsFilled, 
                ref center,
                rx, ry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, XArc arc, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _dc = dc as DrawingContext;

            var style = arc.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / _state.Zoom;
            double half = thickness / 2.0;

            Tuple<Brush, Pen> cache = null;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache
                && _styleCache.TryGetValue(style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(style.Fill);
                stroke = CreatePen(style, thickness);
                if (_enableStyleCache)
                    _styleCache.Add(style, Tuple.Create(fill, stroke));
            }

            var a = WpfArc.FromXArc(arc, dx, dy);

            PathGeometry pg = null;
            if (_enableArcCache
                && _arcCache.TryGetValue(arc, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new Point(a.Start.X, a.Start.Y);
                pf.IsFilled = arc.IsFilled;
                var segment = pf.Segments[0] as ArcSegment;
                segment.Point = new Point(a.End.X, a.End.Y);
                segment.Size = new Size(a.Radius.Width, a.Radius.Height);
                segment.IsLargeArc = a.IsLargeArc;
                segment.IsStroked = arc.IsStroked;
            }
            else
            {
                var pf = new PathFigure()
                {
                    StartPoint = new Point(a.Start.X, a.Start.Y),
                    IsFilled = arc.IsFilled
                };

                var segment = new ArcSegment(
                    new Point(a.End.X, a.End.Y),
                    new Size(a.Radius.Width, a.Radius.Height), 
                    0.0, 
                    a.IsLargeArc, SweepDirection.Clockwise, 
                    arc.IsStroked);

                //segment.Freeze();
                pf.Segments.Add(segment);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                if (_enableArcCache)
                    _arcCache.Add(arc, pg);
            }

            DrawPathGeometryInternal(_dc, half, fill, stroke, arc.IsStroked, arc.IsFilled, pg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="bezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, XBezier bezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _dc = dc as DrawingContext;

            var style = bezier.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / _state.Zoom;
            double half = thickness / 2.0;

            Tuple<Brush, Pen> cache = null;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(style.Fill);
                stroke = CreatePen(style, thickness);
                if (_enableStyleCache)
                    _styleCache.Add(style, Tuple.Create(fill, stroke));
            }

            PathGeometry pg = null;
            if (_enableBezierCache 
                && _bezierCache.TryGetValue(bezier, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new Point(bezier.Point1.X + dx, bezier.Point1.Y + dy);
                pf.IsFilled = bezier.IsFilled;
                var bs = pf.Segments[0] as BezierSegment;
                bs.Point1 = new Point(bezier.Point2.X + dx, bezier.Point2.Y + dy);
                bs.Point2 = new Point(bezier.Point3.X + dx, bezier.Point3.Y + dy);
                bs.Point3 = new Point(bezier.Point4.X + dx, bezier.Point4.Y + dy);
                bs.IsStroked = bezier.IsStroked;
            }
            else
            {
                var pf = new PathFigure()
                {
                    StartPoint = new Point(bezier.Point1.X + dx, bezier.Point1.Y + dy),
                    IsFilled = bezier.IsFilled
                };
                var bs = new BezierSegment(
                        new Point(bezier.Point2.X + dx, bezier.Point2.Y + dy),
                        new Point(bezier.Point3.X + dx, bezier.Point3.Y + dy),
                        new Point(bezier.Point4.X + dx, bezier.Point4.Y + dy),
                        bezier.IsStroked);
                //bs.Freeze();
                pf.Segments.Add(bs);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                if (_enableBezierCache)
                    _bezierCache.Add(bezier, pg);
            }

            DrawPathGeometryInternal(_dc, half, fill, stroke, bezier.IsStroked, bezier.IsFilled, pg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="qbezier"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, XQBezier qbezier, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _dc = dc as DrawingContext;

            var style = qbezier.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / _state.Zoom;
            double half = thickness / 2.0;

            Tuple<Brush, Pen> cache = null;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(style.Fill);
                stroke = CreatePen(style, thickness);
                if (_enableStyleCache)
                    _styleCache.Add(style, Tuple.Create(fill, stroke));
            }

            PathGeometry pg = null;

            if (_enableQBezierCache 
                && _qbezierCache.TryGetValue(qbezier, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new Point(qbezier.Point1.X + dx, qbezier.Point1.Y + dy);
                pf.IsFilled = qbezier.IsFilled;
                var qbs = pf.Segments[0] as QuadraticBezierSegment;
                qbs.Point1 = new Point(qbezier.Point2.X + dx, qbezier.Point2.Y + dy);
                qbs.Point2 = new Point(qbezier.Point3.X + dx, qbezier.Point3.Y + dy);
                qbs.IsStroked = qbezier.IsStroked;
            }
            else
            {
                var pf = new PathFigure()
                {
                    StartPoint = new Point(qbezier.Point1.X + dx, qbezier.Point1.Y + dy),
                    IsFilled = qbezier.IsFilled
                };

                var qbs = new QuadraticBezierSegment(
                        new Point(qbezier.Point2.X + dx, qbezier.Point2.Y + dy),
                        new Point(qbezier.Point3.X + dx, qbezier.Point3.Y + dy),
                        qbezier.IsStroked);
                //bs.Freeze();
                pf.Segments.Add(qbs);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();
                if (_enableQBezierCache)
                    _qbezierCache.Add(qbezier, pg);
            }

            DrawPathGeometryInternal(_dc, half, fill, stroke, qbezier.IsStroked, qbezier.IsFilled, pg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="text"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, XText text, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var _dc = dc as DrawingContext;

            var style = text.Style;
            if (style == null)
                return;

            var tbind = text.BindToTextProperty(db, r);
            if (string.IsNullOrEmpty(tbind))
                return;

            double thickness = style.Thickness / _state.Zoom;
            double half = thickness / 2.0;

            Tuple<Brush, Pen> cache = null;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(style.Fill);
                stroke = CreatePen(style, thickness);
                if (_enableStyleCache)
                    _styleCache.Add(style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                text.TopLeft,
                text.BottomRight,
                dx, dy);

            Tuple<string, FormattedText, ShapeStyle> tcache = null;
            FormattedText ft;
            string ct;
            if (_enableTextCache
                && _textCache.TryGetValue(text, out tcache)
                && string.Compare(tcache.Item1, tbind) == 0
                && tcache.Item3 == style)
            {
                ct = tcache.Item1;
                ft = tcache.Item2;

                _dc.DrawText(
                    ft,
                    GetTextOrigin(style, ref rect, ft));
            }
            else
            {
                var ci = CultureInfo.InvariantCulture;

                var fontStyle = System.Windows.FontStyles.Normal;
                if (style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Italic))
                {
                    fontStyle = System.Windows.FontStyles.Italic;
                }

                var fontWeight = FontWeights.Regular;
                if (style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Bold))
                {
                    fontWeight = FontWeights.Bold;
                }

                var tf = new Typeface(
                    new FontFamily(style.TextStyle.FontName),
                    fontStyle,
                    fontWeight,
                    FontStretches.Normal);

                ft = new FormattedText(
                    tbind,
                    ci,
                    ci.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
                    tf,
                    style.TextStyle.FontSize > 0.0 ? style.TextStyle.FontSize : double.Epsilon,
                    stroke.Brush, null, TextFormattingMode.Ideal);

                if (style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Underline)
                    || style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Strikeout))
                {
                    var decorations = new TextDecorationCollection();

                    if (style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Underline))
                    {
                        decorations = new TextDecorationCollection(
                            decorations.Union(TextDecorations.Underline));
                    }

                    if (style.TextStyle.FontStyle.HasFlag(Test2d.FontStyle.Strikeout))
                    {
                        decorations = new TextDecorationCollection(
                            decorations.Union(TextDecorations.Strikethrough));
                    }

                    ft.SetTextDecorations(decorations);
                }

                if (_enableTextCache)
                {
                    var tuple = Tuple.Create(tbind, ft, style);
                    if (_textCache.ContainsKey(text))
                    {
                        _textCache[text] = tuple;
                    }
                    else
                    {
                        _textCache.Add(text, tuple);
                    }
                }

                _dc.DrawText(
                    ft,
                    GetTextOrigin(style, ref rect, ft));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="image"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, XImage image, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            if (image.Path == null)
                return;

            var _dc = dc as DrawingContext;

            var style = image.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / _state.Zoom;
            double half = thickness / 2.0;

            Tuple<Brush, Pen> cache = null;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache
                && _styleCache.TryGetValue(style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(style.Fill);
                stroke = CreatePen(style, thickness);
                if (_enableStyleCache)
                    _styleCache.Add(style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                image.TopLeft,
                image.BottomRight,
                dx, dy);

            DrawRectangleInternal(_dc, half, fill, stroke, image.IsStroked, image.IsFilled, ref rect);

            if (_enableImageCache
                && _biCache.ContainsKey(image.Path))
            {
                try
                {
                    _dc.DrawImage(_biCache[image.Path], rect);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
            else
            {
                if (!image.Path.IsAbsoluteUri || !System.IO.File.Exists(image.Path.LocalPath))
                    return;

                try
                {
                    byte[] buffer = System.IO.File.ReadAllBytes(image.Path.LocalPath);
                    var ms = new System.IO.MemoryStream(buffer);
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();
                    bi.Freeze();

                    if (_enableImageCache)
                        _biCache[image.Path] = bi;

                    _dc.DrawImage(bi, rect);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    Debug.Print(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="path"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public void Draw(object dc, XPath path, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            if (path.Geometry == null)
                return;

            var _dc = dc as DrawingContext;

            var style = path.Style;
            if (style == null)
                return;

            double thickness = style.Thickness / _state.Zoom;
            double half = thickness / 2.0;

            Tuple<Brush, Pen> cache = null;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(style.Fill);
                stroke = CreatePen(style, thickness);
                if (_enableStyleCache)
                    _styleCache.Add(style, Tuple.Create(fill, stroke));
            }
    
            Tuple<XPathGeometry, StreamGeometry, ShapeStyle> pcache = null;
            StreamGeometry sg;

            if (_enablePathCache
                && _pathCache.TryGetValue(path, out pcache)
                && pcache.Item1 == path.Geometry
                && pcache.Item3 == style)
            {
                sg = pcache.Item2;
                _dc.DrawGeometry(path.IsFilled ? fill : null, path.IsStroked ? stroke : null, sg);
            }
            else
            {
                sg = path.Geometry.ToStreamGeometry();

                if (_enablePathCache)
                {
                    var tuple = Tuple.Create(path.Geometry, sg, style);
                    if (_pathCache.ContainsKey(path))
                    {
                        _pathCache[path] = tuple;
                    }
                    else
                    {
                        _pathCache.Add(path, tuple);
                    }
                }

                _dc.DrawGeometry(path.IsFilled ? fill : null, path.IsStroked ? stroke : null, sg);
            }  
        }
    }
}
