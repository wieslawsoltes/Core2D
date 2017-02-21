// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Spatial.Sat
{
    public struct Projection
    {
        public readonly double Min;
        public readonly double Max;

        public Projection(double min, double max)
        {
            this.Min = min;
            this.Max = max;
        }

        public bool Overlap(Projection p)
        {
            return !(this.Min > p.Max || p.Min > this.Max);
        }

        public double GetOverlap(Projection p)
        {
            return !this.Overlap(p) ? 0.0 : Math.Abs(Math.Max(this.Min, p.Min) - Math.Min(this.Max, p.Max));
        }

        public bool Contains(Projection p)
        {
            return (this.Min <= p.Min && this.Max >= p.Max);
        }
    }
}
