// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public class XBezier : BaseShape
    {
        private XPoint _point1;
        private XPoint _point2;
        private XPoint _point3;
        private XPoint _point4;
        private bool _isFilled;

        public XPoint Point1
        {
            get { return _point1; }
            set
            {
                if (value != _point1)
                {
                    _point1 = value;
                    Notify("Point1");
                }
            }
        }

        public XPoint Point2
        {
            get { return _point2; }
            set
            {
                if (value != _point2)
                {
                    _point2 = value;
                    Notify("Point2");
                }
            }
        }

        public XPoint Point3
        {
            get { return _point3; }
            set
            {
                if (value != _point3)
                {
                    _point3 = value;
                    Notify("Point3");
                }
            }
        }

        public XPoint Point4
        {
            get { return _point4; }
            set
            {
                if (value != _point4)
                {
                    _point4 = value;
                    Notify("Point4");
                }
            }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
            set
            {
                if (value != _isFilled)
                {
                    _isFilled = value;
                    Notify("IsFilled");
                }
            }
        }

        public override void Draw(object dc, IRenderer renderer, double dx, double dy)
        {
            if (State.HasFlag(ShapeState.Visible))
            {
                renderer.Draw(dc, this, dx, dy);
            }

            if (renderer.SelectedShape != null)
            {
                if (this == renderer.SelectedShape)
                {
                    _point1.Draw(dc, renderer, dx, dy);
                    _point2.Draw(dc, renderer, dx, dy);
                    _point3.Draw(dc, renderer, dx, dy);
                    _point4.Draw(dc, renderer, dx, dy);
                }
                else if (_point1 == renderer.SelectedShape)
                {
                    _point1.Draw(dc, renderer, dx, dy);
                }
                else if (_point2 == renderer.SelectedShape)
                {
                    _point2.Draw(dc, renderer, dx, dy);
                }
                else if (_point3 == renderer.SelectedShape)
                {
                    _point3.Draw(dc, renderer, dx, dy);
                }
                else if (_point4 == renderer.SelectedShape)
                {
                    _point4.Draw(dc, renderer, dx, dy);
                }
            }
            
            if (renderer.SelectedShapes != null)
            {
                if (renderer.SelectedShapes.Contains(this))
                {
                    _point1.Draw(dc, renderer, dx, dy);
                    _point2.Draw(dc, renderer, dx, dy);
                    _point3.Draw(dc, renderer, dx, dy);
                    _point4.Draw(dc, renderer, dx, dy);
                }
            }
        }

        public override void Move(double dx, double dy)
        {
            Point1.Move(dx, dy);
            Point2.Move(dx, dy);
            Point3.Move(dx, dy);
            Point4.Move(dx, dy);
        }

        public static XBezier Create(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4,
            ShapeStyle style,
            BaseShape point,
            bool isFilled = false,
            string name = "")
        {
            return new XBezier()
            {
                Name = name,
                Style = style,
                Point1 = XPoint.Create(x1, y1, point),
                Point2 = XPoint.Create(x2, y2, point),
                Point3 = XPoint.Create(x3, y3, point),
                Point4 = XPoint.Create(x4, y4, point),
                IsFilled = isFilled
            };
        }

        public static XBezier Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            bool isFilled = false,
            string name = "")
        {
            return Create(x, y, x, y, x, y, x, y, style, point, isFilled, name);
        }
    }
}
