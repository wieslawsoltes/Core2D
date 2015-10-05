// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Test2d
{
    /// <summary>
    /// Base class for Test2d shapes.
    /// </summary>
    public abstract class BaseShape : ObservableObject
    {
        private string _name;
        private BaseShape _owner;
        private ShapeState _state = ShapeState.Visible | ShapeState.Printable | ShapeState.Standalone;
        private ShapeStyle _style;
        private bool _isStroked;
        private bool _isFilled;
        private Data _data;

        /// <summary>
        /// Gets or sets shape name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets shape owner shape.
        /// </summary>
        public BaseShape Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        /// <summary>
        /// Indicates shape state options.
        /// </summary>
        public ShapeState State
        {
            get { return _state; }
            set { Update(ref _state, value); }
        }

        /// <summary>
        /// Get or sets shape drawing style.
        /// </summary>
        public ShapeStyle Style
        {
            get { return _style; }
            set { Update(ref _style, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStroked
        {
            get { return _isStroked; }
            set { Update(ref _isStroked, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFilled
        {
            get { return _isFilled; }
            set { Update(ref _isFilled, value); }
        }

        /// <summary>
        /// Gets or sets shape data.
        /// </summary>
        public Data Data
        {
            get { return _data; }
            set { Update(ref _data, value); }
        }

        /// <summary>
        /// Binding shape properties to Record data.
        /// </summary>
        /// <param name="r">The external data record used for binding.</param>
        public abstract void Bind(Record r);

        /// <summary>
        /// Draw shape using current renderer.
        /// </summary>
        /// <param name="dc">The generic drawing context object</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="dx">The X axis draw position offset.</param>
        /// <param name="dy">The Y axis draw position offset.</param>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        public abstract void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// Move shape position using dx,dy offset.
        /// </summary>
        /// <param name="dx">The X axis position offset.</param>
        /// <param name="dy">The Y axis position offset.</param>
        public abstract void Move(double dx, double dy);
    }
}
