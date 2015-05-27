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
        private ImmutableArray<ShapeBinding> _bindings;
        private ImmutableArray<ShapeProperty> _properties;
        private Record _record;

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
        /// Idicates shape state options.
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
        /// Gets or sets a colletion ShapeBinding that will be used during drawing.
        /// </summary>
        public ImmutableArray<ShapeBinding> Bindings
        {
            get { return _bindings; }
            set { Update(ref _bindings, value); }
        }

        /// <summary>
        /// Gets or sets a colletion ShapeProperty that will be used during drawing.
        /// </summary>
        public ImmutableArray<ShapeProperty> Properties
        {
            get { return _properties; }
            set { Update(ref _properties, value); }
        }

        /// <summary>
        /// Gets or sets shape data record.
        /// </summary>
        public Record Record
        {
            get { return _record; }
            set { Update(ref _record, value); }
        }

        /// <summary>
        /// Draw shape using current renderer.
        /// </summary>
        /// <param name="dc">The generic drawing context object</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="dx">The X axis draw position osffset.</param>
        /// <param name="dy">The Y axis draw position osffset.</param>
        /// <param name="db">The properties database used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        public abstract void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r);

        /// <summary>
        /// Move shape position using dx,dy offset.
        /// </summary>
        /// <param name="dx">The X axis position osffset.</param>
        /// <param name="dy">The Y axis position osffset.</param>
        public abstract void Move(double dx, double dy);
    }
}
