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
            set { Update(ref _shape, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double X
        {
            get { return _x; }
            set { Update(ref _x, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Y
        {
            get { return _y; }
            set { Update(ref _y, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="renderer"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="db"></param>
        /// <param name="r"></param>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, IList<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Record;

            if (_shape != null)
            {
                if (State.HasFlag(ShapeState.Visible))
                {
                    _shape.Draw(dc, renderer, X + dx, Y + dy, db, record);
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
                Bindings = new ObservableCollection<ShapeBinding>(),
                Properties = new ObservableCollection<ShapeProperty>(),
                X = x, 
                Y = y, 
                Shape = shape 
            };
        }
    }
}
