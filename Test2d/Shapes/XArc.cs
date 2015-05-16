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
    public class XArc : BaseShape
    {
        private XPoint _point1;
        private XPoint _point2;
        private bool _isFilled;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="renderer"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
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
                }
                else if (_point1 == renderer.SelectedShape)
                {
                    _point1.Draw(dc, renderer, dx, dy);
                }
                else if (_point2 == renderer.SelectedShape)
                {
                    _point2.Draw(dc, renderer, dx, dy);
                }
            }
            
            if (renderer.SelectedShapes != null)
            {
                if (renderer.SelectedShapes.Contains(this))
                {
                    _point1.Draw(dc, renderer, dx, dy);
                    _point2.Draw(dc, renderer, dx, dy);
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
            Point1.Move(dx, dy);
            Point2.Move(dx, dy);
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
        /// <param name="isFilled"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XArc Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            bool isFilled = false,
            string name = "")
        {
            return new XArc()
            {
                Name = name,
                Style = style,
                Properties = new ObservableCollection<ShapeProperty>(),
                Point1 = XPoint.Create(x1, y1, point),
                Point2 = XPoint.Create(x2, y2, point),
                IsFilled = isFilled
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isFilled"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XArc Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            bool isFilled = false,
            string name = "")
        {
            return Create(x, y, x, y, style, point, isFilled, name);
        }
    }
}
