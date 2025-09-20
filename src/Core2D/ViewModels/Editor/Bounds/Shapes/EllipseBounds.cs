// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes;

public class EllipseBounds : IBounds
{
    public Type TargetType => typeof(EllipseShapeViewModel);

    public PointShapeViewModel? TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not EllipseShapeViewModel ellipse)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var pointHitTest = registered[typeof(PointShapeViewModel)];

        if (ellipse.TopLeft is { } && pointHitTest.TryToGetPoint(ellipse.TopLeft, target, radius, scale, registered) is { })
        {
            return ellipse.TopLeft;
        }

        if (ellipse.BottomRight is { } && pointHitTest.TryToGetPoint(ellipse.BottomRight, target, radius, scale, registered) is { })
        {
            return ellipse.BottomRight;
        }

        return null;
    }

    public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not EllipseShapeViewModel ellipse)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (ellipse.TopLeft is null || ellipse.BottomRight is null)
        {
            return false;
        }

        var rect = Rect2.FromPoints(
            ellipse.TopLeft.X,
            ellipse.TopLeft.Y,
            ellipse.BottomRight.X,
            ellipse.BottomRight.Y);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (ellipse.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
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
        if (shape is not EllipseShapeViewModel ellipse)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (ellipse.TopLeft is null || ellipse.BottomRight is null)
        {
            return false;
        }

        var rect = Rect2.FromPoints(
            ellipse.TopLeft.X,
            ellipse.TopLeft.Y,
            ellipse.BottomRight.X,
            ellipse.BottomRight.Y);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (ellipse.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            return HitTestHelper.Inflate(ref rect, scale).IntersectsWith(target);
        }
        else
        {
            return rect.IntersectsWith(target);
        }
    }
}
