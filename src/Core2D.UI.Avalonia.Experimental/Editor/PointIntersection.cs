// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor
{
    public abstract class PointIntersection
    {
        public abstract string Title { get; }
        public List<PointShape> Intersections { get; set; }

        protected PointIntersection()
        {
            Intersections = new List<PointShape>();
        }

        public abstract void Find(IToolContext context, BaseShape shape);

        public virtual void Clear(IToolContext context)
        {
            foreach (var point in Intersections)
            {
                context.WorkingContainer.Shapes.Remove(point);
                context.Renderer.Selected.Remove(point);
            }
            Intersections.Clear();
        }
    }
}
