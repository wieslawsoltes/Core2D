// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using static System.Math;

namespace Core2D.Math.Sat
{
    /// <summary>
    /// Projection for Separating Axis Theorem implementation.
    /// </summary>
    public struct Projection
    {
        /// <summary>
        /// 
        /// </summary>
        public double Min { get; }

        /// <summary>
        /// 
        /// </summary>
        public double Max { get; }

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
        public bool Overlap(Projection p) => !(this.Min > p.Max || p.Min > this.Max);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double GetOverlap(Projection p) => !this.Overlap(p) ? 0.0 : Abs(Max(this.Min, p.Min) - Min(this.Max, p.Max));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Projection p) => (this.Min <= p.Min && this.Max >= p.Max);
    }
}
