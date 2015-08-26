// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Globalization;
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
        /// <param name="binding"></param>
        /// <param name="r"></param>
        /// <param name="value"></param>
        private static void BindToDouble(ShapeBinding binding, Record r, ref double value)
        {
            var columns = r.Columns;
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Name != binding.Path)
                    continue;

                double result;
                bool success = double.TryParse(
                    r.Values[i].Content,
                    NumberStyles.Any, CultureInfo.InvariantCulture,
                    out result);
                if (success)
                {
                    value = result;
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="r"></param>
        /// <param name="propertyNameX"></param>
        /// <param name="propertyNameY"></param>
        private void TryToBind(
            ImmutableArray<ShapeBinding> bindings,
            Record r,
            string propertyNameX,
            string propertyNameY)
        {
            if (r == null || bindings == null || bindings.Length <= 0)
                return;

            if (r.Columns == null || r.Values == null || r.Columns.Length != r.Values.Length)
                return;

            foreach (var binding in bindings)
            {
                if (string.IsNullOrEmpty(binding.Property) || string.IsNullOrEmpty(binding.Path))
                    continue;

                if (binding.Property == propertyNameX)
                {
                    BindToDouble(binding, r, ref _x);
                }
                else if (binding.Property == propertyNameY)
                {
                    BindToDouble(binding, r, ref _y);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="bindings"></param>
        /// <param name="record"></param>
        public void TryToBind(string propertyName, ImmutableArray<ShapeBinding> bindings, Record record)
        {
            string propertyNameX = propertyName + ".X";
            string propertyNameY = propertyName + ".Y";
            TryToBind(bindings, record, propertyNameX, propertyNameY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void Bind(Record r)
        {
            var record = r ?? this.Record;
            var bindings = this.Bindings;
            string propertyNameX = "X";
            string propertyNameY = "Y";
            TryToBind(bindings, record, propertyNameX, propertyNameY);
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
        public static XPoint Create(
            double x = 0.0, 
            double y = 0.0, 
            BaseShape shape = null, 
            string name = "")
        {
            return new XPoint() 
            { 
                Name = name,
                Style = default(ShapeStyle),
                Bindings = ImmutableArray.Create<ShapeBinding>(),
                Properties = ImmutableArray.Create<ShapeProperty>(),
                X = x, 
                Y = y, 
                Shape = shape 
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Distance(XPoint point)
        {
            double dx = this.X - point.X;
            double dy = this.Y - point.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
