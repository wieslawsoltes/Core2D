// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;

namespace Core2D.Spatial.Sat;

public struct MinimumTranslationVector
{
    public readonly Vector2 Smallest;
    public readonly double Overlap;

    public MinimumTranslationVector(Vector2 smallest, double overlap)
    {
        Smallest = smallest;
        Overlap = overlap;
    }
}
