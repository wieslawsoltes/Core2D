// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds
{
    public abstract class HitTestBase
    {
        public abstract Type TargetType { get; }
        public abstract PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest);
        public abstract BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest);
        public abstract BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest);
    }
}
