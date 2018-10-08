// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using Core2D.Editor.Bounds;

namespace Core2D.Editor
{
    public interface IToolContext
    {
        ShapeRenderer Renderer { get; set; }
        IHitTest HitTest { get; set; }
        ILayerContainer CurrentContainer { get; set; }
        ILayerContainer WorkingContainer { get; set; }
        ShapeStyle CurrentStyle { get; set; }
        BaseShape PointShape { get; set; }
        Action Capture { get; set; }
        Action Release { get; set; }
        Action Invalidate { get; set; }
        PointShape GetNextPoint(double x, double y, bool connect, double radius);
    }
}
