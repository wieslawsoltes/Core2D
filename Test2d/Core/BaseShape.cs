// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public abstract class BaseShape : ObservableObject
    {
        private string _name;
        private BaseShape _owner;
        private ShapeState _state = ShapeState.Visible | ShapeState.Printable | ShapeState.Standalone;
        private ShapeStyle _style;
        private ShapeProperty[] _properties;

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

        public ShapeProperty[] Properties
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

        public abstract void Draw(object dc, IRenderer renderer, double dx, double dy);

        public abstract void Move(double dx, double dy);
    }
}
