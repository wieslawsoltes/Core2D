// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XTransform
    {
        /// <summary>
        /// 
        /// </summary>
        public double CenterX { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double CenterY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double ScaleX { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double ScaleY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double SkewAngleX { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double SkewAngleY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double RotateAngle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double OffsetY { get; set; }

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
        public static XTransform Create(
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
            return new XTransform()
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
