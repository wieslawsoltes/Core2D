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
    public class XPoint : BaseShape
    {
        private BaseShape _shape;
        private double _x;
        private double _y;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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
            if (_shape != null)
            {
                if (State.HasFlag(ShapeState.Visible))
                {
                    _shape.Draw(dc, renderer, X + dx, Y + dy, db);
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
            X += dx;
            Y += dy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="shape"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XPoint Create(double x, double y, BaseShape shape, string name = "")
        {
            return new XPoint() 
            { 
                Name = name,
                Style = default(ShapeStyle),
                Properties = new ObservableCollection<ShapeProperty>(),
                X = x, 
                Y = y, 
                Shape = shape 
            };
        }
    }
}
