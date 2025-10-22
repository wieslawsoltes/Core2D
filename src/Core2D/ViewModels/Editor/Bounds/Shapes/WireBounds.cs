// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Renderer;
using Core2D.Model.Editor;
using Core2D.Spatial;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor.Bounds.Shapes;

public class WireBounds : IBounds
{
    public Type TargetType => typeof(WireShapeViewModel);

    public PointShapeViewModel? TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not WireShapeViewModel wire)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (wire.Start is null || wire.End is null)
        {
            return null;
        }

        var pointHitTest = registered[typeof(PointShapeViewModel)];

        if (pointHitTest.TryToGetPoint(wire.Start, target, radius, scale, registered) is { })
        {
            return wire.Start;
        }

        if (pointHitTest.TryToGetPoint(wire.End, target, radius, scale, registered) is { })
        {
            return wire.End;
        }

        return null;
    }

    public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not WireShapeViewModel wire)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (!WireGeometryHelper.TryCreateGeometry(wire, out var geometry))
        {
            return false;
        }

        if (wire.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            geometry = ScaleGeometry(geometry, scale);
        }

        var distance = WireGeometryHelper.DistanceToPoint(geometry, target);
        return distance < radius;
    }

    public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not WireShapeViewModel wire)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (!WireGeometryHelper.TryCreateGeometry(wire, out var geometry))
        {
            return false;
        }

        if (wire.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            geometry = ScaleGeometry(geometry, scale);
        }

        if (!geometry.IsBezier)
        {
            return Line2.LineIntersectsWithRect(geometry.Start, geometry.End, target, out _, out _, out _, out _);
        }

        return WireGeometryHelper.IntersectsRect(geometry, target);
    }

    private static WireGeometry ScaleGeometry(WireGeometry geometry, double scale)
    {
        static Point2 ScalePoint(Point2 point, double factor) => new(point.X * factor, point.Y * factor);

        return new WireGeometry(
            ScalePoint(geometry.Start, scale),
            ScalePoint(geometry.Control1, scale),
            ScalePoint(geometry.Control2, scale),
            ScalePoint(geometry.End, scale),
            geometry.IsBezier);
    }
}
