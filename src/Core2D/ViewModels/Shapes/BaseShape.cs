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
        private IDictionary<string, object> _properties = new Dictionary<string, object>();
        private IShapeState _state;
        private IShapeStyle _style;
        private IMatrixObject _transform;
        private bool _isStroked;
        private bool _isFilled;
        private IContext _data;

        /// <inheritdoc/>
        public abstract Type TargetType { get; }

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
        public abstract void Draw(object dc, IShapeRenderer renderer, double dx, double dy);

        /// <inheritdoc/>
        public virtual bool Invalidate(IShapeRenderer renderer, double dx, double dy)
        {
            return false;
        }

        /// <inheritdoc/>
        public abstract void Bind(IDataFlow dataFlow, object db, object r);

        /// <inheritdoc/>
        public virtual void SetProperty(string name, object value)
        {
            _properties[name] = value;
        }

        /// <inheritdoc/>
        public virtual object GetProperty(string name)
        {
            if (_properties.ContainsKey(name))
            {
                return _properties[name];
            }

            return null;
        }

        /// <inheritdoc/>
        public abstract void Move(ISelection selection, double dx, double dy);

        /// <inheritdoc/>
        public virtual void Select(ISelection selection)
        {
            if (!selection.SelectedShapes.Contains(this))
            {
                selection.SelectedShapes.Add(this);
            }
        }

        /// <inheritdoc/>
        public virtual void Deselect(ISelection selection)
        {
            if (selection.SelectedShapes.Contains(this))
            {
                selection.SelectedShapes.Remove(this);
            }
        }

        /// <summary>
        /// Get all points in the shape.
        /// </summary>
        /// <returns>All points in the shape.</returns>
        public abstract IEnumerable<IPointShape> GetPoints();

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
