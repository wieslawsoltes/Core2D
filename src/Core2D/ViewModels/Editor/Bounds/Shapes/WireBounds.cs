// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
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

        if (wire.Start is null || wire.End is null)
        {
            return false;
        }

        Point2 a;
        Point2 b;
        if (wire.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            a = new Point2(wire.Start.X * scale, wire.Start.Y * scale);
            b = new Point2(wire.End.X * scale, wire.End.Y * scale);
        }
        else
        {
            a = new Point2(wire.Start.X, wire.Start.Y);
            b = new Point2(wire.End.X, wire.End.Y);
        }

        var nearest = target.NearestOnLine(a, b);
        var distance = target.DistanceTo(nearest);
        return distance < radius;
    }

    public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not WireShapeViewModel wire)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (wire.Start is null || wire.End is null)
        {
            return false;
        }

        Point2 a;
        Point2 b;
        if (wire.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            a = new Point2(wire.Start.X * scale, wire.Start.Y * scale);
            b = new Point2(wire.End.X * scale, wire.End.Y * scale);
        }
        else
        {
            a = new Point2(wire.Start.X, wire.Start.Y);
            b = new Point2(wire.End.X, wire.End.Y);
        }

        return Line2.LineIntersectsWithRect(a, b, target, out _, out _, out _, out _);
    }
}
