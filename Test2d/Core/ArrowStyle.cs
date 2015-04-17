// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    public class ArrowStyle : ObservableObject
    {
        private ArrowType _arrowType;
        private bool _isFilled;
        private double _radiusX;
        private double _radiusY;
        
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
