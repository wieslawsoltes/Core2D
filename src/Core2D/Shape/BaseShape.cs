// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Shape
{
    /// <summary>
    /// Base class for shapes.
    /// </summary>
    public abstract class BaseShape : ObservableObject, ISelectable
    {
        private BaseShape _owner;
        private ShapeState _state;
        private ShapeStyle _style;
        private MatrixObject _transform;
        private bool _isStroked;
        private bool _isFilled;
        private XContext _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseShape"/> class.
        /// </summary>
        public BaseShape()
            : base()
        {
            State = ShapeState.Create(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone);
            Transform = MatrixObject.Create();
            Data = XContext.Create();
        }

        /// <summary>
        /// Gets or sets shape owner.
        /// </summary>
        public virtual BaseShape Owner
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
        /// Gets or sets shape <see cref="Core2D.Data"/>.
        /// </summary>
        public virtual XContext Data
        {
            get => _data;
            set => Update(ref _data, value);
        }

        /// <summary>
        /// Begins matrix transform.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <returns>The previous transform state.</returns>
        public virtual object BeginTransform(object dc, ShapeRenderer renderer)
        {
            if (Transform != null)
            {
                return renderer.PushMatrix(dc, Transform);
            }
            return null;
        }

        /// <summary>
        /// Ends matrix transform.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="state">The previous transform state.</param>
        public virtual void EndTransform(object dc, ShapeRenderer renderer, object state)
        {
            if (Transform != null)
            {
                renderer.PopMatrix(dc, state);
            }
        }

        /// <summary>
        /// Draws shape using current <see cref="ShapeRenderer"/>.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="dx">The X axis draw position offset.</param>
        /// <param name="dy">The Y axis draw position offset.</param>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        public abstract void Draw(object dc, ShapeRenderer renderer, double dx, double dy, ImmutableArray<XProperty> db, XRecord r);

        /// <summary>
        /// Moves shape to new position using X and Y axis offset.
        /// </summary>
        /// <param name="selected">The selected shapes set.</param>
        /// <param name="dx">The X axis position offset.</param>
        /// <param name="dy">The Y axis position offset.</param>
        public abstract void Move(ISet<BaseShape> selected, double dx, double dy);

        /// <summary>
        /// Selects the shape.
        /// </summary>
        /// <param name="selected">The selected shapes set.</param>
        public virtual void Select(ISet<BaseShape> selected)
        {
            if (!selected.Contains(this))
            {
                selected.Add(this);
            }
        }

        /// <summary>
        /// Deselects the shape.
        /// </summary>
        /// <param name="selected">The selected shapes set.</param>
        public virtual void Deselect(ISet<BaseShape> selected)
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
        public abstract IEnumerable<XPoint> GetPoints();

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
        public virtual bool ShouldSerializeIsStroked() => _isStroked != default(bool);

        /// <summary>
        /// Check whether the <see cref="IsFilled"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsFilled() => _isFilled != default(bool);

        /// <summary>
        /// Check whether the <see cref="Data"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeData() => _data != null;
    }
}
