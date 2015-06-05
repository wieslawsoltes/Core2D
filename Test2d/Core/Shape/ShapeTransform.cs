// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class ShapeTransform : ObservableObject
    {
        private double _centerX;
        private double _centerY;
        private double _scaleX;
        private double _scaleY;
        private double _skewAngleX;
        private double _skewAngleY;
        private double _rotateAngle;
        private double _offsetX;
        private double _offsetY;

        /// <summary>
        /// 
        /// </summary>
        public double CenterX
        {
            get { return _centerX; }
            set { Update(ref _centerX, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double CenterY
        {
            get { return _centerY; }
            set { Update(ref _centerY, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ScaleX
        {
            get { return _scaleX; }
            set { Update(ref _scaleX, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ScaleY
        {
            get { return _scaleY; }
            set { Update(ref _scaleY, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double SkewAngleX
        {
            get { return _skewAngleX; }
            set { Update(ref _skewAngleX, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double SkewAngleY
        {
            get { return _skewAngleY; }
            set { Update(ref _skewAngleY, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double RotateAngle
        {
            get { return _rotateAngle; }
            set { Update(ref _rotateAngle, value); }
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
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="skewAngleX"></param>
        /// <param name="skewAngleY"></param>
        /// <param name="rotateAngle"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public static ShapeTransform Create(
            double centerX = 0.0,
            double centerY = 0.0,
            double scaleX = 1.0,
            double scaleY = 1.0,
            double skewAngleX = 0.0,
            double skewAngleY = 0.0,
            double rotateAngle = 0.0,
            double offsetX = 0.0,
            double offsetY = 0.0)
        {
            return new ShapeTransform()
            {
                CenterX = centerX,
                CenterY = centerY,
                ScaleX = scaleX,
                ScaleY = scaleY,
                SkewAngleX = skewAngleX,
                SkewAngleY = skewAngleY,
                RotateAngle = rotateAngle,
                OffsetX = offsetX,
                OffsetY = offsetY
            };
        }
    }
}
