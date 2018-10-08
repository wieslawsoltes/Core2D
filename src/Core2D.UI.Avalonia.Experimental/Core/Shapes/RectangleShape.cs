// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    public class RectangleShape : BoxShape, ICopyable
    {
        public RectangleShape()
            : base()
        {
        }

        public RectangleShape(PointShape topLeft, PointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public override bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            bool result = base.Invalidate(renderer, dx, dy);

            if (this.IsDirty || result == true)
            {
                renderer.InvalidateCache(this, Style, dx, dy);
                this.IsDirty = false;
                result |= true;
            }

            return result;
        }

        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var state = base.BeginTransform(dc, renderer);

            if (Style != null)
            {
                renderer.DrawRectangle(dc, this, Style, dx, dy);
            }

            if (renderer.Selected.Contains(TopLeft))
            {
                TopLeft.Draw(dc, renderer, dx, dy, db, r);
            }

            if (renderer.Selected.Contains(BottomRight))
            {
                BottomRight.Draw(dc, renderer, dx, dy, db, r);
            }

            base.Draw(dc, renderer, dx, dy, db, r);
            base.EndTransform(dc, renderer, state);
        }

        public object Copy(IDictionary<object, object> shared)
        {
            var copy = new RectangleShape()
            {
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (PointShape)shared[this.TopLeft];
                copy.BottomRight = (PointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }
}
