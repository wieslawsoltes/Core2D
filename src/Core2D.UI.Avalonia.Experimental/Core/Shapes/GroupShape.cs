// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core2D.Renderer;
using Core2D.Shape;

namespace Core2D.Shapes
{
    public class GroupShape : ConnectableShape, ICopyable
    {
        private ObservableCollection<BaseShape> _shapes;

        public ObservableCollection<BaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public GroupShape()
            : base()
        {
            _shapes = new ObservableCollection<BaseShape>();
        }

        public GroupShape(ObservableCollection<BaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public GroupShape(string name)
            : this()
        {
            this.Name = name;
        }

        public GroupShape(string name, ObservableCollection<BaseShape> shapes)
            : base()
        {
            this.Name = name;
            this.Shapes = shapes;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            foreach (var point in Points)
            {
                yield return point;
            }

            foreach (var shape in Shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public override bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            bool result = base.Invalidate(renderer, dx, dy);

            foreach (var point in Points)
            {
                result |= point.Invalidate(renderer, dx, dy);
            }

            foreach (var shape in Shapes)
            {
                result |= shape.Invalidate(renderer, dx, dy);
            }

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

            foreach (var shape in Shapes)
            {
                shape.Draw(dc, renderer, dx, dy, db, r);
            }

            base.Draw(dc, renderer, dx, dy, db, r);
            base.EndTransform(dc, renderer, state);
        }

        public override void Move(ISelection selection, double dx, double dy)
        {
            var points = GetPoints().Distinct();

            foreach (var point in points)
            {
                if (!selection.Selected.Contains(point))
                {
                    point.Move(selection, dx, dy);
                }
            }

            base.Move(selection, dx, dy);
        }

        public object Copy(IDictionary<object, object> shared)
        {
            var copy = new GroupShape()
            {
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared)
            };

            if (shared != null)
            {
                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }

                foreach (var shape in this.Shapes)
                {
                    if (shape is ICopyable copyable)
                    {
                        copy.Shapes.Add((BaseShape)copyable.Copy(shared));
                    }
                }
            }

            return copy;
        }
    }
}
