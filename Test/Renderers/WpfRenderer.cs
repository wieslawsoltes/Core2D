// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Test.Core;

namespace Test
{
    public class WpfRenderer : ObservableObject, IRenderer
    {
        private bool _drawPoints;

        public bool DrawPoints
        {
            get { return _drawPoints; }
            set
            {
                if (value != _drawPoints)
                {
                    _drawPoints = value;
                    Notify("DrawPoints");
                }
            }
        }

        private bool _enableStyleCache = true;
        private bool _enableArcCache = true;
        private bool _enableBezierCache = true;
        private bool _enableQBezierCache = true;
        private bool _enableTextCache = true;

        private IDictionary<ShapeStyle, Tuple<Brush, Pen>> _styleCache;
        private IDictionary<XArc, PathGeometry> _arcCache;
        private IDictionary<XBezier, PathGeometry> _bezierCache;
        private IDictionary<XQBezier, PathGeometry> _qbezierCache;
        private IDictionary<XText, FormattedText> _textCache;

        public WpfRenderer()
        {
            ClearCache();
        }

        public static IRenderer Create(bool drawPoints = false)
        {
            return new WpfRenderer()
            {
                DrawPoints = drawPoints
            };
        }

        public void ClearCache()
        {
            _styleCache = new Dictionary<ShapeStyle, Tuple<Brush, Pen>>();
            _arcCache = new Dictionary<XArc, PathGeometry>();
            _bezierCache = new Dictionary<XBezier, PathGeometry>();
            _qbezierCache = new Dictionary<XQBezier, PathGeometry>();
            _textCache = new Dictionary<XText, FormattedText>();
        }

        private Brush CreateBrush(ArgbColor color)
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

        private Pen CreatePen(ArgbColor color, double thickness)
        {
            var brush = CreateBrush(color);
            var pen = new Pen(brush, thickness);
            pen.Freeze();
            return pen;
        }

        private Rect CreateRect(XPoint tl, XPoint br, double dx, double dy)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            return new Rect(
                new Point(tlx + dx, tly + dy),
                new Point(brx + dx, bry + dy));
        }

        public void Render(object dc, ILayer layer)
        {
            var _dc = dc as DrawingContext;

            foreach (var shape in layer.Shapes)
            {
                shape.Draw(_dc, this, 0, 0);
            }
        }

        public void Draw(object dc, XLine line, double dx, double dy)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(line.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(line.Style.Fill);
                stroke = CreatePen(
                    line.Style.Stroke,
                    line.Style.Thickness);

                if (_enableStyleCache)
                    _styleCache.Add(line.Style, Tuple.Create(fill, stroke));
            }

