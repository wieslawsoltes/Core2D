// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Base class for shapes.
    /// </summary>
    public abstract class BaseShape : ObservableObject, IBaseShape
    {
        private IBaseShape _owner;
        private IShapeState _state;
        private IShapeStyle _style;
        private IMatrixObject _transform;
        private bool _isStroked;
        private bool _isFilled;
        private IContext _data;

        /// <inheritdoc/>
        public abstract Type TargetType { get; }

        /// <inheritdoc/>
        public virtual IBaseShape Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        /// <inheritdoc/>
        public virtual IShapeState State
        {
            get => _state;
            set => Update(ref _state, value);
        }

        /// <inheritdoc/>
        public virtual IShapeStyle Style
        {
            get => _style;
            set => Update(ref _style, value);
        }

        /// <inheritdoc/>
        public IMatrixObject Transform
        {
            get => _transform;
            set => Update(ref _transform, value);
        }

        /// <inheritdoc/>
        public virtual bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        /// <inheritdoc/>
        public virtual bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        /// <inheritdoc/>
        public virtual IContext Data
        {
            get => _data;
            set => Update(ref _data, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseShape"/> class.
        /// </summary>
        public BaseShape()
            : base()
        {
            State = Factory.CreateShapeState(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone);
            Transform = Factory.CreateMatrixObject();
            Data = Factory.CreateContext();
        }

        /// <inheritdoc/>
        public virtual object BeginTransform(object dc, IShapeRenderer renderer)
        {
            if (Transform != null)
            {
                return renderer.PushMatrix(dc, Transform);
            }
            return null;
        }

        /// <inheritdoc/>
        public virtual void EndTransform(object dc, IShapeRenderer renderer, object state)
        {
            if (Transform != null)
            {
                renderer.PopMatrix(dc, state);
            }
        }

        /// <inheritdoc/>
        public abstract void Draw(object dc, IShapeRenderer renderer, double dx, double dy, object db, object r);

        /// <inheritdoc/>
        public virtual bool Invalidate(IShapeRenderer renderer, double dx, double dy)
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
