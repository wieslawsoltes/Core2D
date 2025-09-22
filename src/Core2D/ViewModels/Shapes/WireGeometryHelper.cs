// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core2D.Spatial;

namespace Core2D.ViewModels.Shapes;

public readonly struct WireGeometry
{
    public WireGeometry(Point2 start, Point2 control1, Point2 control2, Point2 end, bool isBezier)
    {
        Start = start;
        Control1 = control1;
        Control2 = control2;
        End = end;
        IsBezier = isBezier;
    }

    public Point2 Start { get; }

    public Point2 Control1 { get; }

    public Point2 Control2 { get; }

    public Point2 End { get; }

    public bool IsBezier { get; }
}

public static class WireGeometryHelper
{
    private const double DefaultOffset = 40.0;
    private const double Epsilon = 1e-6;

    private enum ConnectorSide
    {
        Unknown,
        Left,
        Right,
        Top,
        Bottom
    }

    public static bool TryCreateGeometry(WireShapeViewModel? wire, out WireGeometry geometry)
    {
        if (wire is null)
        {
            geometry = default;
            return false;
        }

        if (wire.Start is null || wire.End is null)
        {
            geometry = default;
            return false;
        }

        var start = new Point2(wire.Start.X, wire.Start.Y);
        var end = new Point2(wire.End.X, wire.End.Y);

        if (string.Equals(wire.RendererKey, WireRendererKeys.Bezier, StringComparison.OrdinalIgnoreCase))
        {
            var (c1, c2) = GetControlPoints(wire, start, end);
            geometry = new WireGeometry(start, c1, c2, end, true);
        }
        else
        {
            geometry = new WireGeometry(start, start, end, end, false);
        }

        return true;
    }

    public static Point2[] Sample(WireGeometry geometry, int steps)
    {
        if (steps < 1)
        {
            steps = 1;
        }

        var points = new Point2[steps + 1];
        for (int i = 0; i <= steps; i++)
        {
            var t = i / (double)steps;
            points[i] = Evaluate(geometry, t);
        }
        return points;
    }

    public static Point2 Evaluate(WireGeometry geometry, double t)
    {
        t = Math.Clamp(t, 0.0, 1.0);
        if (!geometry.IsBezier)
        {
            return Lerp(geometry.Start, geometry.End, t);
        }

        var mt = 1.0 - t;
        var mt2 = mt * mt;
        var t2 = t * t;

        var x = (mt2 * mt * geometry.Start.X)
            + (3 * mt2 * t * geometry.Control1.X)
            + (3 * mt * t2 * geometry.Control2.X)
            + (t * t2 * geometry.End.X);
        var y = (mt2 * mt * geometry.Start.Y)
            + (3 * mt2 * t * geometry.Control1.Y)
            + (3 * mt * t2 * geometry.Control2.Y)
            + (t * t2 * geometry.End.Y);

        return new Point2(x, y);
    }

    public static Point2 GetStartTangent(WireGeometry geometry)
    {
        if (!geometry.IsBezier)
        {
            return new Point2(geometry.End.X - geometry.Start.X, geometry.End.Y - geometry.Start.Y);
        }

        var tangent = new Point2(geometry.Control1.X - geometry.Start.X, geometry.Control1.Y - geometry.Start.Y);
        if (IsNearZero(tangent))
        {
            tangent = new Point2(geometry.End.X - geometry.Start.X, geometry.End.Y - geometry.Start.Y);
        }
        return tangent;
    }

    public static Point2 GetEndTangent(WireGeometry geometry)
    {
        if (!geometry.IsBezier)
        {
            return new Point2(geometry.End.X - geometry.Start.X, geometry.End.Y - geometry.Start.Y);
        }

        var tangent = new Point2(geometry.End.X - geometry.Control2.X, geometry.End.Y - geometry.Control2.Y);
        if (IsNearZero(tangent))
        {
            tangent = new Point2(geometry.End.X - geometry.Start.X, geometry.End.Y - geometry.Start.Y);
        }
        return tangent;
    }

    public static double DistanceToPoint(WireGeometry geometry, Point2 target, int steps = 24)
    {
        var points = Sample(geometry, steps);
        var min = double.MaxValue;
        for (var i = 0; i < points.Length - 1; i++)
        {
            var distance = DistanceToSegment(target, points[i], points[i + 1]);
            if (distance < min)
            {
                min = distance;
            }
        }
        return min;
    }

