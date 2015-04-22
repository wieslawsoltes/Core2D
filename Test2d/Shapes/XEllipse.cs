// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public class XEllipse : BaseShape
    {
        private ShapeStyle _style;
        private XPoint _topLeft;
        private XPoint _bottomRight;
        private bool _isFilled;

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

        public XPoint TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (value != _topLeft)
                {
                    _topLeft = value;
                    Notify("TopLeft");
                }
            }
        }

        public XPoint BottomRight
        {
            get { return _bottomRight; }
            set
            {
                if (value != _bottomRight)
                {
                    _bottomRight = value;
                    Notify("BottomRight");
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

            if (renderer.DrawPoints)
            {
                _topLeft.Draw(dc, renderer, _topLeft.X, _topLeft.Y);
                _bottomRight.Draw(dc, renderer, _bottomRight.X, _bottomRight.Y);
            }
        }

        public override void Move(double dx, double dy)
        {
            TopLeft.X += dx;
            TopLeft.Y += dy;
            BottomRight.X += dx;
            BottomRight.Y += dy;
        }

        public static XEllipse Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            bool isFilled = false)
        {
            return new XEllipse()
            {
                Style = style,
                TopLeft = XPoint.Create(x1, y1, point),
                BottomRight = XPoint.Create(x2, y2, point),
                IsFilled = isFilled
            };
        }

        public static XEllipse Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            bool isFilled = false)
        {
            return Create(x, y, x, y, style, point, isFilled);
        }
    }
}
