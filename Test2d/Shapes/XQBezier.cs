// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XQBezier : BaseShape
    {
        private XPoint _point1;
        private XPoint _point2;
        private XPoint _point3;

        /// <summary>
        /// 
        /// </summary>
        public XPoint Point1
        {
            get { return _point1; }
            set { Update(ref _point1, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public XPoint Point2
        {
            get { return _point2; }
            set { Update(ref _point2, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public XPoint Point3
        {
            get { return _point3; }
            set { Update(ref _point3, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void Bind(Record r)
        {
            var record = r ?? this.Record;
            _point1.TryToBind("Point1", this.Bindings, record);
            _point2.TryToBind("Point2", this.Bindings, record);
            _point3.TryToBind("Point3", this.Bindings, record);
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
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<ShapeProperty> db, Record r)
        {
            var record = r ?? this.Record;

            if (State.HasFlag(ShapeState.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record);
            }

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    _point1.Draw(dc, renderer, dx, dy, db, record);
                    _point2.Draw(dc, renderer, dx, dy, db, record);
                    _point3.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_point1 == renderer.State.SelectedShape)
                {
                    _point1.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_point2 == renderer.State.SelectedShape)
                {
                    _point2.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_point3 == renderer.State.SelectedShape)
                {
                    _point3.Draw(dc, renderer, dx, dy, db, record);
                }
            }
            
            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _point1.Draw(dc, renderer, dx, dy, db, record);
                    _point2.Draw(dc, renderer, dx, dy, db, record);
                    _point3.Draw(dc, renderer, dx, dy, db, record);
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
            if (!Point1.State.HasFlag(ShapeState.Connector))
            {
                Point1.Move(dx, dy);
            }

            if (!Point2.State.HasFlag(ShapeState.Connector))
            {
                Point2.Move(dx, dy);
            }

            if (!Point3.State.HasFlag(ShapeState.Connector))
            {
                Point3.Move(dx, dy);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XQBezier Create(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            ShapeStyle style,
            BaseShape point,
            bool isStroked = true,
            bool isFilled = false,
            string name = "")
        {
            return new XQBezier()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Bindings = ImmutableArray.Create<ShapeBinding>(),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                Point1 = XPoint.Create(x1, y1, point),
                Point2 = XPoint.Create(x2, y2, point),
                Point3 = XPoint.Create(x3, y3, point)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XQBezier Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            bool isStroked = true,
            bool isFilled = false,
            string name = "")
        {
            return Create(x, y, x, y, x, y, style, point, isStroked, isFilled, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XQBezier Create(
            XPoint point1,
            XPoint point2,
            XPoint point3,
            ShapeStyle style,
            BaseShape point,
            bool isStroked = true,
            bool isFilled = false,
            string name = "")
        {
            return new XQBezier()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Bindings = ImmutableArray.Create<ShapeBinding>(),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                Point1 = point1,
                Point2 = point2,
                Point3 = point3
            };
        }
    }
}
