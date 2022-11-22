﻿#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes;

public class ImageBounds : IBounds
{
    public Type TargetType => typeof(ImageShapeViewModel);

    public PointShapeViewModel? TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not ImageShapeViewModel image)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var pointHitTest = registered[typeof(PointShapeViewModel)];

        if (image.TopLeft is { } && pointHitTest.TryToGetPoint(image.TopLeft, target, radius, scale, registered) is { })
        {
            return image.TopLeft;
        }

        if (image.BottomRight is { } && pointHitTest.TryToGetPoint(image.BottomRight, target, radius, scale, registered) is { })
        {
            return image.BottomRight;
        }

        return null;
    }

    public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not ImageShapeViewModel image)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (image.TopLeft is null || image.BottomRight is null)
        {
            return false;
        }

        var rect = Rect2.FromPoints(
            image.TopLeft.X,
            image.TopLeft.Y,
            image.BottomRight.X,
            image.BottomRight.Y);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (image.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
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
        if (shape is not ImageShapeViewModel image)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (image.TopLeft is null || image.BottomRight is null)
        {
            return false;
        }

        var rect = Rect2.FromPoints(
            image.TopLeft.X,
            image.TopLeft.Y,
            image.BottomRight.X,
            image.BottomRight.Y);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (image.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            return HitTestHelper.Inflate(ref rect, scale).IntersectsWith(target);
        }
        else
        {
            return rect.IntersectsWith(target);
        }
    }
}
