// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// Specifies the arrow style used to draw line endings.
    /// </summary>
    public class ArrowStyle : ObservableObject
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
            set
            {
                if (value != _arrowType)
                {
                    _arrowType = value;
                    Notify("ArrowType");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets value indicating whether arrow shape is filled.
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
        /// Gets or sets arrow X axis radius.
        /// </summary>
        public double RadiusX
        {
            get { return _radiusX; }
            set
            {
                if (value != _radiusX)
                {
                    _radiusX = value;
                    Notify("RadiusX");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets arrow Y axis radius.
        /// </summary>
        public double RadiusY
        {
            get { return _radiusY; }
            set
            {
                if (value != _radiusY)
                {
                    _radiusY = value;
                    Notify("RadiusY");
                }
            }
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
            double radiusX = 0.0,
            double radiusY = 0.0)
        {
            return new ArrowStyle() 
            { 
                ArrowType = arrowType,
                IsFilled = isFilled,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
        }
    }
}
