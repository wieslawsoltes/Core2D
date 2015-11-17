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
