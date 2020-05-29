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
        private List<IPointShape> _points = new List<IPointShape>();
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
        public override void DrawShape(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawPath(dc, this, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (renderer.State.SelectedShapes != null && renderer.State.DrawPoints == true)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _points.Clear();
                    GetPoints(_points);

                    foreach (var point in _points)
                    {
                        point.DrawShape(dc, renderer, dx, dy);
                    }
                }
                else
                {
                    _points.Clear();
                    GetPoints(_points);

                    foreach (var point in _points)
                    {
                        if (renderer.State.SelectedShapes.Contains(point))
                        {
                            point.DrawShape(dc, renderer, dx, dy);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            _points.Clear();
            GetPoints(_points);

            foreach (var point in _points)
            {
                point.Bind(dataFlow, db, record);
            }
        }

        /// <inheritdoc/>
        public override void Move(ISelection selection, double dx, double dy)
        {
            _points.Clear();
            GetPoints(_points);

            foreach (var point in _points)
            {
                point.Move(selection, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISelection selection)
        {
            base.Select(selection);

            _points.Clear();
            GetPoints(_points);

            foreach (var point in _points)
            {
                point.Select(selection);
            }
        }

        /// <inheritdoc/>
        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);

            _points.Clear();
            GetPoints(_points);

            foreach (var point in _points)
            {
                point.Deselect(selection);
            }
        }

        /// <inheritdoc/>
        public override void GetPoints(IList<IPointShape> points)
        {
            foreach (var figure in Geometry.Figures)
            {
                figure.GetPoints(points);
            }
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Geometry.IsDirty();

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();
            Geometry.Invalidate();
        }

        /// <inheritdoc/>
        public string ToXamlString()
            => Geometry?.ToXamlString();

        /// <inheritdoc/>
        public string ToSvgString()
            => Geometry?.ToSvgString();

        /// <summary>
        /// Check whether the <see cref="Geometry"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeGeometry() => _geometry != null;
    }
}
