// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    public class XLine : BaseShape
    {
        private ShapeStyle _style;
        private XPoint _start;
        private XPoint _end;

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
            renderer.Draw(dc, this, dx, dy);
            if (renderer.DrawPoints)
            {
                _start.Draw(dc, renderer, _start.X, _start.Y);
                _end.Draw(dc, renderer, _end.X, _end.Y);
            }
        }

        public static XLine Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point)
        {
            return new XLine()
            {
                Style = style,
                Start = XPoint.Create(x1, y1, point),
                End = XPoint.Create(x2, y2, point)
            };
        }

        public static XLine Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point)
        {
            return Create(x, y, x, y, style, point);
        }
    }
}
