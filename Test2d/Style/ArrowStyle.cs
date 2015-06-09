// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// Specifies the arrow style used to draw line endings.
    /// </summary>
    public class ArrowStyle : BaseStyle
    {
        private ArrowType _arrowType;
        private bool _isFilled;
        private double _radiusX;
        private double _radiusY;
        
        /// <summary>
        /// Gets or sets arrow type.
        /// </summary>
        public ArrowType ArrowType
        {
            get { return _arrowType; }
            set { Update(ref _arrowType, value); }
        }
        
        /// <summary>
        /// Gets or sets value indicating whether arrow shape is filled.
        /// </summary>
        public bool IsFilled
        {
            get { return _isFilled; }
            set { Update(ref _isFilled, value); }
        }
        
        /// <summary>
        /// Gets or sets arrow X axis radius.
        /// </summary>
        public double RadiusX
        {
            get { return _radiusX; }
            set { Update(ref _radiusX, value); }
        }
        
        /// <summary>
        /// Gets or sets arrow Y axis radius.
        /// </summary>
        public double RadiusY
        {
            get { return _radiusY; }
            set { Update(ref _radiusY, value); }
        }

        /// <summary>
        /// Default ArrowStyle constructor.
        /// </summary>
        public ArrowStyle()
        {
        }

        /// <summary>
        /// Copy base style from BaseStyle source.
        /// </summary>
        /// <param name="source">The BaseStyle source object.</param>
        public ArrowStyle(BaseStyle source)
        {
            Stroke = ArgbColor.Create
                (source.Stroke.A, 
                 source.Stroke.R, 
                 source.Stroke.G, 
                 source.Stroke.B);
            Fill = ArgbColor.Create
                (source.Fill.A, 
                 source.Fill.R, 
                 source.Fill.G, 
                 source.Fill.B);
            Thickness = source.Thickness;
            LineCap = source.LineCap;
            Dashes = source.Dashes != null ? source.Dashes.ToArray() : default(double[]);
            DashOffset = source.DashOffset;
        }
        
        /// <summary>
        /// Creates a new instance of the ArrowStyle class.
        /// </summary>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isFilled">The arow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the ArrowStyle class.</returns>
        public static ArrowStyle Create(
            ArrowType arrowType = ArrowType.None,
            bool isFilled = false,
            double radiusX = 5.0,
            double radiusY = 3.0)
        {
            return new ArrowStyle() 
            { 
                ArrowType = arrowType,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }
        
        /// <summary>
        /// Creates a new instance of the ArrowStyle class.
        /// </summary>
        /// <param name="source">The BaseStyle source object.</param>
        /// <param name="arrowType">The arrow type.</param>
        /// <param name="isFilled">The arow shape fill flag.</param>
        /// <param name="radiusX">The arrow X axis radius.</param>
        /// <param name="radiusY">The arrow Y axis radius.</param>
        /// <returns>The new instance of the ArrowStyle class.</returns>
        public static ArrowStyle Create(
            BaseStyle source,
            ArrowType arrowType = ArrowType.None,
            bool isFilled = false,
            double radiusX = 5.0,
            double radiusY = 3.0)
        {
            return new ArrowStyle(source) 
            { 
                ArrowType = arrowType,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }
    }
}
