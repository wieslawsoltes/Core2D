// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public class XPoint : BaseShape
    {
        private ShapeStyle _style;
        private BaseShape _shape;
        private double _x;
        private double _y;

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

        public BaseShape Shape
        {
            get { return _shape; }
            set
            {
                if (value != _shape)
                {
                    _shape = value;
                    Notify("Shape");
                }
            }
        }

        public double X
        {
            get { return _x; }
            set
            {
                if (value != _x)
                {
                    _x = value;
                    Notify("X");
                }
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (value != _y)
                {
                    _y = value;
                    Notify("Y");
                }
            }
        }

        public override void Draw(object dc, IRenderer renderer, double dx, double dy)
        {
            if (_shape != null)
            {
                if (State.HasFlag(ShapeState.Visible))
                {
                    _shape.Draw(dc, renderer, dx, dy);
                }
            }
        }

        public override void Move(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public static XPoint Create(double x, double y, BaseShape shape)
        {
            return new XPoint() { X = x, Y = y, Shape = shape };
        }
    }
}
