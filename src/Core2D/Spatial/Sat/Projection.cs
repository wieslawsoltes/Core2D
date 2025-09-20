// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

using System;

namespace Core2D.Spatial.Sat;

public struct Projection
{
    public readonly double Min;
    public readonly double Max;

    public Projection(double min, double max)
    {
        Min = min;
        Max = max;
    }

    public bool Overlap(Projection p)
    {
        return !(Min > p.Max || p.Min > Max);
    }

    public double GetOverlap(Projection p)
    {
        return !Overlap(p) ? 0.0 : Math.Abs(Math.Max(Min, p.Min) - Math.Min(Max, p.Max));
    }

    public bool Contains(Projection p)
    {
        return Min <= p.Min && Max >= p.Max;
    }
}