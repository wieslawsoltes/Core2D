// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Filters
{
    public class LineSnapPointFilter : PointFilter
    {
        public override string Title => "Line-Snap";

        public LineSnapSettings Settings { get; set; }

        public override bool Process(IToolContext context, ref double x, ref double y)
        {
            if (Settings.IsEnabled == false)
            {
                return false;
            }

            if (Settings.Target.HasFlag(LineSnapTarget.Guides))
            {
                if (Process(context, ref x, ref y, context.CurrentContainer.Guides))
                {
                    return true;
                }
            }

            if (Settings.Target.HasFlag(LineSnapTarget.Shapes))
            {
                if (Process(context, ref x, ref y, context.CurrentContainer.Shapes.OfType<LineShape>()))
                {
                    return true;
                }
            }

            return false;
        }

        private bool Process(IToolContext context, ref double x, ref double y, IEnumerable<LineShape> lines)
        {
            if (lines.Any() && Settings.Mode != LineSnapMode.None)
            {
                Point2 snap;
                LineSnapMode mode;
                var result = SnapLines(lines, Settings.Mode, Settings.Threshold, new Point2(x, y), out snap, out mode);
                if (result)
                {
                    x = snap.X;
                    y = snap.Y;

                    if (Settings.EnableGuides)
                    {
                        Clear(context);
                        PointGuides(context, x, y);
                    }

                    Log.Info($"Line Snap {mode} ({Settings.Target})");
                    return true;
                }
                Clear(context);
            }
            Clear(context);
            return false;
        }

        private void PointGuides(IToolContext context, double x, double y)
        {
            var horizontal = new LineShape()
            {
                StartPoint = new PointShape(0, y, null),
                Point = new PointShape(context.CurrentContainer.Width, y, null),
                Style = Settings.GuideStyle
            };

            var vertical = new LineShape()
            {
                StartPoint = new PointShape(x, 0, null),
                Point = new PointShape(x, context.CurrentContainer.Height, null),
                Style = Settings.GuideStyle
            };

            Guides.Add(horizontal);
            Guides.Add(vertical);

            context.WorkingContainer.Shapes.Add(horizontal);
            context.WorkingContainer.Shapes.Add(vertical);
        }

        public static bool SnapLinesToPoint(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);

            foreach (var line in lines)
            {
                var distance0 = line.StartPoint.ToPoint2().DistanceTo(point);
                if (distance0 < threshold)
                {
                    snap = new Point2(line.StartPoint.X, line.StartPoint.Y);
                    result = LineSnapMode.Point;
                    return true;
                }
                var distance1 = line.Point.ToPoint2().DistanceTo(point);
                if (distance1 < threshold)
                {
                    snap = new Point2(line.Point.X, line.Point.Y);
                    result = LineSnapMode.Point;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLineToMiddle(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);

            foreach (var line in lines)
            {
                var middle = Line2.Middle(line.StartPoint.ToPoint2(), line.Point.ToPoint2());
                var distance = middle.DistanceTo(point);
                if (distance < threshold)
                {
                    snap = middle;
                    result = LineSnapMode.Middle;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLineToIntersection(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);

            foreach (var line0 in lines)
            {
                foreach (var line1 in lines)
                {
                    if (line0 == line1)
                        continue;
                    Point2 clip;
                    if (Line2.LineIntersectWithLine(line0.StartPoint.ToPoint2(), line0.Point.ToPoint2(), line1.StartPoint.ToPoint2(), line1.Point.ToPoint2(), out clip))
                    {
                        var distance = clip.DistanceTo(point);
                        if (distance < threshold)
                        {
                            snap = clip;
                            result = LineSnapMode.Intersection;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool SnapLineToHorizontal(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result, out double horizontal)
        {
            snap = default(Point2);
            result = default(LineSnapMode);
            horizontal = default(double);

            foreach (var line in lines)
            {
                if (point.Y >= line.StartPoint.Y - threshold && point.Y <= line.StartPoint.Y + threshold)
                {
                    snap = new Point2(point.X, line.StartPoint.Y);
                    result |= LineSnapMode.Horizontal;
                    horizontal = line.StartPoint.Y;
                    return true;
                }
                if (point.Y >= line.Point.Y - threshold && point.Y <= line.Point.Y + threshold)
                {
                    snap = new Point2(point.X, line.StartPoint.Y);
                    result |= LineSnapMode.Horizontal;
                    horizontal = line.Point.Y;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLinesToVertical(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result, out double vertical)
        {
            snap = default(Point2);
            result = default(LineSnapMode);
            vertical = default(double);

            foreach (var line in lines)
            {
                if (point.X >= line.StartPoint.X - threshold && point.X <= line.StartPoint.X + threshold)
                {
                    snap = new Point2(line.StartPoint.X, point.Y);
                    result |= LineSnapMode.Vertical;
                    vertical = line.StartPoint.X;
                    return true;
                }
                if (point.X >= line.Point.X - threshold && point.X <= line.Point.X + threshold)
                {
                    snap = new Point2(line.Point.X, point.Y);
                    result |= LineSnapMode.Vertical;
                    vertical = line.Point.X;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLinesToNearest(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);

            foreach (var line in lines)
            {
                var nearest = point.NearestOnLine(line.StartPoint.ToPoint2(), line.Point.ToPoint2());
                var distance = nearest.DistanceTo(point);
                if (distance < threshold)
                {
                    snap = nearest;
                    result = LineSnapMode.Nearest;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLines(IEnumerable<LineShape> lines, LineSnapMode mode, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);

            if (mode.HasFlag(LineSnapMode.Point))
            {
                if (SnapLinesToPoint(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Middle))
            {
                if (SnapLineToMiddle(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Intersection))
            {
                if (SnapLineToIntersection(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            double horizontal = default(double);
            double vertical = default(double);

            if (mode.HasFlag(LineSnapMode.Horizontal))
            {
                SnapLineToHorizontal(lines, threshold, point, out snap, out result, out horizontal);
            }

            if (mode.HasFlag(LineSnapMode.Vertical))
            {
                SnapLinesToVertical(lines, threshold, point, out snap, out result, out vertical);
            }

            if (result.HasFlag(LineSnapMode.Horizontal) || result.HasFlag(LineSnapMode.Vertical))
            {
                if (result.HasFlag(LineSnapMode.Vertical) || result.HasFlag(LineSnapMode.Horizontal))
                {
                    double x = result.HasFlag(LineSnapMode.Vertical) ? vertical : point.X;
                    double y = result.HasFlag(LineSnapMode.Horizontal) ? horizontal : point.Y;
                    snap = new Point2(x, y);
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Nearest))
            {
                if (SnapLinesToNearest(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
