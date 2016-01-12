// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// Projection for Separating Axis Theorem implementation.
    /// </summary>
    public struct Projection
    {
        /// <summary>
        /// 
        /// </summary>
        public double Min { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double Max { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Projection(double min, double max)
            : this()
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Overlap(Projection p)
        {
            return !(this.Min > p.Max || p.Min > this.Max);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double GetOverlap(Projection p)
        {
            return !this.Overlap(p) ?
                0.0 :
                Math.Abs(Math.Max(this.Min, p.Min) - Math.Min(this.Max, p.Max));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Projection p)
        {
            return (this.Min <= p.Min && this.Max >= p.Max);
        }
    }
}
