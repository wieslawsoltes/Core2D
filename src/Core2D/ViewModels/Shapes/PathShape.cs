using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Data;
using Core2D.Path;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Path shape.
    /// </summary>
    public class PathShape : BaseShape, IPathShape
    {
        private IPathGeometry _geometry;

        /// <inheritdoc/>
        public override Type TargetType => typeof(IPathShape);

        /// <inheritdoc/>
        public IPathGeometry Geometry
        {
            get => _geometry;
            set => Update(ref _geometry, value);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            var state = base.BeginTransform(dc, renderer);

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy);
            }

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    var points = GetPoints();
                    foreach (var point in points)
                    {
                        point.Draw(dc, renderer, dx, dy);
                    }
                }
                else
                {
                    var points = GetPoints();
                    foreach (var point in points)
                    {
                        if (point == renderer.State.SelectedShape)
                        {
                            point.Draw(dc, renderer, dx, dy);
                        }
                    }
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    var points = GetPoints();
                    foreach (var point in points)
                    {
                        point.Draw(dc, renderer, dx, dy);
                    }
                }
            }

            base.EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            var points = GetPoints();

            foreach (var point in points)
            {
                point.Bind(dataFlow, db, record);
            }
        }

        /// <inheritdoc/>
        public override void Move(ISelection selection, double dx, double dy)
        {
            var points = GetPoints();
            foreach (var point in points)
            {
                point.Move(selection, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISelection selection)
        {
            base.Select(selection);

            var points = GetPoints();
            foreach (var point in points)
            {
                point.Select(selection);
            }
        }

        /// <inheritdoc/>
        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);

            var points = GetPoints();
            foreach (var point in points)
            {
                point.Deselect(selection);
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            return Geometry.Figures.SelectMany(f => f.GetPoints());
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="Geometry"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeGeometry() => _geometry != null;
    }
}
