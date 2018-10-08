// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds
{
    public interface IHitTest
    {
        Dictionary<Type, HitTestBase> Registered { get; set; }
        void Register(HitTestBase hitTest);
        PointShape TryToGetPoint(IEnumerable<BaseShape> shapes, Point2 target, double radius, PointShape exclude);
        PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius);
        BaseShape TryToGetShape(IEnumerable<BaseShape> shapes, Point2 target, double radius);
        ISet<BaseShape> TryToGetShapes(IEnumerable<BaseShape> shapes, Rect2 target, double radius);
    }
}
