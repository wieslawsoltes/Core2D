// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Spatial.Sat
{
    public struct MinimumTranslationVector
    {
        public readonly Vector2 Smallest;
        public readonly double Overlap;

        public MinimumTranslationVector(Vector2 smallest, double overlap)
        {
            this.Smallest = smallest;
            this.Overlap = overlap;
        }
    }
}
