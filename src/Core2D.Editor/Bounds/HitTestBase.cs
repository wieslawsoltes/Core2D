// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes.Interfaces;
using Spatial;

namespace Core2D.Editor.Bounds
{
    public abstract class HitTestBase
    {
        public abstract Type TargetType { get; }
        public abstract IPointShape TryToGetPoint(IShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered);
        public abstract bool Contains(IShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered);
        public abstract bool Overlaps(IShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered);
    }
}
