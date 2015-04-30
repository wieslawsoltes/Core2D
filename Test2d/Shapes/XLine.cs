// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public class XLine : BaseShape
    {
        private XPoint _start;
        private XPoint _end;

        public XPoint Start
        {
            get { return _start; }
            set
            {
                if (value != _start)
                {
                    _start = value;
                    Notify("Start");
                }
            }
        }

        public XPoint End
        {
            get { return _end; }
            set
            {
                if (value != _end)
                {
                    _end = value;
                    Notify("End");
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
                    _start.Draw(dc, renderer, dx, dy);
                    _end.Draw(dc, renderer, dx, dy);
                }
                else if (_start == renderer.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy);
                }
                else if (_end == renderer.SelectedShape)
                {
                    _end.Draw(dc, renderer, dx, dy);
                }
            }
            
            if (renderer.SelectedShapes != null)
            {
                if (renderer.SelectedShapes.Contains(this))
                {
                    _start.Draw(dc, renderer, dx, dy);
                    _end.Draw(dc, renderer, dx, dy);
                }
            }
        }

        public override void Move(double dx, double dy)
        {
            Start.Move(dx, dy);
            End.Move(dx, dy);
        }

        public static XLine Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            string name = "")
        {
            return new XLine()
            {
                Name = name,
                Style = style,
                Start = XPoint.Create(x1, y1, point),
                End = XPoint.Create(x2, y2, point)
            };
        }

        public static XLine Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            string name = "")
        {
            return Create(x, y, x, y, style, point, name);
        }
    }
}
