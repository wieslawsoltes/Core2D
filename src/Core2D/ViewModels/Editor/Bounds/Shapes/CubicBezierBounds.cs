﻿#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes;

public class CubicBezierBounds : IBounds
{
    private readonly List<PointShapeViewModel> _points = new();

    public Type TargetType => typeof(CubicBezierShapeViewModel);

    public PointShapeViewModel? TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not CubicBezierShapeViewModel cubic)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var pointHitTest = registered[typeof(PointShapeViewModel)];

        if (cubic.Point1 is { } && pointHitTest.TryToGetPoint(cubic.Point1, target, radius, scale, registered) is { })
        {
            return cubic.Point1;
        }

        if (cubic.Point2 is { } && pointHitTest.TryToGetPoint(cubic.Point2, target, radius, scale, registered) is { })
        {
            return cubic.Point2;
        }

        if (cubic.Point3 is { } && pointHitTest.TryToGetPoint(cubic.Point3, target, radius, scale, registered) is { })
        {
            return cubic.Point3;
        }

        if (cubic.Point4 is { } && pointHitTest.TryToGetPoint(cubic.Point4, target, radius, scale, registered) is { })
        {
            return cubic.Point4;
        }

        return null;
    }

    public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not CubicBezierShapeViewModel cubic)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        _points.Clear();
        cubic.GetPoints(_points);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (cubic.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            return HitTestHelper.Contains(_points, target, scale);
        }
        else
        {
            return HitTestHelper.Contains(_points, target, 1.0);
        }
    }

    public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not CubicBezierShapeViewModel cubic)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        _points.Clear();
        cubic.GetPoints(_points);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (cubic.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            return HitTestHelper.Overlap(_points, target, scale);
        }
        else
        {
            return HitTestHelper.Overlap(_points, target, 1.0);
        }
    }
}
