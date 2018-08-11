// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Base class for shapes.
    /// </summary>
    public abstract class BaseShape : ObservableObject, IBaseShape
    {
        private IBaseShape _owner;
        private ShapeState _state;
        private ShapeStyle _style;
        private MatrixObject _transform;
        private bool _isStroked;
        private bool _isFilled;
        private Context _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseShape"/> class.
        /// </summary>
        public BaseShape()
            : base()
        {
            State = ShapeState.Create(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone);
            Transform = MatrixObject.Create();
            Data = Context.Create();
        }

        /// <summary>
        /// Gets or sets shape owner.
        /// </summary>
        public virtual IBaseShape Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <summary>
        /// Indicates shape state options.
        /// </summary>
        public virtual ShapeState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <summary>
        /// Get or sets shape drawing style.
        /// </summary>
        public virtual ShapeStyle Style
        {
            get => _style;
            set => Update(ref _style, value);
        }

        /// <summary>
        /// Get or sets shape matrix transform.
        /// </summary>
        public MatrixObject Transform
        {
            get => _transform;
            set => Update(ref _transform, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether shape is stroked.
        /// </summary>
        public virtual bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether shape is filled.
        /// </summary>
        public virtual bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        /// <summary>
        /// Gets or sets shape <see cref="Context"/>.
        /// </summary>
        public virtual Context Data
        {
            get => _data;
            set => Update(ref _data, value);
        }

        /// <inheritdoc/>
        public virtual object BeginTransform(object dc, ShapeRenderer renderer)
        {
            if (Transform != null)
            {
                return renderer.PushMatrix(dc, Transform);
            }
            return null;
        }

        /// <inheritdoc/>
        public virtual void EndTransform(object dc, ShapeRenderer renderer, object state)
        {
            if (Transform != null)
            {
                renderer.PopMatrix(dc, state);
            }
        }

        /// <inheritdoc/>
        public abstract void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public virtual bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            return false;
        }

        /// <inheritdoc/>
        public abstract void Move(ISet<IBaseShape> selected, double dx, double dy);

        /// <inheritdoc/>
        public virtual void Select(ISet<IBaseShape> selected)
        {
            if (!selected.Contains(this))
            {
                selected.Add(this);
            }
        }

        /// <inheritdoc/>
        public virtual void Deselect(ISet<IBaseShape> selected)
        {
            if (selected.Contains(this))
            {
                selected.Remove(this);
            }
        }

        /// <inheritdoc/>
        public abstract object Copy(IDictionary<object, object> shared);

        /// <summary>
        /// Get all points in the shape.
        /// </summary>
        /// <returns>All points in the shape.</returns>
        public abstract IEnumerable<IPointShape> GetPoints();

        /// <summary>
        /// Check whether the <see cref="Owner"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOwner() => _owner != null;

        /// <summary>
        /// Check whether the <see cref="State"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeState() => _state != null;

        /// <summary>
        /// Check whether the <see cref="Style"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStyle() => _style != null;

        /// <summary>
        /// Check whether the <see cref="Transform"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTransform() => _transform != null;

        /// <summary>
        /// Check whether the <see cref="IsStroked"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsStroked() => _isStroked != default;

        /// <summary>
        /// Check whether the <see cref="IsFilled"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsFilled() => _isFilled != default;

        /// <summary>
        /// Check whether the <see cref="Data"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeData() => _data != null;
    }
}
