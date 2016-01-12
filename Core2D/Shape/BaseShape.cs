// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// Base class for shapes.
    /// </summary>
    [RuntimeNameProperty(nameof(Name))]
    public abstract class BaseShape : ObservableResource
    {
        private string _name;
        private BaseShape _owner;
        private ShapeState _state = ShapeState.Create(ShapeStateFlags.Visible | ShapeStateFlags.Printable | ShapeStateFlags.Standalone);
        private ShapeStyle _style;
        private bool _isStroked;
        private bool _isFilled;
        private Data _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseShape"/> class.
        /// </summary>
        public BaseShape()
            : base()
        {
            Data = new Data();
        }

        /// <summary>
        /// Gets or sets shape name.
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets shape owner.
        /// </summary>
        public virtual BaseShape Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        /// <summary>
        /// Indicates shape state options.
        /// </summary>
        public virtual ShapeState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        /// <summary>
        /// Get or sets shape drawing style.
        /// </summary>
        public virtual ShapeStyle Style
        {
            get { return _style; }
            set { Update(ref _style, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating whether shape is stroked.
        /// </summary>
        public virtual bool IsStroked
        {
            get { return _isStroked; }
            set { Update(ref _isStroked, value); }
        }

        /// <summary>
        /// Gets or sets flag indicating whether shape is filled.
        /// </summary>
        public virtual bool IsFilled
        {
            get { return _isFilled; }
            set { Update(ref _isFilled, value); }
        }

        /// <summary>
        /// Gets or sets shape <see cref="Core2D.Data"/>.
        /// </summary>
        public virtual Data Data
        {
            get { return _data; }
            set { Update(ref _data, value); }
        }

        /// <summary>
        /// Draws shape using current <see cref="Renderer"/>.
        /// </summary>
        /// <param name="dc">The generic drawing context object</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="dx">The X axis draw position offset.</param>
        /// <param name="dy">The Y axis draw position offset.</param>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        public abstract void Draw(object dc, Renderer renderer, double dx, double dy, ImmutableArray<Property> db, Record r);

        /// <summary>
        /// Moves shape to new position using X and Y axis offset.
        /// </summary>
        /// <param name="dx">The X axis position offset.</param>
        /// <param name="dy">The Y axis position offset.</param>
        public abstract void Move(double dx, double dy);

        /// <summary>
        /// Get all points in the shape.
        /// </summary>
        /// <returns>All points in the shape.</returns>
        public abstract IEnumerable<XPoint> GetPoints();
    }
}
