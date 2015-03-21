using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class WpfRenderer : IRenderer
    {
        private IDictionary<XStyle, Tuple<Brush, Pen>> _styleCache;
        private readonly bool _enableStyleCache = true;

        private IDictionary<XBezier, PathGeometry> _bezierCache;
        private readonly bool _enableBezierCache = true;

        public WpfRenderer()
        {
            Initialize();
        }

        public void Initialize()
        {
            _styleCache = new Dictionary<XStyle, Tuple<Brush, Pen>>();
            _bezierCache = new Dictionary<XBezier, PathGeometry>();
        }

        private Brush CreateBrush(XColor color)
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

        private Pen CreatePen(XColor color, double thickness)
        {
            var brush = CreateBrush(color);
            var pen = new Pen(brush, thickness);
            pen.Freeze();
            return pen;
        }

        private Rect CreateRect(XPoint topLeft, XPoint bottomRight)
        {
            double tlx = Math.Min(topLeft.X, bottomRight.X);
            double tly = Math.Min(topLeft.Y, bottomRight.Y);
            double brx = Math.Max(topLeft.X, bottomRight.X);
            double bry = Math.Max(topLeft.Y, bottomRight.Y);
            return new Rect(
                new Point(tlx, tly),
                new Point(brx, bry));
        }

        public void Render(object dc, ILayer layer)
        {
            var _dc = dc as DrawingContext;

            foreach (var shape in layer.Shapes)
            {
                shape.Draw(_dc, this);
            }
        }

        public void Draw(object dc, XLine line)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache && _styleCache.TryGetValue(line.Style, out cache))
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
                new Point(line.Start.X, line.Start.Y),
                new Point(line.End.X, line.End.Y));
        }

        public void Draw(object dc, XRectangle rectangle)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache && _styleCache.TryGetValue(rectangle.Style, out cache))
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
                rectangle.BottomRight);
            _dc.DrawRectangle(
                rectangle.IsFilled ? fill : null,
                stroke,
                rect);
        }

        public void Draw(object dc, XEllipse ellipse)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache && _styleCache.TryGetValue(ellipse.Style, out cache))
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
                ellipse.BottomRight);
            double rx = rect.Width / 2.0;
            double ry = rect.Height / 2.0;
            var center = new Point(rect.X + rx, rect.Y + ry);
            _dc.DrawEllipse(
                ellipse.IsFilled ? fill : null,
                stroke,
                center,
                rx, ry);
        }

        public void Draw(object dc, XBezier bezier)
        {
            var _dc = dc as DrawingContext;

            Tuple<Brush, Pen> cache;
            Brush fill;
            Pen stroke;
            if (_enableStyleCache && _styleCache.TryGetValue(bezier.Style, out cache))
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
            if (_enableBezierCache && _bezierCache.TryGetValue(bezier, out pg))
            {
                var pf = pg.Figures[0];
                pf.StartPoint = new Point(bezier.Point1.X, bezier.Point1.Y);
                var bs = pf.Segments[0] as BezierSegment;
                bs.Point1 = new Point(bezier.Point2.X, bezier.Point2.Y);
                bs.Point2 = new Point(bezier.Point3.X, bezier.Point3.Y);
                bs.Point3 = new Point(bezier.Point4.X, bezier.Point4.Y);
            }
            else
            {
                var pf = new PathFigure()
                {
                    StartPoint = new Point(bezier.Point1.X, bezier.Point1.Y)
                };
                var bs = new BezierSegment(
                        new Point(bezier.Point2.X, bezier.Point2.Y),
                        new Point(bezier.Point3.X, bezier.Point3.Y),
                        new Point(bezier.Point4.X, bezier.Point4.Y),
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
    }
}
