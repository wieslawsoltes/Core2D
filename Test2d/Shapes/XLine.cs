// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XLine : BaseShape
    {
        private XPoint _start;
        private XPoint _end;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="renderer"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, IList<ShapeProperty> db)
        {
            if (State.HasFlag(ShapeState.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db); 
            }

            if (renderer.SelectedShape != null)
            {
                if (this == renderer.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy, db);
                    _end.Draw(dc, renderer, dx, dy, db);
                }
                else if (_start == renderer.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy, db);
                }
                else if (_end == renderer.SelectedShape)
                {
                    _end.Draw(dc, renderer, dx, dy, db);
                }
            }
            
            if (renderer.SelectedShapes != null)
            {
                if (renderer.SelectedShapes.Contains(this))
                {
                    _start.Draw(dc, renderer, dx, dy, db);
                    _end.Draw(dc, renderer, dx, dy, db);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public override void Move(double dx, double dy)
        {
            Start.Move(dx, dy);
            End.Move(dx, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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
                Properties = new ObservableCollection<ShapeProperty>(),
                Start = XPoint.Create(x1, y1, point),
                End = XPoint.Create(x2, y2, point)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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
