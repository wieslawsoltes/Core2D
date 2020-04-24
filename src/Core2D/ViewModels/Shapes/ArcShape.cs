using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Arc shape.
    /// </summary>
    public class ArcShape : BaseShape, IArcShape
    {
        private IPointShape _point1;
        private IPointShape _point2;
        private IPointShape _point3;
        private IPointShape _point4;

        /// <inheritdoc/>
        public override Type TargetType => typeof(IArcShape);

        /// <inheritdoc/>
        public IPointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        /// <inheritdoc/>
        public IPointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        /// <inheritdoc/>
        public IPointShape Point3
        {
            get => _point3;
            set => Update(ref _point3, value);
        }

        /// <inheritdoc/>
        public IPointShape Point4
        {
            get => _point4;
            set => Update(ref _point4, value);
        }

        /// <inheritdoc/>
        public override void DrawShape(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (renderer.State.SelectedShape != null && renderer.State.DrawPoints == true)
            {
                if (this == renderer.State.SelectedShape)
                {
                    _point1.DrawShape(dc, renderer, dx, dy);
                    _point2.DrawShape(dc, renderer, dx, dy);
                    _point3.DrawShape(dc, renderer, dx, dy);
                    _point4.DrawShape(dc, renderer, dx, dy);
                }
                else if (_point1 == renderer.State.SelectedShape)
                {
                    _point1.DrawShape(dc, renderer, dx, dy);
                }
                else if (_point2 == renderer.State.SelectedShape)
                {
                    _point2.DrawShape(dc, renderer, dx, dy);
                }
                else if (_point3 == renderer.State.SelectedShape)
                {
                    _point3.DrawShape(dc, renderer, dx, dy);
                }
                else if (_point4 == renderer.State.SelectedShape)
                {
                    _point4.DrawShape(dc, renderer, dx, dy);
                }
            }

            if (renderer.State.SelectedShapes != null && renderer.State.DrawPoints == true)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _point1.DrawShape(dc, renderer, dx, dy);
                    _point2.DrawShape(dc, renderer, dx, dy);
                    _point3.DrawShape(dc, renderer, dx, dy);
                    _point4.DrawShape(dc, renderer, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            _point1.Bind(dataFlow, db, record);
            _point2.Bind(dataFlow, db, record);
            _point3.Bind(dataFlow, db, record);
            _point4.Bind(dataFlow, db, record);
        }

        /// <inheritdoc/>
        public override void Move(ISelection selection, double dx, double dy)
        {
            if (!Point1.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point1.Move(selection, dx, dy);
            }

            if (!Point2.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point2.Move(selection, dx, dy);
            }

            if (!Point3.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point3.Move(selection, dx, dy);
            }

            if (!Point4.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Point4.Move(selection, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISelection selection)
        {
            base.Select(selection);
            Point1.Select(selection);
            Point2.Select(selection);
            Point3.Select(selection);
            Point4.Select(selection);
        }

        /// <inheritdoc/>
        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
            Point1.Deselect(selection);
            Point2.Deselect(selection);
            Point3.Deselect(selection);
            Point4.Deselect(selection);
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            yield return Point1;
            yield return Point2;
            yield return Point3;
            yield return Point4;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="Point1"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint1() => _point1 != null;

        /// <summary>
        /// Check whether the <see cref="Point2"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint2() => _point2 != null;

        /// <summary>
        /// Check whether the <see cref="Point3"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint3() => _point3 != null;

        /// <summary>
        /// Check whether the <see cref="Point4"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint4() => _point4 != null;
    }
}
