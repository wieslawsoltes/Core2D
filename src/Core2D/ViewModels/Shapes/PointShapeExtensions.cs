// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using static System.Math;

namespace Core2D.ViewModels.Shapes;

public static class PointShapeExtensions
{
    public static double DistanceTo(this PointShapeViewModel point, PointShapeViewModel other)
    {
        var dx = point.X - other.X;
        var dy = point.Y - other.Y;
        // ReSharper disable once ArrangeRedundantParentheses
        return Sqrt((dx * dx) + (dy * dy));
    }
}