    public static bool IntersectsRect(WireGeometry geometry, Rect2 rect, int steps = 24)
    {
        var points = Sample(geometry, steps);
        for (var i = 0; i < points.Length - 1; i++)
        {
            if (Line2.LineIntersectsWithRect(points[i], points[i + 1], rect, out _, out _, out _, out _))
            {
                return true;
            }
        }
        return false;
    }

    private static Point2 Lerp(Point2 a, Point2 b, double t)
    {
        return new Point2(
            (a.X * (1.0 - t)) + (b.X * t),
            (a.Y * (1.0 - t)) + (b.Y * t));
    }

    private static (Point2 c1, Point2 c2) GetControlPoints(WireShapeViewModel wire, Point2 start, Point2 end)
    {
        if (TryGetControlPoint(wire, WireShapeViewModel.BezierControl1Property, out var c1)
            && TryGetControlPoint(wire, WireShapeViewModel.BezierControl2Property, out var c2))
        {
            return (c1, c2);
        }

        return ComputeDefaultControls(start, end, wire.Start, wire.End);
    }

    private static (Point2 c1, Point2 c2) ComputeDefaultControls(Point2 start, Point2 end, PointShapeViewModel? startPoint, PointShapeViewModel? endPoint)
    {
        var startSide = GetConnectorSide(startPoint, out var startBounds);
        var endSide = GetConnectorSide(endPoint, out var endBounds);

        var startSet = false;
        var endSet = false;

        var c1 = start;
        var c2 = end;

        if (startSide != ConnectorSide.Unknown)
        {
            c1 = OffsetFromSide(start, startSide, end, startBounds);
            startSet = true;
        }

        if (endSide != ConnectorSide.Unknown)
        {
            c2 = OffsetFromSide(end, endSide, start, endBounds);
            endSet = true;
        }

        if (!startSet || !endSet)
        {
            var fallback = ComputeFallbackControls(start, end);
            if (!startSet)
            {
                c1 = fallback.c1;
            }

            if (!endSet)
            {
                c2 = fallback.c2;
            }
        }

        return (c1, c2);
    }

    private static (Point2 c1, Point2 c2) ComputeFallbackControls(Point2 start, Point2 end)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;

        if (IsNearZero(dx) && IsNearZero(dy))
        {
            return (start, end);
        }

