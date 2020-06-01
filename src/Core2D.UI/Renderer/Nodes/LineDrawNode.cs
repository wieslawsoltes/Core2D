using System;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using A = Avalonia;
using AM = Avalonia.Media;
using AME = Avalonia.MatrixExtensions;

namespace Core2D.UI.Renderer
{
    internal class LineDrawNode : DrawNode
    {
        public abstract class Marker
        {
            public IArrowStyle Style;
            public AM.IBrush Brush;
            public AM.IPen Pen;
            public A.Matrix Rotation;
            public A.Point Point;

            public abstract void Draw(AM.DrawingContext context);
        }

        public class NoneMarker : Marker
        {
            public override void Draw(AM.DrawingContext context)
            {
            }
        }

        public class RectangleMarker : Marker
        {
            public A.Rect Rect;

            public override void Draw(AM.DrawingContext context)
            {
                if (Style.IsFilled)
                {
                    context.FillRectangle(Brush, Rect);
                }

                if (Style.IsStroked)
                {
                    context.DrawRectangle(Pen, Rect);
                }
            }
        }

        public class EllipseMarker : Marker
        {
            public AM.EllipseGeometry EllipseGeometry;

            public override void Draw(AM.DrawingContext context)
            {
                using var rotationDisposable = context.PushPreTransform(Rotation);
                context.DrawGeometry(Style.IsFilled ? Brush : null, Style.IsStroked ? Pen : null, EllipseGeometry);
            }
        }

        public class ArrowMarker : Marker
        {
            public A.Point P11;
            public A.Point P21;
            public A.Point P12;
            public A.Point P22;

            public override void Draw(AM.DrawingContext context)
            {
                if (Style.IsStroked)
                {
                    context.DrawLine(Pen, P11, P21);
                    context.DrawLine(Pen, P12, P22);
                }
            }
        }

        public ILineShape Line { get; set; }
        public A.Point P0 { get; set; }
        public A.Point P1 { get; set; }
        public AM.IBrush FillStartArrow { get; set; }
        public AM.IPen StrokeStartArrow { get; set; }
        public AM.IBrush FillEndArrow { get; set; }
        public AM.IPen StrokeEndArrow { get; set; }
        public Marker StartMarker { get; set; }
        public Marker EndMarker { get; set; }
        public AM.StreamGeometry CurveGeometry { get; set; }

        public LineDrawNode(ILineShape line, IShapeStyle style)
        {
            Style = style;
            Line = line;
            UpdateGeometry();
        }

        private AM.StreamGeometry CreateLineCurveGeometry(A.Point p0, A.Point p1, double curvature, CurveOrientation orientation, PointAlignment pt0a, PointAlignment pt1a)
        {
            var curveGeometry = new AM.StreamGeometry();

            using (var geometryContext = curveGeometry.Open())
            {
                geometryContext.BeginFigure(new A.Point(p0.X, p0.Y), false);
                double p0x = p0.X;
                double p0y = p0.Y;
                double p1x = p1.X;
                double p1y = p1.Y;
                LineShapeExtensions.GetCurvedLineBezierControlPoints(orientation, curvature, pt0a, pt1a, ref p0x, ref p0y, ref p1x, ref p1y);
                var point1 = new A.Point(p0x, p0y);
                var point2 = new A.Point(p1x, p1y);
                var point3 = new A.Point(p1.X, p1.Y);
                geometryContext.CubicBezierTo(point1, point2, point3);
                geometryContext.EndFigure(false);
            }

            return curveGeometry;
        }

