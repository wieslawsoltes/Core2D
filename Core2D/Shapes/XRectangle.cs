// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class XRectangle : XText
    {
        private bool _isGrid;
        private double _offsetX;
        private double _offsetY;
        private double _cellWidth;
        private double _cellHeight;

        /// <summary>
        /// 
        /// </summary>
        public bool IsGrid
        {
            get { return _isGrid; }
            set { Update(ref _isGrid, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double OffsetX
        {
            get { return _offsetX; }
            set { Update(ref _offsetX, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double OffsetY
        {
            get { return _offsetY; }
            set { Update(ref _offsetY, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double CellWidth
        {
            get { return _cellWidth; }
            set { Update(ref _cellWidth, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double CellHeight
        {
            get { return _cellHeight; }
            set { Update(ref _cellHeight, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void Bind(Record r)
        {
            base.Bind(r ?? this.Data.Record);
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
        public override void Draw(object dc, IRenderer renderer, double dx, double dy, ImmutableArray<Property> db, Record r)
        {
            var record = r ?? this.Data.Record;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record);
                base.Draw(dc, renderer, dx, dy, db, record);
            }
        }

        /// <summary>
        /// Creates a new <see cref="XRectangle"/> instance.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XRectangle Create(
            double x1, double y1,
            double x2, double y2,
            ShapeStyle style,
            BaseShape point,
            bool isStroked = true,
            bool isFilled = false,
            string text = null,
            string name = "")
        {
            return new XRectangle()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<Binding>(),
                    Properties = ImmutableArray.Create<Property>()
                },
                TopLeft = XPoint.Create(x1, y1, point),
                BottomRight = XPoint.Create(x2, y2, point),
                Text = text,
                IsGrid = false,
                OffsetX = 30.0,
                OffsetY = 30.0,
                CellWidth = 30.0,
                CellHeight = 30.0
            };
        }

        /// <summary>
        /// Creates a new <see cref="XRectangle"/> instance.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XRectangle Create(
            double x, double y,
            ShapeStyle style,
            BaseShape point,
            bool isStroked = true,
            bool isFilled = false,
            string text = null,
            string name = "")
        {
            return Create(x, y, x, y, style, point, isStroked, isFilled, text, name);
        }

        /// <summary>
        /// Creates a new <see cref="XRectangle"/> instance.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <param name="style"></param>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="isFilled"></param>
        /// <param name="text"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XRectangle Create(
            XPoint topLeft,
            XPoint bottomRight,
            ShapeStyle style,
            BaseShape point,
            bool isStroked = true,
            bool isFilled = false,
            string text = null,
            string name = "")
        {
            return new XRectangle()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                Data = new Data()
                {
                    Bindings = ImmutableArray.Create<Binding>(),
                    Properties = ImmutableArray.Create<Property>()
                },
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text,
                IsGrid = false,
                OffsetX = 30.0,
                OffsetY = 30.0,
                CellWidth = 30.0,
                CellHeight = 30.0
            };
        }
    }
}
