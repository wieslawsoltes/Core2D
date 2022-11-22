﻿#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes;

public class TextBounds : IBounds
{
    public Type TargetType => typeof(TextShapeViewModel);

    public PointShapeViewModel? TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not TextShapeViewModel text)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var pointHitTest = registered[typeof(PointShapeViewModel)];

        if (text.TopLeft is { } && pointHitTest.TryToGetPoint(text.TopLeft, target, radius, scale, registered) is { })
        {
            return text.TopLeft;
        }

        if (text.BottomRight is { } && pointHitTest.TryToGetPoint(text.BottomRight, target, radius, scale, registered) is { })
        {
            return text.BottomRight;
        }

        return null;
    }

    public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not TextShapeViewModel text)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (text.TopLeft is null || text.BottomRight is null)
        {
            return false;
        }

        var rect = Rect2.FromPoints(
            text.TopLeft.X,
            text.TopLeft.Y,
            text.BottomRight.X,
            text.BottomRight.Y);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (text.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
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
        if (shape is not TextShapeViewModel text)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (text.TopLeft is null || text.BottomRight is null)
        {
            return false;
        }

        var rect = Rect2.FromPoints(
            text.TopLeft.X,
            text.TopLeft.Y,
            text.BottomRight.X,
            text.BottomRight.Y);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (text.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            return HitTestHelper.Inflate(ref rect, scale).IntersectsWith(target);
        }
        else
        {
            return rect.IntersectsWith(target);
        }
    }
}