            _dc.DrawLine(
                stroke,
                new Point(line.Start.X + dx, line.Start.Y + dy),
                new Point(line.End.X + dx, line.End.Y + dy));
        }

        public void Draw(object dc, XRectangle rectangle, double dx, double dy)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(rectangle.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(rectangle.Style.Fill);
                stroke = CreatePen(
                    rectangle.Style.Stroke,
                    rectangle.Style.Thickness);

                if (_enableStyleCache)
                    _styleCache.Add(rectangle.Style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                rectangle.TopLeft,
                rectangle.BottomRight,
                dx, dy);
            _dc.DrawRectangle(
                rectangle.IsFilled ? fill : null,
                stroke,
                rect);
        }

        public void Draw(object dc, XEllipse ellipse, double dx, double dy)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(ellipse.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(ellipse.Style.Fill);
                stroke = CreatePen(
                    ellipse.Style.Stroke,
                    ellipse.Style.Thickness);

                if (_enableStyleCache)
                    _styleCache.Add(ellipse.Style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                ellipse.TopLeft,
                ellipse.BottomRight,
                dx, dy);
            double rx = rect.Width / 2.0;
            double ry = rect.Height / 2.0;
            var center = new Point(rect.X + rx, rect.Y + ry);
            _dc.DrawEllipse(
                ellipse.IsFilled ? fill : null,
                stroke,
                center,
                rx, ry);
        }

        public void Draw(object dc, XArc arc, double dx, double dy)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache
                && _styleCache.TryGetValue(arc.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(arc.Style.Fill);
                stroke = CreatePen(
                    arc.Style.Stroke,
                    arc.Style.Thickness);

                if (_enableStyleCache)
                    _styleCache.Add(arc.Style, Tuple.Create(fill, stroke));
            }

            var a = WpfArc.FromXArc(arc, dx, dy);

            PathGeometry pg;
            if (_enableArcCache
                && _arcCache.TryGetValue(arc, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = a.Start;
                pf.IsFilled = arc.IsFilled;
                var segment = pf.Segments[0] as ArcSegment;
                segment.Point = a.End;
                segment.Size = a.Radius;
            }
            else
            {
                var pf = new PathFigure()
                {
                    StartPoint = a.Start,
                    IsFilled = arc.IsFilled
                };

                var segment = new ArcSegment(a.End, a.Radius, 0.0, false, SweepDirection.Clockwise, true);

                //segment.Freeze();
                pf.Segments.Add(segment);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                if (_enableArcCache)
                    _arcCache.Add(arc, pg);
            }

            _dc.DrawGeometry(arc.IsFilled ? fill : null, stroke, pg);
        }

        public void Draw(object dc, XBezier bezier, double dx, double dy)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(bezier.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(bezier.Style.Fill);
                stroke = CreatePen(
                    bezier.Style.Stroke,
                    bezier.Style.Thickness);

                if (_enableStyleCache)
                    _styleCache.Add(bezier.Style, Tuple.Create(fill, stroke));
            }

            PathGeometry pg;
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
                        true);
                //bs.Freeze();
                pf.Segments.Add(bs);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();

                if (_enableBezierCache)
                    _bezierCache.Add(bezier, pg);
            }

            _dc.DrawGeometry(bezier.IsFilled ? fill : null, stroke, pg);
        }

        public void Draw(object dc, XQBezier qbezier, double dx, double dy)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(qbezier.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(qbezier.Style.Fill);
                stroke = CreatePen(
                    qbezier.Style.Stroke,
                    qbezier.Style.Thickness);

                if (_enableStyleCache)
                    _styleCache.Add(qbezier.Style, Tuple.Create(fill, stroke));
            }

            PathGeometry pg;

            if (_enableQBezierCache 
                && _qbezierCache.TryGetValue(qbezier, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new Point(qbezier.Point1.X + dx, qbezier.Point1.Y + dy);
                pf.IsFilled = qbezier.IsFilled;
                var qbs = pf.Segments[0] as QuadraticBezierSegment;
                qbs.Point1 = new Point(qbezier.Point2.X + dx, qbezier.Point2.Y + dy);
                qbs.Point2 = new Point(qbezier.Point3.X + dx, qbezier.Point3.Y + dy);
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
                        true);
                //bs.Freeze();
                pf.Segments.Add(qbs);
                //pf.Freeze();
                pg = new PathGeometry();
                pg.Figures.Add(pf);
                //pg.Freeze();
                if (_enableQBezierCache)
                    _qbezierCache.Add(qbezier, pg);
            }
            
            _dc.DrawGeometry(qbezier.IsFilled ? fill : null, stroke, pg);
        }

        private Point GetTextOrigin(ShapeStyle style, Rect rect, FormattedText ft)
        {
            double ox, oy;

            switch (style.TextHAlignment)
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

            switch (style.TextVAlignment)
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
        
        public void Draw(object dc, XText text, double dx, double dy)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache 
                && _styleCache.TryGetValue(text.Style, out cache))
            {
                fill = cache.Item1;
                stroke = cache.Item2;
            }
            else
            {
                fill = CreateBrush(text.Style.Fill);
                stroke = CreatePen(
                    text.Style.Stroke,
                    text.Style.Thickness);

                if (_enableStyleCache)
                    _styleCache.Add(text.Style, Tuple.Create(fill, stroke));
            }

            var rect = CreateRect(
                text.TopLeft,
                text.BottomRight,
                dx, dy);
            _dc.DrawRectangle(
                text.IsFilled ? fill : null,
                null,
                rect);

            FormattedText ft;

            if (_enableTextCache
                && _textCache.TryGetValue(text, out ft))
            {
                _dc.DrawText(
                    ft, 
                    GetTextOrigin(text.Style, rect, ft));
            }
            else
            {
                var ci = CultureInfo.InvariantCulture;

                ft = new FormattedText(
                    text.Text,
                    ci,
                    ci.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
                    new Typeface(
                        new FontFamily(text.Style.FontName),
                        FontStyles.Normal,
                        FontWeights.Normal,
                        FontStretches.Normal),
                    text.Style.FontSize,
                    stroke.Brush, null, TextFormattingMode.Ideal);

                _dc.DrawText(
                    ft, 
                    GetTextOrigin(text.Style, rect, ft));
            }
        }
    }
}
