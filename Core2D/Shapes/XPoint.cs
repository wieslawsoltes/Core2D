// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// Object representing point shape.
    /// </summary>
    public class XPoint : BaseShape
    {
        private BaseShape _shape;
        private double _x;
        private double _y;

        /// <summary>
        /// Gets or sets point template shape.
        /// </summary>
        public BaseShape Shape
        {
            get { return _shape; }
            set { Update(ref _shape, value); }
        }

        /// <summary>
        /// Gets or sets X coordinate of point.
        /// </summary>
        public double X
        {
            get { return _x; }
            set { Update(ref _x, value); }
        }

        /// <summary>
        /// Gets or sets Y coordinate of point.
        /// </summary>
        public double Y
        {
            get { return _y; }
            set { Update(ref _y, value); }
        }

        /// <summary>
        /// Try binding data record to one of <see cref="XPoint"/> shape properties.
        /// </summary>
        /// <param name="binding">The binding object used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        /// <param name="value">The output double value bound to data record.</param>
        private static void BindToDouble(Binding binding, Record r, ref double value)
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
        /// Try binding data record to one of <see cref="XPoint"/> shape properties.
        /// </summary>
        /// <param name="bindings">The bindings database used for binding.</param>
        /// <param name="r">The external data record used for binding.</param>
        /// <param name="propertyNameX">The target X property name.</param>
        /// <param name="propertyNameY">The target Y property name.</param>
        private void TryToBind(
            ImmutableArray<Binding> bindings,
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
        /// Try binding data record to <see cref="XPoint"/> shape property.
        /// </summary>
        /// <param name="propertyName">The target property name.</param>
        /// <param name="bindings">The bindings database used for binding.</param>
        /// <param name="record">The external data record used for binding.</param>
        public void TryToBind(string propertyName, ImmutableArray<Binding> bindings, Record record)
        {
            string propertyNameX = propertyName + ".X";
            string propertyNameY = propertyName + ".Y";
            TryToBind(bindings, record, propertyNameX, propertyNameY);
        }

        /// <inheritdoc/>
        public override void Bind(Record r)
        {
            var record = r ?? this.Data.Record;
            var bindings = this.Data.Bindings;
            string propertyNameX = "X";
            string propertyNameY = "Y";
            TryToBind(bindings, record, propertyNameX, propertyNameY);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<Property> db, Record r)
        {
            var record = r ?? this.Data.Record;

            if (_shape != null)
            {
                if (State.Flags.HasFlag(ShapeStateFlags.Visible))
                {
                    _shape.Draw(dc, renderer, X + dx, Y + dy, db, record);
                }
            }
        }

        /// <inheritdoc/>
        public override void Move(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        /// <summary>
        /// Creates a new <see cref="XPoint"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="shape">The point template.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XPoint"/> class.</returns>
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
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<Binding>(),
                    Properties = ImmutableArray.Create<Property>()
                },
                X = x,
                Y = y,
                Shape = shape
            };
        }

        /// <summary>
        /// Calculates distance between points.
        /// </summary>
        /// <param name="point">The other point</param>
        /// <returns>The distance between points.</returns>
        public double Distance(XPoint point)
        {
            double dx = this.X - point.X;
            double dy = this.Y - point.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