        private Marker CreateLineArrowMarker(AM.IPen pen, AM.IBrush brush, double x, double y, double angle, IArrowStyle style)
        {
            switch (style.ArrowType)
            {
                default:
                case ArrowType.None:
                    {
                        var marker = new NoneMarker();

                        marker.Style = style;
                        marker.Brush = brush;
                        marker.Pen = pen;
                        marker.Point = new A.Point(x, y);

                        return marker;
                    }
                case ArrowType.Rectangle:
                    {
                        double rx = style.RadiusX;
                        double ry = style.RadiusY;
                        double sx = 2.0 * rx;
                        double sy = 2.0 * ry;

                        var marker = new RectangleMarker();

                        marker.Style = style;
                        marker.Brush = brush;
                        marker.Pen = pen;
                        marker.Rotation = AME.MatrixHelper.Rotation(angle, new A.Vector(x, y));
                        marker.Point = AME.MatrixHelper.TransformPoint(marker.Rotation, new A.Point(x - (float)sx, y));

                        var rect2 = new Rect2(x - sx, y - ry, sx, sy);
                        marker.Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);

                        return marker;
                    }
                case ArrowType.Ellipse:
                    {
                        double rx = style.RadiusX;
                        double ry = style.RadiusY;
                        double sx = 2.0 * rx;
                        double sy = 2.0 * ry;

                        var marker = new EllipseMarker();

                        marker.Style = style;
                        marker.Brush = brush;
                        marker.Pen = pen;
                        marker.Rotation = AME.MatrixHelper.Rotation(angle, new A.Vector(x, y));
                        marker.Point = AME.MatrixHelper.TransformPoint(marker.Rotation, new A.Point(x - (float)sx, y));

                        var rect2 = new Rect2(x - sx, y - ry, sx, sy);
                        var rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
                        marker.EllipseGeometry = new AM.EllipseGeometry(rect);

                        return marker;
                    }
                case ArrowType.Arrow:
                    {
                        double rx = style.RadiusX;
                        double ry = style.RadiusY;
                        double sx = 2.0 * rx;
                        double sy = 2.0 * ry;

                        var marker = new ArrowMarker();

                        marker.Style = style;
                        marker.Brush = brush;
                        marker.Pen = pen;
                        marker.Rotation = AME.MatrixHelper.Rotation(angle, new A.Vector(x, y));
                        marker.Point = AME.MatrixHelper.TransformPoint(marker.Rotation, new A.Point(x, y));

                        marker.P11 = AME.MatrixHelper.TransformPoint(marker.Rotation, new A.Point(x - (float)sx, y + (float)sy));
                        marker.P21 = AME.MatrixHelper.TransformPoint(marker.Rotation, new A.Point(x, y));
                        marker.P12 = AME.MatrixHelper.TransformPoint(marker.Rotation, new A.Point(x - (float)sx, y - (float)sy));
                        marker.P22 = AME.MatrixHelper.TransformPoint(marker.Rotation, new A.Point(x, y));

                        return marker;
                    }
            }
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Line.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Line.State.Flags.HasFlag(ShapeStateFlags.Size);
            P0 = new A.Point(Line.Start.X, Line.Start.Y);
            P1 = new A.Point(Line.End.X, Line.End.Y);
            Center = new A.Point((P0.X + P1.X) / 2.0, (P0.Y + P1.Y) / 2.0);

            double x1 = Line.Start.X;
            double y1 = Line.Start.Y;
            double x2 = Line.End.X;
            double y2 = Line.End.Y;
            Line.GetMaxLength(ref x1, ref y1, ref x2, ref y2);

            if (Style.StartArrowStyle.ArrowType != ArrowType.None)
            {
                double a1 = Math.Atan2(y1 - y2, x1 - x2);
                StartMarker = CreateLineArrowMarker(StrokeStartArrow, FillStartArrow, x1, y1, a1, Style.StartArrowStyle);
                P0 = StartMarker.Point;
            }

            if (Style.EndArrowStyle.ArrowType != ArrowType.None)
            {
                double a2 = Math.Atan2(y2 - y1, x2 - x1);
                EndMarker = CreateLineArrowMarker(StrokeEndArrow, FillEndArrow, x2, y2, a2, Style.EndArrowStyle);
                P1 = EndMarker.Point;
            }

            CurveGeometry = CreateLineCurveGeometry(P0, P1, Style.LineStyle.Curvature, Style.LineStyle.CurveOrientation, Line.Start.Alignment, Line.End.Alignment);
        }

        public override void UpdateStyle()
        {
            base.UpdateStyle();

            if (Style.StartArrowStyle.ArrowType != ArrowType.None)
            {
                FillStartArrow = ToBrush(Style.StartArrowStyle.Fill);
                StrokeStartArrow = ToPen(Style.StartArrowStyle, Style.StartArrowStyle.Thickness);
            }

            if (Style.EndArrowStyle.ArrowType != ArrowType.None)
            {
                FillEndArrow = ToBrush(Style.EndArrowStyle.Fill);
                StrokeEndArrow = ToPen(Style.EndArrowStyle, Style.EndArrowStyle.Thickness);
            }
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            if (Line.IsStroked)
            {
                if (Style.StartArrowStyle.ArrowType != ArrowType.None)
                {
                    StartMarker?.Draw(context);
                }

                if (Style.EndArrowStyle.ArrowType != ArrowType.None)
                {
                    EndMarker?.Draw(context);
                }

                if (Style.LineStyle.IsCurved)
                {
                    context.DrawGeometry(null, Stroke, CurveGeometry);
                }
                else
                {
                    context.DrawLine(Stroke, P0, P1);
                }
            }
        }
    }
}
