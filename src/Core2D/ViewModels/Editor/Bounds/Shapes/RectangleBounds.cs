﻿#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes;

public class RectangleBounds : IBounds
{
    public Type TargetType => typeof(RectangleShapeViewModel);

    public PointShapeViewModel? TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not RectangleShapeViewModel rectangle)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var pointHitTest = registered[typeof(PointShapeViewModel)];

        if (rectangle.TopLeft is { } && pointHitTest.TryToGetPoint(rectangle.TopLeft, target, radius, scale, registered) is { })
        {
            return rectangle.TopLeft;
        }

        if (rectangle.BottomRight is { } && pointHitTest.TryToGetPoint(rectangle.BottomRight, target, radius, scale, registered) is { })
        {
            return rectangle.BottomRight;
        }

        return null;
    }

    public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not RectangleShapeViewModel rectangle)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (rectangle.TopLeft is null || rectangle.BottomRight is null)
        {
            return false;
        }

        var rect = Rect2.FromPoints(
            rectangle.TopLeft.X,
            rectangle.TopLeft.Y,
            rectangle.BottomRight.X,
            rectangle.BottomRight.Y);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (rectangle.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            return HitTestHelper.Inflate(ref rect, scale).Contains(target);
        }
        else
        {
            return rect.Contains(target);
        }
    }

    public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not RectangleShapeViewModel rectangle)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (rectangle.TopLeft is null || rectangle.BottomRight is null)
        {
            return false;
        }

        var rect = Rect2.FromPoints(
            rectangle.TopLeft.X,
            rectangle.TopLeft.Y,
            rectangle.BottomRight.X,
            rectangle.BottomRight.Y);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (rectangle.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            return HitTestHelper.Inflate(ref rect, scale).IntersectsWith(target);
        }
        else
        {
            return rect.IntersectsWith(target);
        }
    }
}
