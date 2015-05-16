// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

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
        private IList<ShapeProperty> _properties;

        /// <summary>
        /// Gets or sets shape name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    Notify("Name");
                }
            }
        }

        /// <summary>
        /// Gets or sets shape owner shape.
        /// </summary>
        public BaseShape Owner
        {
            get { return _owner; }
            set
            {
                if (value != _owner)
                {
                    _owner = value;
                    Notify("Owner");
                }
            }
        }

        /// <summary>
        /// Idicates shape state options.
        /// </summary>
        public ShapeState State
        {
            get { return _state; }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    Notify("State");
                }
            }
        }

        /// <summary>
        /// Get or sets shape drawing style.
        /// </summary>
        public ShapeStyle Style
        {
            get { return _style; }
            set
            {
                if (value != _style)
                {
                    _style = value;
                    Notify("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets a colletion ShapeProperty that will be used during drawing.
        /// </summary>
        public IList<ShapeProperty> Properties
        {
            get { return _properties; }
            set
            {
                if (value != _properties)
                {
                    _properties = value;
                    Notify("Properties");
                }
            }
        }

        /// <summary>
        /// Draw shape using current renderer.
        /// </summary>
        /// <param name="dc">The generic drawing context object</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="dx">The X axis draw position osffset.</param>
        /// <param name="dy">The Y axis draw position osffset.</param>
        /// <param name="db">The properties database used for binding.</param>
        public abstract void Draw(object dc, IRenderer renderer, double dx, double dy, IList<ShapeProperty> db);

        /// <summary>
        /// Move shape position using dx,dy offset.
        /// </summary>
        /// <param name="dx">The X axis position osffset.</param>
        /// <param name="dy">The Y axis position osffset.</param>
        public abstract void Move(double dx, double dy);
    }
}
