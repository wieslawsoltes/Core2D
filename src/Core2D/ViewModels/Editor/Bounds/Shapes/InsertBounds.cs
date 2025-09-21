// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes;

public class InsertBounds : IBounds
{
    public Type TargetType => typeof(InsertShapeViewModel);

    public PointShapeViewModel? TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not InsertShapeViewModel insert)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var pointHitTest = registered[typeof(PointShapeViewModel)];

        foreach (var p in insert.Connectors.Reverse())
        {
            if (pointHitTest.TryToGetPoint(p, target, radius, scale, registered) is { })
            {
                return p;
            }
        }

        return null;
    }

    public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not InsertShapeViewModel insert)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var block = insert.Block;
        if (block is null)
        {
            return false;
        }

        var dx = insert.Point?.X ?? 0.0;
        var dy = insert.Point?.Y ?? 0.0;
        var localTarget = new Point2(target.X - dx, target.Y - dy);
        var hasSize = insert.State.HasFlag(ShapeStateFlags.Size);

        foreach (var s in block.Shapes.Reverse())
        {
            var hitTest = registered[s.TargetType];
            var result = hitTest.Contains(s, localTarget, radius, hasSize ? scale : 1.0, registered);
            if (result)
            {
                return true;
            }
        }
        return false;
    }

    public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not InsertShapeViewModel insert)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var block = insert.Block;
        if (block is null)
        {
            return false;
        }

        var dx = insert.Point?.X ?? 0.0;
        var dy = insert.Point?.Y ?? 0.0;
        var local = new Rect2(target.X - dx, target.Y - dy, target.Width, target.Height);
        var hasSize = insert.State.HasFlag(ShapeStateFlags.Size);

        foreach (var s in block.Shapes.Reverse())
        {
            var hitTest = registered[s.TargetType];
            var result = hitTest.Overlaps(s, local, radius, hasSize ? scale : 1.0, registered);
            if (result)
            {
                return true;
            }
        }
        return false;
    }
}