        if (Math.Abs(dx) >= Math.Abs(dy))
        {
            var sign = Math.Sign(dx);
            if (sign == 0)
            {
                sign = dy >= 0 ? 1 : -1;
            }
            var offset = Math.Max(Math.Abs(dx) / 2.0, DefaultOffset) * sign;
            var c1 = new Point2(start.X + offset, start.Y);
            var c2 = new Point2(end.X - offset, end.Y);
            return (c1, c2);
        }
        else
        {
            var sign = Math.Sign(dy);
            if (sign == 0)
            {
                sign = dx >= 0 ? 1 : -1;
            }
            var offset = Math.Max(Math.Abs(dy) / 2.0, DefaultOffset) * sign;
            var c1 = new Point2(start.X, start.Y + offset);
            var c2 = new Point2(end.X, end.Y - offset);
            return (c1, c2);
        }
    }

    private static ConnectorSide GetConnectorSide(PointShapeViewModel? point, out Rect2 bounds)
    {
        bounds = default;

        if (point is null)
        {
            return ConnectorSide.Unknown;
        }

        if (!TryGetOwnerBounds(point, out bounds))
        {
            return ConnectorSide.Unknown;
        }

        var leftDistance = Math.Abs(point.X - bounds.Left);
        var rightDistance = Math.Abs(bounds.Right - point.X);
        var topDistance = Math.Abs(point.Y - bounds.Top);
        var bottomDistance = Math.Abs(bounds.Bottom - point.Y);

        var minEdgeDistance = Math.Min(Math.Min(leftDistance, rightDistance), Math.Min(topDistance, bottomDistance));
        var dominantSpan = Math.Max(bounds.Width, bounds.Height);
        var tolerance = Math.Max(Epsilon, Math.Min(dominantSpan * 0.1, DefaultOffset));

        if (minEdgeDistance <= tolerance)
        {
            if (leftDistance <= minEdgeDistance + Epsilon)
            {
                return ConnectorSide.Left;
            }

            if (rightDistance <= minEdgeDistance + Epsilon)
            {
                return ConnectorSide.Right;
            }

            if (topDistance <= minEdgeDistance + Epsilon)
            {
                return ConnectorSide.Top;
            }

            if (bottomDistance <= minEdgeDistance + Epsilon)
            {
                return ConnectorSide.Bottom;
            }
        }

        var center = bounds.Center;
        var dx = point.X - center.X;
        var dy = point.Y - center.Y;

        if (IsNearZero(dx) && IsNearZero(dy))
        {
            return ConnectorSide.Unknown;
        }

        if (Math.Abs(dx) >= Math.Abs(dy))
        {
            return dx >= 0 ? ConnectorSide.Right : ConnectorSide.Left;
        }

        return dy >= 0 ? ConnectorSide.Bottom : ConnectorSide.Top;
    }

    private static bool TryGetOwnerBounds(PointShapeViewModel point, out Rect2 bounds)
    {
        bounds = default;

        if (point.Owner is not BaseShapeViewModel owner)
        {
            return false;
        }

        if (owner is WireShapeViewModel)
        {
            return false;
        }

        var ownerPoints = new List<PointShapeViewModel>();
        owner.GetPoints(ownerPoints);

        if (ownerPoints.Count == 0)
        {
            return false;
        }

        double minX = double.PositiveInfinity;
        double maxX = double.NegativeInfinity;
        double minY = double.PositiveInfinity;
        double maxY = double.NegativeInfinity;

        foreach (var p in ownerPoints)
        {
            if (double.IsNaN(p.X) || double.IsNaN(p.Y))
            {
                continue;
            }

            minX = Math.Min(minX, p.X);
            maxX = Math.Max(maxX, p.X);
            minY = Math.Min(minY, p.Y);
            maxY = Math.Max(maxY, p.Y);
        }

        if (double.IsInfinity(minX) || double.IsInfinity(maxX) || double.IsInfinity(minY) || double.IsInfinity(maxY))
        {
            return false;
        }

        bounds = Rect2.FromPoints(minX, minY, maxX, maxY);
        return true;
    }

    private static Point2 OffsetFromSide(Point2 point, ConnectorSide side, Point2 opposite, Rect2 bounds)
    {
        var axisDistance = side is ConnectorSide.Left or ConnectorSide.Right
            ? Math.Abs(opposite.X - point.X)
            : Math.Abs(opposite.Y - point.Y);

        var span = side is ConnectorSide.Left or ConnectorSide.Right ? bounds.Width : bounds.Height;
        var directionalOffset = Math.Min(axisDistance * 0.5, DefaultOffset * 4.0);
        var spanOffset = Math.Min(span * 0.5, DefaultOffset * 4.0);
        var offset = Math.Max(DefaultOffset, Math.Max(directionalOffset, spanOffset));

        return side switch
        {
            ConnectorSide.Left => new Point2(point.X - offset, point.Y),
            ConnectorSide.Right => new Point2(point.X + offset, point.Y),
            ConnectorSide.Top => new Point2(point.X, point.Y - offset),
            ConnectorSide.Bottom => new Point2(point.X, point.Y + offset),
            _ => point
        };
    }

    private static bool TryGetControlPoint(WireShapeViewModel wire, string propertyName, out Point2 point)
    {
        if (wire.Properties.IsDefaultOrEmpty)
        {
            point = default;
            return false;
        }

        var property = wire.Properties.FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
        if (property?.Value is { } value && TryParsePoint(value, out point))
        {
            return true;
        }

        point = default;
        return false;
    }

    private static bool TryParsePoint(string value, out Point2 point)
    {
        var parts = value.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            point = default;
            return false;
        }

        if (double.TryParse(parts[0], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var x)
            && double.TryParse(parts[1], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var y))
        {
            point = new Point2(x, y);
            return true;
        }

        point = default;
        return false;
    }

    private static double DistanceToSegment(Point2 p, Point2 a, Point2 b)
    {
        var ax = b.X - a.X;
        var ay = b.Y - a.Y;
        var lengthSquared = (ax * ax) + (ay * ay);
        if (lengthSquared < Epsilon)
        {
            return p.DistanceTo(a);
        }

        var t = ((p.X - a.X) * ax + (p.Y - a.Y) * ay) / lengthSquared;
        t = Math.Clamp(t, 0.0, 1.0);
        var proj = new Point2(a.X + (ax * t), a.Y + (ay * t));
        return p.DistanceTo(proj);
    }

    private static bool IsNearZero(Point2 vector)
    {
        return IsNearZero(vector.X) && IsNearZero(vector.Y);
    }

    private static bool IsNearZero(double value)
    {
        return Math.Abs(value) < Epsilon;
    }
}
